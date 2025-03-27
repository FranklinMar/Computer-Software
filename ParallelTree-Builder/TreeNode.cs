using System.Text;
using LexSyntax_Analyzer;

namespace ParallelTree_Builder;

public class TreeNode: Tree {

    private Tree _Left { get; set; }
    private Tree _Right { get; set; }

    public Tree Left
    {
        get => _Left;
        set
        {
            if (Category.HasFlag(Category.Object) && value != null)
            {
                throw new ArgumentException("Value nodes cannot have children.", nameof(Left));
            }

            _Left = value;
        }
    }

    public Tree Right
    {
        get => _Right;
        set
        {
            if (Category.HasFlag(Category.Object) && value != null)
            {
                throw new ArgumentException("Value nodes cannot have children.", nameof(Right));
            }
            _Right = value;
        }
    }

    public TreeNode(string Value, Category Category, Tree Left, Tree Right)
    {
        this.Value = Value;
        this.Category = Category;
        _Left = Left;
        _Right = Right;
    }

    public override string ToString()
    {
        return Value;
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
