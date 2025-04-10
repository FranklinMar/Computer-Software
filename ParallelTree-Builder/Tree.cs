using LexSyntax_Analyzer;
using System.Text;
namespace ParallelTree_Builder
{
    public abstract class Tree
    {
        public string Value { get; protected set; }

        public string Print()
        {
            StringBuilder Builder = new();
            Print(Builder);
            return Builder.ToString();
        }

        abstract public void Print(StringBuilder Builder, string Indent = "", bool Last = true);
    }
}
