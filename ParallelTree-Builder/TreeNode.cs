using System.Text;
using LexSyntax_Analyzer;

namespace ParallelTree;

public class TreeNode: Tree {

    private Tree _Left { get; set; }
    private Tree _Right { get; set; }

    public Tree Left
    {
        get => _Left;
        set
        {
            if (value == null)
            {
                throw new ArgumentException("Children leaves nodes cannot be empty.", nameof(Left));
            }

            _Left = value;
        }
    }

    public Tree Right
    {
        get => _Right;
        set
        {
            if (value == null)
            {
                throw new ArgumentException("Children leaves nodes cannot be empty.", nameof(Right));
            }
            _Right = value;
        }
    }
    public int Depth { get
        {
            if (Left is TreeValue && Right is TreeValue) {
                return 0;
            }
            int LeftValue = Left is TreeValue ? 0 : ((TreeNode) Left).Depth;
            int RightValue = Right is TreeValue ? 0 : ((TreeNode)Right).Depth;
            return Math.Max(LeftValue, RightValue) + 1;
        }
    }

    public TreeNode(string Value, Tree Left, Tree Right)
    {
        this.Value = Value;
        _Left = Left;
        _Right = Right;
    }

    public override string ToString()
    {
        return Value;
        //return $"{this.GetHashCode():X8}";
    }
    

    override public void Print(StringBuilder Builder, string Indent = "", bool Last = true)
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
        Builder.Append($"\'{Value}\'\n");

        var Children = new List<Tree>();
        Children.Add(Left); 
        Children.Add(Right);

        for (int i = 0; i < Children.Count; i++)
        {
            Children[i].Print(Builder, Indent, i == Children.Count - 1);
        }
    }

    public void Traverse(Action<Tree> Func)
    {
        Func(this);
        if (Left is TreeNode)
        {
            ((TreeNode) Left).Traverse(Func);
        }

        if (Right is TreeNode)
        {
            ((TreeNode) Right).Traverse(Func);
        }
    }

}
