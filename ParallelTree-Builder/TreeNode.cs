using System.Text;
using LexSyntax_Analyzer;

namespace ParallelTree_Builder;

public class TreeNode
{
    private Token _Token { get; init; }

    private TreeNode? _Left { get; set; }
    private TreeNode? _Right { get; set; }
    public Token Token
    {
        get => _Token;
        private init
        {
            if (value.Category == "unknown")
            {
                throw new ArgumentException("Unknown tokens are not acceptable in expression trees.", nameof(Value));
            }
            _Token = value;
        }
    }
    public string Value => Token.Value;
    public string Category => Token.Category;
    
    public int Index => Token.Index;

    public TreeNode? Left
    {
        get => _Left;
        set
        {
            if (_Token.Category is "name" or "num" && value != null)
            {
                throw new ArgumentException("Value nodes cannot have children.", nameof(Left));
            }

            if (value != null && _Right == null)
            {
                throw new ArgumentException("Lone value in binary operator", nameof(Left));
            }
            /*if (_Token.Category is not "name" or "num" && value !=null)
            {
                Queue<TreeNode> Queue = new();
                Queue.Enqueue(value);
                TreeNode Node;
                while (Queue.Count != 0)
                {
                    Node = Queue.Dequeue();
                    if (Node.Left != null)
                    {
                        Queue.Enqueue(Node.Left);
                    }
                    else if (Node.Token.IsOp && Node.Right == null)
                    {
                        throw new ArgumentException("Empty operator without operands.", nameof(Left));
                    }
                    else if (Node.Value != "-" && Node.Right != null)
                    {
                        throw new ArgumentException("Non-unary operator without second operand.", nameof(Left));
                    }

                    if (Node.Right != null)
                    {
                        Queue.Enqueue(Node.Right);
                    }
                }
            }*/

            _Left = value;
        }
    }

    public TreeNode? Right
    {
        get => _Right;
        set
        {
            if (_Token.Category is "name" or "num" && value != null)
            {
                throw new ArgumentException("Value nodes cannot have children.", nameof(Right));
            }

            if (value != null && _Left == null)
            {
                throw new ArgumentException("Lone value in binary operator", nameof(Right));
            }
            /*if (_Token.Category is not "name" or "num" && value !=null)
            {
                Queue<TreeNode> Queue = new();
                Queue.Enqueue(value);
                TreeNode Node;
                while (Queue.Count != 0)
                {
                    Node = Queue.Dequeue();
                    if (Node.Left != null)
                    {
                        Queue.Enqueue(Node.Left);
                    }
                    else if (Node.Token.IsOp && Node.Right == null)
                    {
                        throw new ArgumentException("Empty operator without operands.", nameof(Left));
                    }
                    else if (Node.Value != "-" && Node.Right != null)
                    {
                        throw new ArgumentException("Non-unary operator without second operand.", nameof(Left));
                    }

                    if (Node.Right != null)
                    {
                        Queue.Enqueue(Node.Right);
                    }
                }
            }*/
            _Right = value;
        }
    }


    public TreeNode(Token Token)
    {
        this.Token = Token;
        _Left = _Right = null;
    }

    /*public TreeNode(Token Token, TreeNode? Right)
    {
        this.Token = Token;
        Left = null;
        this.Right = Right;
    }*/

    public TreeNode(Token Token, TreeNode? Left, TreeNode? Right)
    {
        this.Token = Token;
        if (Token.Category is "name" or "num")
        {
            if (Left != null)
            {
                throw new ArgumentException("Value nodes cannot have children.", nameof(Left));
            }
            if (Right != null)
            {
                throw new ArgumentException("Value nodes cannot have children.", nameof(Right));
            }
        }
        _Left = Left;
        _Right = Right;
        if (this.Token.IsOp)
        {
            if (Left == null)
            {
                throw new ArgumentException("Lone value in binary operator", nameof(Right));
            }
            if (Right == null)
            {
                throw new ArgumentException("Lone value in binary operator", nameof(Right));
            }
        }
    }

    /*private string PrintTree(StringBuilder Builder, string Prefix)
    {
        string Current;
        Builder.Append(Prefix + Value.Value);
        if (Left != null && Right != null)
        {
            //Left.PrintTree(Prefix + "| ")
        }
    }*/

    public override string ToString()
    {
        StringBuilder Builder = new();
        Print(Builder);
        return Builder.ToString();
    }

    public void Print(StringBuilder Builder, string Indent = "", bool Last = true)
    {
        Builder.Append(Indent);
        if (Last)
        {
            Builder.Append("└─");
            Indent += "  ";
        }
        else
        {
            Builder.Append("├─");
            Indent += "| ";
        }
        Builder.Append($" \'{Value}\'\n");

        var Children = new List<TreeNode>();
        if (Left != null)
        {
            Children.Add(Left); 
        }

        if (Right != null)
        {
            Children.Add(Right);
        }

        for (int i = 0; i < Children.Count; i++)
        {
            Children[i].Print(Builder, Indent, i == Children.Count - 1);
        }
    }

    public void Traverse(Action<TreeNode> Func)
    {
        Func(this);
        if (Left != null)
        {
            Left.Traverse(Func);
        }

        if (Right != null)
        {
            Right.Traverse(Func);
        }
    }
}
