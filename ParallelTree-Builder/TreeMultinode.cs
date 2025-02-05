using LexSyntax_Analyzer;

namespace ParallelTree_Builder;

public class TreeMultinode {
    // private string _Value { get; set ; }
    public string Value { get; private set ; }
    private string _Category { get; set; }
    /*public string Value
    {
        get => _Value;
        set
        {
            if ("%^".Contains(value))
            {
                throw new ArgumentException($"High priority operation {value} is not supported.");
            }
        }
    }*/

    public string Category
    {
        get => _Category;
        set
        {
            if (value == "unknown")
            {
                throw new ArgumentException("Unknown tokens are not acceptable in expression trees.", nameof(Category));
            }

            _Category = value;
        }
    }
    public bool IsOp => Category.StartsWith("op");
    public List<TreeNode> Children { get; } = new();

    public TreeMultinode(string Value, string Category)
    {
        this.Value = Value;
        this.Category = Category;
    }

    public TreeMultinode(string Value, string Category, List<TreeNode> Children)
    {
        this.Value = Value;
        this.Category = Category;
        this.Children = Children;
    }

    public TreeMultinode(string Value, string Category, params TreeNode[] Nodes)
    {
        this.Value = Value;
        this.Category = Category;
        Children.AddRange(Nodes);
    }
}
    