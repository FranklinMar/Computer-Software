using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace LexSyntax_Analyzer
{
    public class LexicalAnalyzer
    {
        private static char [] Separators = ",()+-*/%^ \t\n\r".ToCharArray();
        protected static readonly string RNum = @"^[\d]*\.?[\d]+([eE][-+][\d]+)?";
        protected static readonly string RName = @"^[A-Za-z]{1}[_A-Z0-9a-z]*";
        public List<Token> Tokens { get; private set; } = new();
        public List<SyntaxException> Errors { get; protected set; } = new();
        public string Expression { get; protected set; }


        public LexicalAnalyzer(string Expression)
        {
            this.Expression = Expression;   
            Parse();
        }

        private void Parse()
        {
            Regex RegNum = new(RNum);
            Regex RegName = new(RName);
            var CharArray = Expression.ToCharArray();
            Match NumMatch;
            Match NameMatch;
            for (int i = 0; i < CharArray.Length; i++)
            {
                NumMatch = RegNum.Match(Expression.Substring(i));
                NameMatch = RegName.Match(Expression.Substring(i));
                if (NumMatch.Success)
                {
                    if ((i + NumMatch.Length < CharArray.Length && !Separators.Contains(CharArray[i + NumMatch.Length])))
                    {
                        int Begin = i + NumMatch.Length;
                        int End = Begin;
                        while (End < CharArray.Length && !Separators.Contains(CharArray[End]))
                        {
                            End++;
                        }
                        if (Begin == End - 1)
                        {
                            Errors.Add(new SyntaxException($"Character #{i + NumMatch.Length} ('{CharArray[i + NumMatch.Length]}') is not a valid separator", i, NumMatch.Length));
                        } else {
                            Errors.Add(new SyntaxException($"Invalid token ('{Expression[i..End]}') on indexes [{i} - {End - 1}]", i, End - i));
                            Tokens.Add(new Token(Expression[i..End], "unknown", i));
                        }
                        i = End - 1;

                    } else {
                        Tokens.Add(new Token(NumMatch.Value, "num", i));
                        i = i + NumMatch.Length - 1;
                    }
                }
                else if (NameMatch.Success)
                {
                    if (i + NameMatch.Length < CharArray.Length && !Separators.Contains(CharArray[i + NameMatch.Length]))
                    {
                        int Begin = i + NameMatch.Length;
                        int End = Begin;
                        while (End < CharArray.Length && !Separators.Contains(CharArray[End]))
                        {
                            End++;
                        }
                        Errors.Add(new SyntaxException($"Invalid token ('{Expression[i..End]}') on indexes [{i} - {End - 1}]", i, End - i));
                        Tokens.Add(new Token(Expression[i..End], "unknown", i));
                        i = End - 1;
                    } else {
                        Tokens.Add(new Token(NameMatch.Value, "name", i));
                    }
                    i = i + NameMatch.Length - 1;
                } 
                else if (i < CharArray.Length)
                {
                    /*if ('(' == CharArray[i])
                    {
                        Tokens.Add(new Token(CharArray[i].ToString(), "op parentheses open", i));
                    }
                    else if (')' == CharArray[i])
                    {
                        Tokens.Add(new Token(CharArray[i].ToString(), "op parentheses close", i));
                    }*/
                    if ("()".Contains(CharArray[i]))
                    {
                        Tokens.Add(new Token(CharArray[i].ToString(), "parentheses", i));
                    }
                    else if ('^' == CharArray[i])
                    {
                        Tokens.Add(new Token(CharArray[i].ToString(), "op high power", i));
                    }
                    else if ("*%/".Contains(CharArray[i]))
                    {
                        Tokens.Add(new Token(CharArray[i].ToString(), "op high mult", i));
                    }
                    else if ("+-".Contains(CharArray[i]))
                    {
                        Tokens.Add(new Token(CharArray[i].ToString(), "op low add", i));
                    }
                    else if (',' == CharArray[i])
                    {
                        Tokens.Add(new Token(CharArray[i].ToString(), "op sep", i));
                    }
                }
            }
        }
    }
}
