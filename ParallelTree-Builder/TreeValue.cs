using LexSyntax_Analyzer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelTree_Builder
{
    public class TreeValue: Tree
    {
        //public string Value { get; protected set; }
        /*private Category _Category { get; set; }

        public Category Category
        {
            get => _Category;
            set
            {
                if (value == Category.Unknown)
                {
                    throw new ArgumentException("Unknown tokens are not acceptable in expression trees.", nameof(Category));
                }

                _Category = value;
            }
        }*/
        public TreeValue(string Value, Category Category)
        {
            this.Value = Value;
            this.Category = Category;
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
