using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexSyntax_Analyzer
{
    public class Token(string Value, string Category, int Index)
    {
        public string Value { get; private set; } = Value;

        public string Category { get; private set; } = Category;

        private int _Index { get; set; } = Index;

        public int Index
        {
            get => _Index;
            private set {
                if (value < 0)
                {
                    throw new IndexOutOfRangeException("Negative index is out of range.");
                }
                _Index = value;
            }
        }

        public bool IsOp => Category.StartsWith("op");
    }
}
