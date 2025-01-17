using LexSyntax_Analyzer;

namespace ParallelTree_Builder;

public class TreeMultinode
{
    private Token _Token { get; set; }
    public Token Token
    {
        get => _Token;
        private set
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

    public List<TreeNode> Children { get; private set; } = new();

    public TreeMultinode(Token Token, List<TreeNode> Children)
    {
        this.Token = Token;
        this.Children = Children;
    }
}
    