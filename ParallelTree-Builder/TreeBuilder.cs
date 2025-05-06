using System.Data;
using System.Diagnostics;
using LexSyntax_Analyzer;

namespace ParallelTree;

public static class TreeBuilder
{
    //private SyntaxAnalyzer Analyzer;
    //public Tree Root { get; private set; }

    public static Dictionary<string, int> OperationPriorities = new()
    {
        { "^", 3 },
        { "*", 2 },
        { "/", 2 },
        { "%", 2 },
        { "+", 1 },
        { "-", 1 }
    };
    public static string Negatives = "-/";
    public static Dictionary<string, string> NegateOperation = new()
    {
        { "+", "-" },
        { "-", "+" },
        { "*", "/" },
        { "/", "*" }
    };

    private static double CalculateOperation(string Operation, double Left, double Right)
    {
        return Operation switch
                    {
                        "+" => Left + Right,
                        "-" => Left - Right,
                        "*" => Left* Right,
                        "/" => Left / Right,
                        "%" => Left % Right,
                        "^" => Math.Pow(Left, Right),
                        _ => throw new DataException($"Couldn't parse expression: {Operation}")
                    };
}

    private static bool IsNegative(string Operator) => Negatives.Contains(Operator);

    public static Tree Parse(SyntaxAnalyzer Analyzer)
    {
        //this.Analyzer = Analyzer;
        Tree Root;
        if (Analyzer.Errors.Count > 0)
        {
            throw new ArgumentException("No errors allowed in expression tree", nameof(Analyzer));
        }

        var PostFix = ToPostfixList(Analyzer.Tokens);

        PostFix.ForEach(Token => Debug.Write(Token.Value + ", "));

        Tree TreeNode;
        Stack<Tree> TreeStack = new();
        for (int i = 0; i < PostFix.Count; i++)
        {
            if (!PostFix[i].IsOp)
            {
                TreeNode = new TreeValue(PostFix[i].Value);
            }
            else
            {
                var Right = TreeStack.Pop();
                var Left = TreeStack.Pop();
                TreeNode = new TreeNode(PostFix[i].Value, Left, Right);
            }
            TreeStack.Push(TreeNode);
        }
        Root = TreeStack.Count > 0 ? TreeStack.Pop() : null;

        if (Root == null)
        {
            return Root;
        }
        Root = OptimizeConstants(Root);
        if (Root is TreeNode && OperationPriorities[((TreeNode)Root).Value] <= 2)
        {
            TreeNode Node = (TreeNode)Root;

            TreeMultiNode CurrentMulti = Node.ToTreeMultiNode();
            Root = OptimizeConstants(CurrentMulti);

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
                        if (Leaf.Value is "/" or "%" && decimal.TryParse(Leaf.Right.Value, out _) && decimal.Parse(Leaf.Right.Value) == 0m)
                        {
                            throw new DivideByZeroException("Cannot divide by 0");
                        }
                    }
                });
            }
        }
        return Root;
    }

    public static TreeMultiNode ToTreeMultiNode(this TreeNode Node)
    {
        string Sign = IsNegative(Node.Value) ? NegateOperation[Node.Value] : Node.Value;
        List<Tree> MultiNodes = new List<Tree>();
        List<bool> MultiSigns = new List<bool>();
        ToMultiNodeTree(Node, Sign, MultiNodes, MultiSigns);
        return new TreeMultiNode(Sign, MultiNodes, MultiSigns);
    }

    public static TreeNode ToTreeNode(this TreeMultiNode Node)
    {
        return (TreeNode) ToTreeNodeTree(Node);
    }

    public static List<Token> ToPostfixList(List<Token> Tokens)
    {
        Stack<Token> PostFixStack = new();
        Stack<Token> Stack = new();

        Token StackToken;
        foreach (var Token in Tokens)
        {
            if (!(Token.IsOp || Token.Category == Category.Parenthesis))
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
                        if (OperationPriorities.GetValueOrDefault(StackToken.Value, -1) >=
                            OperationPriorities.GetValueOrDefault(Token.Value, -1))
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

    private static void ToMultiNodeTree(TreeNode TreeNode, string Sign, List<Tree> MultiNodes, List<bool> MultiSigns, bool Negate = false)
    {
        if (TreeNode.Left is TreeNode && OperationPriorities[TreeNode.Left.Value] == OperationPriorities[Sign])
        {
            ToMultiNodeTree((TreeNode)TreeNode.Left, Sign, MultiNodes, MultiSigns, Negate);
        }
        else if (TreeNode.Left is TreeNode)
        {
            TreeNode Node = (TreeNode)TreeNode.Left;
            string NewSign = IsNegative(Node.Value) ? NegateOperation[Node.Value] : Node.Value;
            List<Tree> InnerMultiNodes = new();
            List<bool> InnerMultiSigns = new();
            ToMultiNodeTree(Node, NewSign, InnerMultiNodes, InnerMultiSigns);
            MultiNodes.Add(new TreeMultiNode(NewSign,InnerMultiNodes, InnerMultiSigns));
            MultiSigns.Add(!Negate);
        }
        if (TreeNode.Right is TreeNode && OperationPriorities[TreeNode.Right.Value] == OperationPriorities[Sign])
        {
            ToMultiNodeTree((TreeNode)TreeNode.Right, Sign, MultiNodes, MultiSigns, IsNegative(TreeNode.Value) ^ Negate);
        }
        else if (TreeNode.Right is TreeNode)
        {
            TreeNode Node = (TreeNode)TreeNode.Right;
            string NewSign = IsNegative(Node.Value) ? NegateOperation[Node.Value] : Node.Value;
            List<Tree> NewMultiOpNodes = new();
            List<bool> NewMultiOpSigns = new();
            ToMultiNodeTree(Node, NewSign, NewMultiOpNodes, NewMultiOpSigns);
            MultiNodes.Add(new TreeMultiNode(NewSign, NewMultiOpNodes, NewMultiOpSigns));
            MultiSigns.Add(!(IsNegative(TreeNode.Value) ^ Negate));
        }
        if (TreeNode.Left is TreeValue || (TreeNode.Left is TreeNode && OperationPriorities[TreeNode.Left.Value] > 2))
        {
            MultiNodes.Insert(0, TreeNode.Left);
            MultiSigns.Insert(0, !Negate);
        }
        if (TreeNode.Right is TreeValue || (TreeNode.Right is TreeNode && OperationPriorities[TreeNode.Right.Value] > 2))
        {
            MultiNodes.Add(TreeNode.Right);
            MultiSigns.Add(!(IsNegative(TreeNode.Value) ^ Negate));
        }
    }

    private static Tree ToTreeNodeTree(TreeMultiNode Tree)
    {
        Tree.Sort();
        if (Tree.Values.Count == 0)
        {
            throw new ArgumentException("Cannot create empty tree");
        } else if (Tree.Values.Count == 1)
        {
            if (Tree.Values[0] is TreeMultiNode)
            {
                return ToTreeNodeTree((TreeMultiNode)Tree.Values[0]);
            }
            return Tree.Values[0];
        } else if (Tree.Values.Count == 2)
        {
            var LeftNode = Tree.Values[0] is TreeValue or TreeNode ? Tree.Values[0] : ToTreeNodeTree((TreeMultiNode)Tree.Values[0]);
            var RightNode = Tree.Values[1] is TreeValue or TreeNode ? Tree.Values[1] : ToTreeNodeTree((TreeMultiNode)Tree.Values[1]);
            return new TreeNode(Tree.Signs[1] ? Tree.Value : NegateOperation[Tree.Value], LeftNode, RightNode);
        }
        int Middle = (Tree.Values.Count + 1) / 2;
        Tree Left = ToTreeNodeTree(new TreeMultiNode(Tree.Value, Tree.Values[..Middle], Tree.Signs[..Middle]));
        Tree Right;
        if (!Tree.Signs[Middle])
        {
            Right = ToTreeNodeTree(new TreeMultiNode(Tree.Value, Tree.Values[Middle..], Tree.Signs[Middle..].Select(item => !item).ToList()));
            return new TreeNode(NegateOperation[Tree.Value], Left, Right);
        } else
        {
            Right = ToTreeNodeTree(new TreeMultiNode(Tree.Value, Tree.Values[Middle..], Tree.Signs[Middle..]));
            return new TreeNode(Tree.Value, Left, Right);
        }
    }

    public static Tree OptimizeConstants(TreeMultiNode Tree)
    {
        Tree.Sort();
        int Priority = OperationPriorities[Tree.Value];
        double Result = Priority switch
        {
            1 => 0,
            2 => 1,
            _ => 1
        };
        List<Tree> Values = new(Tree.Values);
        List<bool> Signs = new(Tree.Signs);
        List<int> RemoveIndexes = new();
        for (int i = 0; i < Values.Count; i++)
        {
            if (Values[i] is TreeMultiNode)
            {
                Values[i] = OptimizeConstants((TreeMultiNode) Values[i]);
            } else if (double.TryParse(Values[i].Value, out _))
            {
                Result = CalculateOperation(Signs[i] ? Tree.Value : NegateOperation[Tree.Value], Result, double.Parse(Values[i].Value.Replace('.', ',')));
                RemoveIndexes.Add(i);
                //Tree.Values.RemoveAt(0);
                //Tree.Signs.RemoveAt(0);
            }
        }
        RemoveIndexes.Reverse();
        for (int i = 0; i < RemoveIndexes.Count; i++)
        {
            Tree.Values.RemoveAt(RemoveIndexes[i]);
            Tree.Signs.RemoveAt(RemoveIndexes[i]);
        }
        bool Sign = true;
        if (Tree.Values.Count == 1)
        {
            if (Priority == 1 && ((decimal)Result) != 0)
            {
                Tree.Values.Insert(0, new TreeValue(Math.Abs((decimal)Result).ToString("G29")));
                Sign = Result >= 0;
                Tree.Signs.Insert(0, Sign);
            }
            else if (Priority == 2 && ((decimal)Result) != 1)
            {
                Tree.Values.Insert(0, new TreeValue(Math.Abs((decimal)Result).ToString("G29")));
                Tree.Signs.Insert(0, Sign);
            } else
            {
                return Tree.Values[0];
            }
        }
        return Tree;
        //Tree.Values.Insert(0, true);
    }

    public static Tree OptimizeConstants(Tree Tree, bool SeparateNegative = false)
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
                    if (Node is TreeNode && ((TreeNode)Node).Left is TreeValue && ((TreeNode)Node).Right is TreeValue)
                    {
                        Leaves.Add(Node);
                    }
                    ++Count;
                });
            }
            foreach (TreeNode Leaf in Leaves)
            {
                if (Leaf.Value is "/" or "%" && decimal.TryParse(Leaf.Right.Value, out _) && decimal.Parse(Leaf.Right.Value) == 0.0m)
                {
                    throw new DivideByZeroException("Cannot divide by 0");
                } 
                Tree NewNode = null;
                if (decimal.TryParse(Leaf.Left.Value, out _) && decimal.TryParse(Leaf.Right.Value, out _))
                {
                    double Left = double.Parse(Leaf.Left.Value.Replace('.', ',')), 
                        Right = double.Parse(Leaf.Right.Value.Replace('.', ','));
                    double Temp;
                    Temp = CalculateOperation(Leaf.Value, Left, Right);
                    if (Temp < 0 && SeparateNegative)
                    {
                        NewNode = new TreeNode("-", new TreeValue("0"), new TreeValue(Math.Abs((decimal)Temp).ToString("G29")));
                    }
                    else
                    {
                        NewNode = new TreeValue(((decimal) Temp).ToString("G29"));
                    }
                }
                else if (decimal.TryParse(Leaf.Right.Value, out _) || decimal.TryParse(Leaf.Left.Value, out _))
                {
                    double? Left = decimal.TryParse(Leaf.Left.Value, out _) ? double.Parse(Leaf.Left.Value.Replace('.', ',')) : null,
                        Right = decimal.TryParse(Leaf.Right.Value, out _) ? double.Parse(Leaf.Right.Value.Replace('.', ',')) : null;
                    if (Leaf.Value is "+" && ((Left != null && (decimal) Left == 0) || (Right != null && (decimal) Right == 0)))
                    {
                        NewNode = Left != null && (decimal) Left != 0 ? Leaf.Left : Leaf.Right;
                    } else if (Leaf.Value is "-" && (Right != null && (decimal) Right == 0)) {
                        NewNode = Leaf.Left;
                    }
                    else if (Leaf.Value is "*" &&
                        ((Left != null && (decimal) Left == 0) || (Right != null && (decimal) Right == 0)))
                    {
                        NewNode = Left != null && (decimal) Left == 0 ? Leaf.Left : Leaf.Right;
                    }
                    else if (Leaf.Value is "/" or "%" && (decimal) Left == 0) {
                        NewNode = Leaf.Left;
                    }
                    else if ((Leaf.Value is "*" &&
                            ((Left != null && (decimal) Left == 1) || (Right != null && (decimal) Right == 1))) ||
                             (Leaf.Value == "/" && (Right != null && (decimal) Right == 1)))
                    {
                        NewNode = Left != null && (decimal) Left != 1 ? Leaf.Left : Leaf.Right;
                    }
                }
                else if (Leaf.Left.Value == Leaf.Right.Value)
                {
                    double? Right = decimal.TryParse(Leaf.Right.Value, out _) ? double.Parse(Leaf.Right.Value.Replace('.', ',')) : null;
                    if (Leaf.Value == "-")
                    {
                        NewNode = new TreeValue($"{0}");
                    }
                    else if (Leaf.Value == "/" && Right != null && (decimal) Right >= 0)
                    {
                        NewNode = new TreeValue($"{1}");
                    }
                    else if (Leaf.Value == "%" && Right != null && (decimal) Right >= 0)
                    {
                        NewNode = new TreeValue($"{0}");
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