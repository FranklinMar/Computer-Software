using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Xml.Linq;
using LexSyntax_Analyzer;

namespace ParallelTree_Builder;

public class TreeBuilder
{
    private SyntaxAnalyzer Analyzer;
    public Tree Root { get; private set; }

    private static Dictionary<string, int> Priorities = new()
    {
        { "^", 3 },
        { "*", 2 },
        { "/", 2 },
        { "%", 2 },
        { "+", 1 },
        { "-", 1 }
    };
    private static string Negatives = "-/";
    private static Dictionary<string, string> OppositeOp = new()
    { 
        { "+", "-" },
        { "-", "+" },
        { "*", "/" },
        { "/", "*" }
    };

    private static bool IsNegative(string Operator) => Negatives.Contains(Operator);

    public TreeBuilder(SyntaxAnalyzer Analyzer)
    {
        this.Analyzer = Analyzer;
        if (this.Analyzer.Errors.Count > 0)
        {
            throw new ArgumentException("No errors allowed in expression tree", nameof(Analyzer));
        }

        var PostFix = ToPostfixList(this.Analyzer.Tokens);
        
        PostFix.ForEach(Token => Debug.Write(Token.Value + ", "));

        Tree TreeNode;
        Stack<Tree> TreeStack = new();
        for (int i = 0; i < PostFix.Count; i++)
        {
            if (!PostFix[i].IsOp)
            {
                TreeNode = new TreeValue(PostFix[i].Value, PostFix[i].Category);
            }
            else
            {
                var Right = TreeStack.Pop();
                var Left = TreeStack.Pop();
                TreeNode = new TreeNode(PostFix[i].Value, PostFix[i].Category, Left, Right);
            }
            TreeStack.Push(TreeNode);
        }
        Root = TreeStack.Count > 0 ? TreeStack.Pop() : null;

        if (Root == null)
        {
            return;
        }
        Root = OptimizeConstants(Root);
        if (Root is TreeNode && Priorities[((TreeNode) Root).Value] <= 2)
        {
            TreeNode Node = (TreeNode) Root;
            string Sign = IsNegative(Node.Value) ? OppositeOp[Node.Value] : Node.Value;
            Category Category = Node.Category;
            List <Tree> BinaryTreeQueue = new List<Tree>();
            List<Tree> MultiNodes = new List<Tree>();
            List<bool> MultiSigns = new List<bool>();
            ToMultiNodeTree(Node, Sign, Category, MultiNodes, MultiSigns);
            
            TreeMultiNode CurrentMulti = new(Sign, Category, MultiNodes, MultiSigns);
            
            CurrentMulti.Sort();

            Root = ToTreeNodeTree(CurrentMulti);
            Root = OptimizeConstants(Root, true);

            if (Root is TreeNode)
            {
                Node = (TreeNode)Root;
                Node.Traverse(Tree =>
                {
                    if (Tree is TreeNode)
                    {
                        TreeNode Leaf = (TreeNode)Tree;
                        if (Leaf.Value is "/" or "%" && Leaf.Right.Category == Category.Number && Leaf.Right.Value == "0")
                        {
                            throw new DivideByZeroException("Cannot divide by 0");
                        }
                    }
                });
            }
        }
    }

    public List<Token> ToPostfixList(List<Token> Tokens)
    {
        Stack<Token> PostFixStack = new();
        Stack<Token> Stack = new();

        Token StackToken;
        foreach (var Token in Tokens)
        {
            if (!(Token.IsOp || Token.Category == Category.Parenthesis/*"parenthesis"*/))
            {
                PostFixStack.Push(Token);
            }
            else
            {
                if (Token.Value == "(")
                {
                    Stack.Push(Token);
                }
                else if (Token.Value == ")")
                {
                    StackToken = Stack.Pop();
                    while (StackToken.Value != "(")
                    {
                        PostFixStack.Push(StackToken);
                        StackToken = Stack.Pop();
                    }
                }
                else
                {
                    while (Stack.Count > 0)
                    {
                        StackToken = Stack.Pop();
                        if (Priorities.GetValueOrDefault(StackToken.Value, -1) >=
                            Priorities.GetValueOrDefault(Token.Value, -1))
                        {
                            PostFixStack.Push(StackToken);
                        }
                        else
                        {
                            Stack.Push(StackToken);
                            break;
                        }
                    }

                    Stack.Push(Token);
                }
            }
        }

        while (Stack.Count > 0)
        {
            PostFixStack.Push(Stack.Pop());
        }
        return PostFixStack.Reverse().ToList();
    }

    public void ToMultiNodeTree(TreeNode TreeNode, string Sign, Category Category, List<Tree> MultiNodes, List<bool> MultiSigns/*, int Layer = 0*/, bool Negate = false)
    {
        if (TreeNode.Left is TreeNode && Priorities[TreeNode.Left.Value] == Priorities[Sign])
        {
            ToMultiNodeTree((TreeNode)TreeNode.Left, Sign, Category, MultiNodes, MultiSigns/*, Layer + 1*/, Negate);
        }
        else if (TreeNode.Left is TreeNode)
        {
            TreeNode Node = (TreeNode)TreeNode.Left;
            string NewSign = IsNegative(Node.Value) ? OppositeOp[Node.Value] : Node.Value;
            List<Tree> InnerMultiNodes = new();
            List<bool> InnerMultiSigns = new();
            ToMultiNodeTree(Node, NewSign, Category, InnerMultiNodes, InnerMultiSigns/*, TreeNode*/);
            MultiNodes.Add(new TreeMultiNode(NewSign, Category, InnerMultiNodes, InnerMultiSigns));
            MultiSigns.Add(Negate);
        }
        if (TreeNode.Right is TreeNode && Priorities[TreeNode.Right.Value] == Priorities[Sign])
        {
            ToMultiNodeTree((TreeNode)TreeNode.Right, Sign, Category, MultiNodes, MultiSigns/*, Layer + 1*/, IsNegative(TreeNode.Value) ^ Negate);
        }
        else if (TreeNode.Right is TreeNode)
        {
            TreeNode Node = (TreeNode)TreeNode.Right;
            string NewSign = IsNegative(Node.Value) ? OppositeOp[Node.Value] : Node.Value;
            List<Tree> NewMultiOpNodes = new();
            List<bool> NewMultiOpSigns = new();
            ToMultiNodeTree(Node, NewSign, Node.Category, NewMultiOpNodes, NewMultiOpSigns);
            MultiNodes.Add(new TreeMultiNode(NewSign, Node.Category, NewMultiOpNodes, NewMultiOpSigns));
            MultiSigns.Add(!(IsNegative(TreeNode.Value) ^ Negate));
        }
        if (TreeNode.Left is TreeValue || (TreeNode.Left is TreeNode && Priorities[TreeNode.Left.Value] > 2)) 
        {
            MultiNodes.Insert(0, TreeNode.Left);
            MultiSigns.Insert(0, !Negate);
        }
        if (TreeNode.Right is TreeValue || (TreeNode.Right is TreeNode && Priorities[TreeNode.Right.Value] > 2)
        {
            MultiNodes.Add(TreeNode.Right);
            MultiSigns.Add(!(IsNegative(TreeNode.Value) ^ Negate));
        }
    }

    public Tree ToTreeNodeTree(TreeMultiNode Tree)
    {
        if (Tree.Values.Count == 0)
        {
            throw new ArgumentException("Cannot create empty tree");
        } else if (Tree.Values.Count == 1)
        {
            if (Tree.Values[0] is TreeMultiNode)
            {
                return ToTreeNodeTree((TreeMultiNode) Tree.Values[0]);
            }
            return Tree.Values[0];
        } else if (Tree.Values.Count == 2)
        {
            var LeftNode = Tree.Values[0].Category.HasFlag(Category.Object) ? Tree.Values[0] : ToTreeNodeTree((TreeMultiNode) Tree.Values[0]);
            var RightNode = Tree.Values[1].Category.HasFlag(Category.Object) ? Tree.Values[1] : ToTreeNodeTree((TreeMultiNode) Tree.Values[1]);
            return new TreeNode(Tree.Signs[1] ? Tree.Value : OppositeOp[Tree.Value], Tree.Category, LeftNode, RightNode);
        }
        int Middle = (Tree.Values.Count + 1) / 2;
        Tree Left = ToTreeNodeTree(new TreeMultiNode(Tree.Value, Tree.Category, Tree.Values[..Middle], Tree.Signs[..Middle]));
        Tree Right;
        if (!Tree.Signs[Middle])
        {
            Right = ToTreeNodeTree(new TreeMultiNode(Tree.Value, Tree.Category, Tree.Values[Middle..], Tree.Signs[Middle..].Select(item => !item).ToList()));
            return new TreeNode(OppositeOp[Tree.Value], Tree.Category, Left, Right);
        } else
        {
            Right = ToTreeNodeTree(new TreeMultiNode(Tree.Value, Tree.Category, Tree.Values[Middle..], Tree.Signs[Middle..]));
            return new TreeNode(Tree.Value, Tree.Category, Left, Right);
        }
    }

    public Tree OptimizeConstants(Tree Tree, bool SeparateNegative = false)
    {
        if (Tree == null)
        {
            return Tree;
        }
        List<Tree> Leaves = new();
        int Count, NewCount;
        do
        {
          Count = 0;
          NewCount = 0;
            if (Tree is TreeNode)
            {
                ((TreeNode) Tree).Traverse(Node =>
                {
                    if (Node is TreeNode && ((TreeNode)Node).Left.Category.HasFlag(Category.Object) && ((TreeNode)Node).Right.Category.HasFlag(Category.Object))
                    {
                        Leaves.Add(Node);
                    }
                    ++Count;
                });
            }
            foreach (TreeNode Leaf in Leaves)
            {
                if (Leaf.Value is "/" or "%" && Leaf.Right?.Category == Category.Number && double.Parse(Leaf.Right.Value) == 0.0)
                {
                    throw new DivideByZeroException("Cannot divide by 0");
                } 
                Tree NewNode = null;
                if (Leaf.Left.Category == Category.Number && Leaf.Right.Category == Category.Number)
                {
                    double Temp;
                    double Left = double.Parse(Leaf.Left.Value.Replace('.', ',')), Right = double.Parse(Leaf.Right.Value.Replace('.', ','));
                    Temp = Leaf.Value switch
                    {
                        "+" => Left + Right,
                        "-" => Left - Right,
                        "*" => Left * Right,
                        "/" => Left / Right,
                        "%" => Left % Right,
                        "^" => Math.Pow(Left, Right),
                        _ => throw new DataException($"Couldn't parse expression: {Leaf.Value}")
                    };
                    if (Temp < 0 && SeparateNegative)
                    {
                        NewNode = new TreeNode("-", Category.Op, new TreeValue("0", Category.Number), new TreeValue(Math.Abs((decimal)Temp).ToString("G29"), Category.Number));
                    }
                    else
                    {
                        NewNode = new TreeValue(((decimal) Temp).ToString("G29"), Category.Number);
                    }
                }
                else if (Leaf.Left.Category == Category.Name && Leaf.Right.Category == Category.Number ||
                         Leaf.Left.Category == Category.Number && Leaf.Right.Category == Category.Name)
                {
                    //double Left = double.Parse(Leaf.Left.Value.Replace('.', ',')), Right = double.Parse(Leaf.Right.Value.Replace('.', ','));
                    if ((Leaf.Value is "+" && (Leaf.Left.Value == "0" || Leaf.Right.Value == "0")) ||
                        Leaf is { Value: "-", Right.Value: "0" })
                    {
                        NewNode = Leaf.Left.Value != "0" ? Leaf.Left : Leaf.Right;
                    }
                    else if (Leaf.Value is "*" or "/" or "%" &&
                        /*(Left <= double.Epsilon || // Left == 0 ||
                        Right <= double.Epsilon)) // Right == 0*/
                        (Leaf.Left.Value == "0" || Leaf.Right.Value == "0"))
                    {
                        NewNode = Leaf.Left.Value == "0" /*Math.Abs(Left) <= double.Epsilon*/ ? Leaf.Left : Leaf.Right; // Left == 0 ? ...
                    }
                    else if ((Leaf.Value is "*" &&
                            /*(Left - 1 <= double.Epsilon || // Left == 1
                            Right - 1 <= double.Epsilon)) // Right == 1*/
                            (Leaf.Left.Value == "1" || Leaf.Right.Value == "1")) ||
                             Leaf is { Value: "/", Right.Value: "1" })
                    {
                        NewNode = Leaf.Left.Value != "1" ? Leaf.Left : Leaf.Right;
                    }
                }
                else if (Leaf.Left.Value == Leaf.Right.Value)
                {
                    if (Leaf.Value == "-")
                    {
                        NewNode = new TreeValue($"{0}", Category.Number);
                    }
                    else if (Leaf.Value == "/" && Leaf.Right.Category == Category.Number && Math.Abs(double.Parse(Leaf.Right.Value)) >= double.Epsilon)
                    {
                        NewNode = new TreeValue($"{1}", Category.Number);
                    }
                    else if (Leaf.Value == "%" && Leaf.Right.Category == Category.Number && Math.Abs(double.Parse(Leaf.Right.Value)) >= double.Epsilon)
                    {
                        NewNode = new TreeValue($"{0}", Category.Number);
                    }
                }

                if (NewNode != null)
                {
                    if (Tree == Leaf)
                    {
                        return NewNode;
                    }
                    if (Tree is TreeNode)
                    {
                        ((TreeNode)Tree).Traverse(TreeNode =>
                        {
                            if (TreeNode is TreeNode && ((TreeNode)TreeNode).Left == Leaf)
                            {
                                ((TreeNode)TreeNode).Left = NewNode;
                            }
                            else if (TreeNode is TreeNode && ((TreeNode)TreeNode).Right == Leaf)
                            {
                                ((TreeNode)TreeNode).Right = NewNode;
                            }
                        });
                    }
                }
            }
            if (Tree is TreeNode)
            {
                ((TreeNode)Tree).Traverse(_ => ++NewCount);
            }
        } while (Count != NewCount);

        return Tree;
    }
}