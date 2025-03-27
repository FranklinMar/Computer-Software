using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexSyntax_Analyzer
{
    [Flags]
    public enum Category
    {
        Unknown = 0,      // 000000
        Object = 1,       // 000001
        Number = 3,       // 000011
        Name = 5,         // 000101
        Op = 8,           // 001000
        Separator = 24,   // 011000
        Parenthesis = 32, // 100000
    }

    public class Token(string Value,/* string*/Category Category, int Index)
    {
        public string Value { get; private set; } = Value;

        //public string Category { get; } = Category;

        public Category Category { get; } = Category;

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

        public bool IsOp => Category.HasFlag(Category.Op);//Category.StartsWith("op");

        override public string ToString() => Value;
    }
}
