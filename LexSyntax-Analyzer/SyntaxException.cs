using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexSyntax_Analyzer
{
    public class SyntaxException: Exception
    {
        //public Token Token { get; private set; }
        public int Index { get; private set; }
        public int Length { get; private set; }
        public SyntaxException(string Message, int Index, int Length): base(Message) {
            //this.Token = Token;
            this.Index = Index;
            this.Length = Length;
        }
    }
}
