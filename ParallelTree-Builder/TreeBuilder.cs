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
                /*if ("+-/*".Contains(PostFix[i].Value))
                {
                    int j;
                    List<Tree> Values = new List<Tree>();
                    List<bool> Signs = new List<bool>();
                    for (j = i; j < PostFix.Count; j++)
                    {
                        if (PostFix[j].IsOp && Priorities[PostFix[j].Value] != Priorities[PostFix[i].Value])
                        {
                            break;
                        }
                        else if (PostFix[j].IsOp)
                        {
                            Signs.Insert(0, !IsNegative(PostFix[j].Value));
                        }
                        else
                        {
                            Values.Insert(0, TreeStack.Pop());
                            //Values.Insert(0, new TreeValue(PostFix[j].Value, PostFix[j].Category));
                        }
                    }

                    TreeNode = new MultiOperator(IsNegative(PostFix[i].Value) ? OppositeOp[PostFix[i].Value] : PostFix[i].Value, Values, Signs);
                    i = j - 1;
                }
                else
                {
                    var Right = TreeStack.Pop();
                    var Left = TreeStack.Pop();
                    TreeNode = new TreeNode(PostFix[i].Value, PostFix[i].Category, Left, Right);
                }*/
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

        Tree Current = Root;
        Root = OptimizeConstants(Root);
        if (Current is TreeNode && Priorities[((TreeNode) Current).Value] <= 2)
        {
            TreeNode Node = (TreeNode) Current;
            string Sign = IsNegative(Node.Value) ? OppositeOp[Node.Value] : Node.Value;
            Category Category = Node.Category;
            List <Tree> BinaryTreeQueue = new List<Tree>();
            List<Tree> MultiNodes = new List<Tree>();
            List<bool> MultiSigns = new List<bool>();
            ToMultiNodeTree(Node, Sign, Category, MultiNodes, MultiSigns);
            
            TreeMultiNode CurrentMulti = new(Sign, Category, MultiNodes, MultiSigns);
            
            CurrentMulti.Sort();

            Root = ToTreeNodeTree(CurrentMulti);
            Root = OptimizeConstants(Root);

            if (Root is TreeNode)
            {
                Node = (TreeNode)Root;
                Node.Traverse(Tree =>
                {
                    if (Tree is TreeNode)
                    {
                        TreeNode Leaf = (TreeNode)Tree;
                        if (Leaf.Value is "/" or "%" && Leaf.Right.Category == Category.Number && Math.Abs(double.Parse(Leaf.Right.Value)) < double.Epsilon)
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

    public void ToMultiNodeTree(TreeNode TreeNode, string Sign, Category Category, List<Tree> MultiNodes, List<bool> MultiSigns)
    {
        if (TreeNode.Left is TreeValue || (TreeNode.Left is TreeNode && Priorities[TreeNode.Left.Value] > 2)) 
        {
            MultiNodes.Insert(0, TreeNode.Left);
            MultiSigns.Insert(0, true);
        }
        /*if (TreeNode.Left is TreeNode && Priorities[((TreeNode)TreeNode.Left).Value] != Priorities[Sign])
        {

        }*/
        if (TreeNode.Left is TreeNode && Priorities[TreeNode.Left.Value] == Priorities[Sign])
        {
            ToMultiNodeTree((TreeNode) TreeNode.Left, Sign, Category, MultiNodes, MultiSigns);
        } else if (TreeNode.Left is TreeNode)
        {
            TreeNode Node = (TreeNode) TreeNode.Left;
            string NewSign = IsNegative(Node.Value) ? OppositeOp[Node.Value] : Node.Value;
            List<Tree> InnerMultiNodes = new();
            List<bool> InnerMultiSigns = new();
            ToMultiNodeTree(Node, NewSign, Category, InnerMultiNodes, InnerMultiSigns);
            MultiNodes.Insert(0, new TreeMultiNode(NewSign, Category, InnerMultiNodes, InnerMultiSigns));
            MultiSigns.Insert(0, !IsNegative(TreeNode.Value));
        }
        if (TreeNode.Right is TreeValue || (TreeNode.Right is TreeNode && Priorities[TreeNode.Right.Value] > 2)/* || TreeNode.Right is TreeNode && Priorities[((TreeNode)TreeNode.Right).Value] != Priorities[Sign]*/)
        //if (TreeNode.Right is Tree)
        {
            MultiNodes.Add(TreeNode.Right);
            MultiSigns.Add(!IsNegative(TreeNode.Value));
        }
        if (TreeNode.Right is TreeNode && Priorities[TreeNode.Right.Value] == Priorities[Sign])
        {
            ToMultiNodeTree((TreeNode) TreeNode.Right, Sign, Category, MultiNodes, MultiSigns);
        }
        else if (TreeNode.Right is TreeNode)
        {
            TreeNode Node = (TreeNode)TreeNode.Right;
            string NewSign = IsNegative(Node.Value) ? OppositeOp[Node.Value] : Node.Value;
            List<Tree> NewMultiOpNodes = new();
            List<bool> NewMultiOpSigns = new();
            ToMultiNodeTree(Node, NewSign, Node.Category, NewMultiOpNodes, NewMultiOpSigns);
            MultiNodes.Add(new TreeMultiNode(NewSign, Node.Category, NewMultiOpNodes, NewMultiOpSigns));
            MultiSigns.Add(!IsNegative(TreeNode.Value));
        }
    }

    public Tree ToTreeNodeTree(TreeMultiNode Tree)
    {
        //int Middle = Math.Max(((Tree.Values.Count + 1) / 2) / 2 * 2 - 1, 0);\
        if (Tree.Values.Count == 0)
        {
            throw new ArgumentException("Cannot create empty tree");
        } else if (Tree.Values.Count == 1)
        {
            if (Tree.Values[0] is TreeMultiNode)
            {
                return ToTreeNodeTree((TreeMultiNode) Tree.Values[0]);
            }
            //return new TreeValue(Tree.Signs[0] ? Tree.Value : OppositeOp[Tree.Value], Tree.Category);
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
        if (/*Priorities[Tree.Value] == 2 && */!Tree.Signs[Middle])
        {
            Right = ToTreeNodeTree(new TreeMultiNode(Tree.Value, Tree.Category, Tree.Values[Middle..], Tree.Signs[Middle..].Select(item => !item).ToList()));
            return new TreeNode(OppositeOp[Tree.Value], Tree.Category, Left, Right);
        } else
        {
            Right = ToTreeNodeTree(new TreeMultiNode(Tree.Value, Tree.Category, Tree.Values[Middle..], Tree.Signs[Middle..]));
            return new TreeNode(Tree.Value, Tree.Category, Left, Right);
        }
    }

    public Tree OptimizeConstants(Tree Tree)
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

                    NewNode = new TreeValue($"{Temp}", Category.Number);

                }
                else if (Leaf.Left.Category == Category.Name && Leaf.Right.Category == Category.Number ||
                         Leaf.Left.Category == Category.Number && Leaf.Right.Category == Category.Name)
                {
                    if ((Leaf.Value is "+" && (Leaf.Left.Value == "0" || Leaf.Right.Value == "0")) || 
                        Leaf is { Value: "-", Right.Value: "0" })
                    {
                        NewNode = Leaf.Left.Value != "0" ? Leaf.Left : Leaf.Right;
                    }
                    else if (Leaf.Value is "*" or "/" or "%" && (Leaf.Left.Value == "0" || Leaf.Right.Value == "0"))   
                    {
                        NewNode = Leaf.Left.Value == "0" ? Leaf.Left : Leaf.Right;
                    }
                    else if ((Leaf.Value is "*" && (Leaf.Left.Value == "1" || Leaf.Right.Value == "1")) || 
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
    /*private Tree Optimize(Tree Root)
    {
        if (Root == null || !Root.IsOp)
        {
            return Root;
        }
	    List<string> Expressions = new();
        List<Tree> CornerNodes = new();
        Tree Node = Root;
        List<Tree> RootNodes = new();
        List <Token> Tokens;
        List<Token> PositiveTokens, NegativeTokens;

        string Sign = Root.Value;
        Category Category = Root.Category;

        string Current = "";
        TraverseTree(Node, Sign, ref RootNodes, ref Current);
        Tokens = new StateAnalyzer(Current).Tokens;
        PositiveTokens = new() { Tokens[0] };
        NegativeTokens = new();
        for (int i = 1; i < Tokens.Count; i += 2)
        {
            if (IsNegative(Tokens[i].Value))
            {
                NegativeTokens.Add(Tokens[i]);
                NegativeTokens.Add(Tokens[i + 1]);
            }
            else
            {
                PositiveTokens.Add(Tokens[i]);
                PositiveTokens.Add(Tokens[i + 1]);
            }
        }
        PositiveTokens.AddRange(NegativeTokens);
        Tree Merged = SplitMerge(PositiveTokens);

        RootNodes.Add(Merged);
        Root = SplitMerge(RootNodes, Sign, Category);

        Root = OptimizeConstants(Root);
        return Root;
    }

    private void TraverseTree(Tree Node, string Sign, ref List<Tree> RootNodes, ref string Current)
    {
        if (Node is TreeNode)
        {
            TreeNode TreeNode = (TreeNode)Node;
            if (TreeNode.Left.IsOp && Priorities[TreeNode.Left.Value] == Priorities[Sign] && Priorities[Node.Value] != 3)
            {
                TraverseTree(TreeNode.Left, Sign, ref RootNodes, ref Current);
            }
            if (TreeNode.Right.IsOp && Priorities[TreeNode.Right.Value] == Priorities[Sign] && Priorities[Node.Value] != 3)
            {
                TraverseTree(TreeNode.Right, Sign, ref RootNodes, ref Current);
            }
            if (TreeNode.Left.IsOp && Priorities[TreeNode.Left.Value] != Priorities[Sign])
            {
                RootNodes.Add(TreeNode.Left);
            }
            if (TreeNode.Right.IsOp && Priorities[TreeNode.Right.Value] != Priorities[Sign])
            {
                RootNodes.Add(TreeNode.Right);
            }
            if (TreeNode.Left.Category.HasFlag(Category.Object) && TreeNode.Right.Category.HasFlag(Category.Object))
            {
                Current = TreeNode.Left.Value + Node.Value + TreeNode.Right.Value;
            }
            else if (TreeNode.Left.Category.HasFlag(Category.Object))
            {
                if (TreeNode.Right.IsOp && Priorities[TreeNode.Right.Value] == Priorities[Sign] && Priorities[Node.Value] != 3)
                {
                    Current = TreeNode.Left.Value + Node.Value + Current;
                }
                else
                {
                    Current = TreeNode.Left.Value;
                }
            }
            else if (TreeNode.Right.Category.HasFlag(Category.Object))
            {
                if (TreeNode.Left.IsOp && Priorities[TreeNode.Left.Value] == Priorities[Sign] && Priorities[Node.Value] != 3)
                {
                    Current += Node.Value + TreeNode.Right.Value;
                }
                else
                {
                    Current = TreeNode.Right.Value;
                }
            }
        }
    }

    private Tree SplitMerge(List<Tree> Nodes, string Sign, Category Category)
    {
        if (Nodes.Count == 0)
        {
            throw new ArgumentException("EMPTY!");
        } else if (Nodes.Count == 1)
        {
            return Nodes[0];
        }
        else if (Nodes.Count == 2)
        {
            return new TreeNode(Sign, Category, Nodes[0], Nodes[1]);
        }
        int Middle = (Nodes.Count - 1) / 2;
        //int Middle = Math.Max(((Nodes.Count + 1) / 2) / 2 * 2 - 1, 0);

        TreeNode Root = new(Sign, Category,
                SplitMerge(Nodes.GetRange(0, Middle), Sign, Category),
                SplitMerge(Nodes.GetRange(Middle, Nodes.Count - Middle), Sign, Category));

        return Root;
    }

    private Tree SplitMerge(List<Token> Tokens)
    {
        if (Tokens.Count == 0)
        {
            return null;
        }
        int Middle = 0;
        if (Tokens.Count > 1)
        {
            string Operation = Tokens[1].Value;
            bool SameOperations = true;
            for (int i = 1; i < Tokens.Count; i += 2)
            {
                if (Tokens[i].Value != Operation)
                {
                    SameOperations = false;
                    Middle = i;
                }
                if (!SameOperations)
                {
                    Tokens[i] = new Token(OppositeOp[Tokens[i].Value], Tokens[i].Category, Tokens[i].Index);
                }
            }
            if (SameOperations)
            {
                Middle = Math.Max(((Tokens.Count + 1) / 2) / 2 * 2 - 1, 0);
            }
        } else
        {
            return new TreeValue(Tokens[0].Value, Tokens[0].Category);
        }
        Token MiddleToken = Tokens[Middle];

        TreeNode Root = new(MiddleToken.Value,
                MiddleToken.Category,
                SplitMerge(Tokens.GetRange(0, Middle)),
                SplitMerge(Tokens.GetRange(Middle + 1, Tokens.Count - Middle - 1)));

        return Root;
    }*/
}