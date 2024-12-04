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
        protected static readonly string RName = @"^[A-Za-z]{1}[_A-Z0-9a-z]+";
        //protected readonly Regex RegNum = new Regex(@"^[-]?[\d]*\.?[\d]+([eE][-+][\d]+)?");
        //protected readonly Regex RegName = new Regex(@"^[A-Za-z]{1}[_A-Z0-9a-z]+\(?");
        public List<Token> Tokens { get; private set; } = new();
        public List<Exception> Errors { get; protected set; } = new();
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
                //Debug.WriteLine($"{i}: {Match}");
                //Debug.WriteLine($"{i}: {CharArray[i]}");
                if (NumMatch.Success)
                {
                    //Debug.WriteLine($"NUMBER {i}: {NumMatch}");
                    if ((i + NumMatch.Length < CharArray.Length && !Separators.Contains(CharArray[i + NumMatch.Length])))
                    {
                        int Begin = i + NumMatch.Length;
                        int End = Begin;
                        while (!Separators.Contains(CharArray[End]))
                        {
                            End++;
                        }
                        if (Begin == End - 1)
                        {
                            Errors.Add(new FormatException($"Character #{i + NumMatch.Length} ('{CharArray[i + NumMatch.Length]}') is not a valid separator"));
                        } else {
                            Errors.Add(new FormatException($"Invalid token ('{Expression[i..End]}') on indexes [{i} - {End - 1}]"));
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
                    //Debug.WriteLine($"NAME {i}: {NameMatch}");
                    if (i + NameMatch.Length < CharArray.Length && !Separators.Contains(CharArray[i + NameMatch.Length]))
                    {
                        int Begin = i + NameMatch.Length;
                        int End = Begin;
                        while (!Separators.Contains(CharArray[End]))
                        {
                            End++;
                        }
                        /*if (Begin == End - 1)
                        {
                            Errors.Add(new FormatException($"Character #{i + NameMatch.Length} ('{CharArray[i + NameMatch.Length]}') is not a valid separator"));
                        }
                        else if (Begin == End - 1) {
                        {
                            Errors.Add(new FormatException($"Invalid token ('{Expression[i..End]}') on indexes [{i} - {End - 1}]"));
                            Tokens.Add(new Token(Expression[i..End], "unknown", i));
                        }*/
                        Errors.Add(new FormatException($"Invalid token ('{Expression[i..End]}') on indexes [{i} - {End - 1}]"));
                        Tokens.Add(new Token(Expression[i..End], "unknown", i));
                        i = End - 1;
                        //Errors.Add(new FormatException($"Character #{i + NameMatch.Length} ('{CharArray[i + NameMatch.Length]}') is not a valid separator"));
                    } else {
                        Tokens.Add(new Token(NameMatch.Value, "name", i));
                    }
                    i = i + NameMatch.Length - 1;
                } 
                else if (i < CharArray.Length)
                {
                    if ('(' == CharArray[i])
                    {
                        Tokens.Add(new Token(CharArray[i].ToString(), "bracket open", i));
                    }
                    else if (')' == CharArray[i])
                    {
                        Tokens.Add(new Token(CharArray[i].ToString(), "bracket close", i));
                    }
                    else if ('^' == CharArray[i])
                    {
                        Tokens.Add(new Token(CharArray[i].ToString(), "op power", i));
                    }
                    else if ("*%/".Contains(CharArray[i]))
                    {
                        Tokens.Add(new Token(CharArray[i].ToString(), "op mult", i));
                    }
                    else if ("+-".Contains(CharArray[i]))
                    {
                        Tokens.Add(new Token(CharArray[i].ToString(), "op add", i));
                    }
                    else if (',' == CharArray[i])
                    {
                        Tokens.Add(new Token(CharArray[i].ToString(), "op sep", i));
                    }
                }

                Debug.WriteLine($"Token {Tokens[Tokens.Count - 1].Category}: '{Tokens[Tokens.Count - 1].Value}'");
            }
        }
    }
}
