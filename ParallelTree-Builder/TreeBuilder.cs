using System.Data;
using System.Globalization;
using System.Security.Cryptography;
using LexSyntax_Analyzer;

namespace ParallelTree_Builder;

public class TreeBuilder
{
    private StateAnalyzer Analyzer;
    public TreeNode? Root { get; private set; }

    private static Dictionary<string, int> Priorities = new()
    {
        { "^", 3 },
        { "*", 2 },
        { "/", 2 },
        { "%", 2 },
        { "+", 1 },
        { "-", 1 }
    };

    public TreeBuilder(StateAnalyzer Analyzer)
    {
        this.Analyzer = Analyzer;
        if (this.Analyzer.Errors.Count > 0)
        {
            throw new ArgumentException("No errors allowed in expression tree", nameof(Analyzer));
        }

        var PostFix = ToPostfixList(this.Analyzer.Tokens);
        PostFix.ForEach(Token => Console.Write(Token.Value + ", "));

        TreeNode TreeNode;
        Stack<TreeNode> TreeStack = new();
        foreach (var Token in PostFix)
        {
            if (!Token.IsOp)
            {
                TreeNode = new TreeNode(Token);
            }
            else
            {
                var Right = TreeStack.Pop();
                var Left = TreeStack.Pop();
                TreeNode = new TreeNode(Token, Left, Right);
            }
            TreeStack.Push(TreeNode);
        }
        Root = TreeStack.Count > 0 ? TreeStack.Pop() : null;
        Root = OptimizeConstants(Root);
    }

    public List<Token> ToPostfixList(List<Token> Tokens)
    {
        Stack<Token> PostFixStack = new();
        Stack<Token> Stack = new();

        Token StackToken;
        foreach (var Token in Tokens)
        {
            if (!(Token.IsOp || Token.Category == "parentheses"))
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

    public TreeNode OptimizeConstants(TreeNode? Tree)
    {
        if (Tree == null)
        {
            return Tree;
        }
        List<TreeNode> Leaves = new();
        int Count, NewCount;
        do
        {
          Count = 0;
          NewCount = 0;
            // Tree.Traverse(_ => { ++Count; });
            Tree.Traverse(Node => { 
                if (Node.Left?.Category is "num" or "name" && Node.Right?.Category is "num" or "name")
                {
                    Leaves.Add(Node);
                }
                ++Count;
            });
            foreach (TreeNode Leaf in Leaves)
            {
                if (Leaf.Value is "/" or "%" && Leaf.Right?.Category == "num" && double.Parse(Leaf.Right?.Value) == 0.0)
                {
                    throw new DivideByZeroException("Cannot divide by 0");
                } 
                TreeNode? NewNode = null;
                if (Leaf.Left?.Category is "num" && Leaf.Right?.Category is "num")
                {
                    double Temp;
                    int Index;
                    Temp = Leaf.Value switch
                    {
                        "+" => double.Parse(Leaf.Left.Value) + double.Parse(Leaf.Right.Value),
                        "-" => double.Parse(Leaf.Left.Value) - double.Parse(Leaf.Right.Value),
                        "*" => double.Parse(Leaf.Left.Value) * double.Parse(Leaf.Right.Value),
                        "/" => double.Parse(Leaf.Left.Value) / double.Parse(Leaf.Right.Value),
                        "%" => double.Parse(Leaf.Left.Value) % double.Parse(Leaf.Right.Value),
                        "^" => Math.Pow(double.Parse(Leaf.Left.Value), double.Parse(Leaf.Right.Value)),
                        _ => throw new DataException($"Couldn't parse expression: {Leaf.Value}")
                    };

                    Index = (int)(Leaf.Left?.Index < Leaf.Right?.Index ? Leaf.Left?.Index : Leaf.Right?.Index)!;
                    NewNode = new (new ($"{Temp}", "num", Index));
                    
                }
                else if (Leaf.Left?.Category is "name" && Leaf.Right?.Category is "num" ||
                         Leaf.Left?.Category is "num" && Leaf.Right?.Category is "name")
                {
                    if ((Leaf.Value is "+" && (Leaf.Left?.Value == "0" || Leaf.Right?.Value == "0")) || 
                        Leaf is { Value: "-", Right.Value: "0" })
                    {
                        NewNode = Leaf.Left?.Value != "0" ? Leaf.Left : Leaf.Right;
                    }
                    else if (Leaf.Value is "*" or "/" or "%" && (Leaf.Left?.Value == "0" || Leaf.Right?.Value == "0"))   
                    {
                        NewNode = Leaf.Left?.Value == "0" ? Leaf.Left : Leaf.Right;
                    }
                    else if ((Leaf.Value is "*" && (Leaf.Left?.Value == "1" || Leaf.Right?.Value == "1")) || 
                             Leaf is { Value: "/", Right.Value: "1" })
                    {
                        NewNode = Leaf.Left?.Value != "1" ? Leaf.Left : Leaf.Right;
                    }
                }
                /*else if (Leaf is { Value: "-", Left: null, Right.Category: "num" })
                {
                        
                }*/

                if (NewNode != null)
                {
                    if (Tree == Leaf)
                    {
                        return NewNode;
                    }
                    Tree.Traverse(TreeNode =>
                    {
                        if (TreeNode.Left == Leaf)
                        {
                            TreeNode.Left = NewNode;
                        }
                        else if (TreeNode.Right == Leaf)
                        {
                            TreeNode.Right = NewNode;
                        }
                    });
                }
            }
            Tree.Traverse(_ => ++NewCount);
        } while (Count != NewCount);

        return Tree;
    }

    public TreeNode DivideAndConquer(TreeNode Tree)
    {
        return null;
    }
}