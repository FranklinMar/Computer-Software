using LexSyntax_Analyzer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelTree_Builder
{
    public abstract class Tree
    {
        public string Value { get; protected set; }
        private Category _Category;
        public Category Category {
            get => _Category;
            protected set {
                    if (value == Category.Unknown)
                    {
                        throw new ArgumentException("Unknown tokens are not acceptable in expression trees.", nameof(Category));
                    }
                    _Category = value;
                }
            }
        public bool IsOp => Category.HasFlag(Category.Op);

        public string Print()
        {
            StringBuilder Builder = new();
            Print(Builder);
            return Builder.ToString();
        }

        abstract public void Print(StringBuilder Builder, string Indent = "", bool Last = true);
    }
}
