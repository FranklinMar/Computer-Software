using LexSyntax_Analyzer;
using System.Text;

namespace ParallelTree
{
    public class TreeValue: Tree
    {
        public TreeValue(string Value)
        {
            this.Value = Value;
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
        }
    }
}
