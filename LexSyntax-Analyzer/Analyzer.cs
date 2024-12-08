using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexSyntax_Analyzer
{
    public partial class Analyzer
    {
        private static readonly Dictionary<char, uint> TokensPriorities = new()

        {
            { '-', 0 },
            { '+', 0 },
            { '*', 1 },
            { '/', 1 },
            { '%', 1 },
            { '(', 2 },
            { ')', 2 },
            { '^', 2 }
        };
        //private static readonly List<int> FinalStates = new() {  };
        //private int State = 0;

        public Analyzer(string Expression)
        {

        }

        private void Parse(string Expression)
        {

        }
    }
}
