using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LexSyntax_Analyzer;

namespace ParallelTree_Builder
{
    public class TreeMultiNode: Tree
    {
        public List<Tree> Values { get; protected set; }
        protected List<bool> _Signs { get; set; }
        public List<bool> Signs { 
            get
            {
                return _Signs;
            }
            protected set
            {
                if (_Signs != null && _Signs.Count != Values.Count - 1)
                {
                    throw new ArgumentException("Cannot allow independent list of values with no signs to match (Only 'Count - 1' allowed)", nameof(Signs));
                }
                _Signs = value;
            }
        }
        public TreeMultiNode(string Value, Category Category, List<Tree> Values, List<bool> Signs)
        {
            this.Value = Value;
            this.Category = Category;
            this.Values = Values;
            this.Signs = Signs;
        }
        /*public TreeMultiNode(string Value, Category Category, List<Token> Tokens)
        {
            this.Value = Value;
            this.Category = Category;
            *//*this.Values = Values;
            this.Signs = Signs;*//*
        }*/
        public void Sort()
        {
            QuickSort(Values, Signs, 0, Values.Count - 1);
        }

        public static void QuickSort(List<Tree> Values, List<bool> Signs, int Begin, int End)
        {
            if (Begin < End)
            {
                int Pivot = Partition(Values, Signs, Begin, End);
                QuickSort(Values, Signs, Begin, Pivot - 1);
                QuickSort(Values, Signs, Pivot + 1, End);
            }
        }

        //O(n)
        private static int Partition(List<Tree> Values, List<bool> Signs, int Begin, int End)
        {
            Tree Last = Values[End];
            bool LastBool = Signs[End];
            int i = Begin - 1;
            for (int j = Begin; j < End; j++)
            {
                if (Values[j].Category != Last.Category || Signs[j] != LastBool)//(Values[j] <= last)
                {
                    i++;
                    Exchange(Values, Signs, i, j);
                }
            }
            Exchange(Values, Signs, i + 1, End);
            return i + 1;
        }

        private static void Exchange(List<Tree> Values, List<bool> Signs, int i, int j)
        {
            Tree Temp = Values[i];
            Values[i] = Values[j];
            Values[j] = Temp;
            bool TempBool = Signs[i];
            Signs[i] = Signs[j];
            Signs[j] = TempBool;
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

            var Children = Values;

            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].Print(Builder, Indent, i == Children.Count - 1);
            }
        }
    }
}
