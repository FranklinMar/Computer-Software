using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexSyntax_Analyzer
{
    public class Token
    {
        public string Value { get; private set; }
        public string Category { get; private set; }
        public int Index { get; private set; }
        public Token(string Value, string Category, int Index) { 
            this.Value = Value;
            this.Category = Category;
            this.Index = Index;
        }
    }
}
