using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LexSyntax_Analyzer
{
    public class SyntaxAnalyzer: LexicalAnalyzer
    {
        //private IEnumerator<Token> TokenEnumerator;
        private Dictionary<string, string> Operations = new Dictionary<string, string>()
        {
            {RNum, "number"},
            {RName, "identifier"},
            {@"\+", "addition operator"},
            {@"\-", "subtraction operator"},
            {@"\*", "multiplication operator"},
            {@"\/", "division operator"},
            {@"\%", "mod division operator"},
            {@"\^", "power operator"},
            {@"\(", "opening bracket"},
            {@"\)", "closing bracket"}
        };
        public SyntaxAnalyzer(string Expression): base(Expression)
        {
            this.Expression = Expression;
            foreach (var Error in Errors)
            {
                continue;
            }
            //TokenEnumerator = Tokens.GetEnumerator();
            int Index = 0;
            //while (Index < Tokens.Count)
            //{
            ParseSyntax(ref Index);
            //}
        }

        private bool ParseSyntax(ref int Index)
        {
            //bool Result = false;
            bool Sign = false;
            while (Tokens[Index].Category == "unknown")
            {
                AddError(Tokens[Index], $"Unexpected '{Tokens[Index].Value}' unknown token, expected a number instead on index {Tokens[Index].Index}");
                Index++;
            }
            if ("+-".Contains(Tokens[Index].Value))
            {
                Sign = true;
                ++Index;
            }
            bool Expression = ParseExpression(ref Index);
            if (!Expression)
            {
                /*if (Sign)
                {
                    //AddError(Tokens[Index], "after operator");
                    AddError(Tokens[Index], $"Unexpected '{Tokens[Index].Value}' {"{0}"}, expected identifier or expression instead on index {Tokens[Index].Index}");
                    Index++;
                    //Index++;
                }
                else
                {
                    return false;
                }*/
                if (Sign)
                {
                    AddError(Tokens[Index], $"Unexpected '{Tokens[Index].Value}' {"{0}"}, expected identifier or expression instead on index {Tokens[Index].Index}");
                }
                return false;
            }

            while (Index < Tokens.Count)
            {
                Sign = false;
                if ("+-".Contains(Tokens[Index].Value))
                {
                    Sign = true;
                    ++Index;
                }
                Expression = ParseExpression(ref Index);
                if (!Expression)
                {
                    if (!Sign)
                    {
                        break;
                    }
                    else
                    {
                        //AddError(Tokens[Index], "after operator");
                        AddError(Tokens[Index], $"Unexpected '{Tokens[Index].Value}' {"{0}"} after operator on index {Tokens[Index].Index}");
                    }
                }/* else if (Tokens[Index].Category == "bracket close")
                {
                    AddError(Tokens[Index], $"Unexpected '{Tokens[Index].Value}' {"{0}"} on index {Tokens[Index].Index}");
                }*/
            }
            return true;
        }

        private bool ParseExpression(ref int Index)
        {
            bool Expression = ParseValueOrBr(ref Index);
            if (!Expression)
            {
                return false;
            }
            while (Index < Tokens.Count)
            {
                bool Operator = false;
                while (Tokens[Index].Category == "unknown")
                {
                    AddError(Tokens[Index], $"Unexpected '{Tokens[Index].Value}' unknown token, expected a number instead on index {Tokens[Index].Index}");
                    Index++;
                }
                if ("*%/^".Contains(Tokens[Index].Value))
                {
                    Operator = true;
                    ++Index;
                } else 
                Expression = ParseValueOrBr(ref Index);
                if (!Expression)
                {
                    if (!Operator)
                    {
                        break;
                    } else
                    {
                        //AddError(Tokens[Index], "after operator");
                        AddError(Tokens[Index], $"Unexpected '{Tokens[Index].Value}' {"{0}"} after operator on index {Tokens[Index].Index}");
                    }
                    //++Index;
                    //Result = StartIndex != Index || Result;
                }/* else if (Tokens[Index].Category == "bracket close")
                {
                    AddError(Tokens[Index], $"Unexpected '{Tokens[Index].Value}' {"{0}"} on index {Tokens[Index].Index}");
                }*/ 
                //else if (Operator)
                //{
                //    //AddError(Tokens[Index], "after operator");
                //    Result = false || Result;
                //    break;
                //} else
                //{
                //    break;
                //}
            }
            /*while (TokenEnumerator.MoveNext())
            {
                Token Token = TokenEnumerator.Current;
                bool Operator = "*%/".Contains(Token.Value);
                if (Operator && !ParseExpression())
                {

                }
            }*/
            return true;
        }

        /*private bool ParsePower(ref int Index)
        {
            if (!ParseBrackets(ref Index))
            {
                return false;
            }
            while (Index < Tokens.Count)
            {
                //Token Token = TokenEnumerator.Current;
                bool Operator = "*%/".Contains(Tokens[Index].Value);
                ++Index;
                if (Operator && !ParsePower(ref Index))
                {
                    AddError(Tokens[Index], "after operator");
                    ++Index;
                }
                else if (Operator)
                {
                    return true;
                }
            }
            return false;
        }*/

        private bool ParseValueOrBr(ref int Index)
        {
            bool Result = false;
            try
            {
                if (Tokens[Index].Category == "bracket open")
                {
                    Index++;
                    Result = ParseSyntax(ref Index);
                    if (Tokens[Index].Category == "bracket close" && Result)
                    {
                        Result = true;
                        Index++;
                    }
                    else
                    {
                        AddError(Tokens[Index], $"Expected {Operations["\\)"]} instead of {"{0}"} token '{Tokens[Index].Value}' on index {Tokens[Index].Index}");
                        Index++;
                        //AddError(Tokens[Index], "instead of closing bracket");
                        //Result = false;
                        //THROW ERROR
                    }
                }
                else if (Tokens[Index].Category == "name")
                {
                    Index++;
                    if(Tokens[Index].Category == "bracket open")
                    {
                        int BracketIndex = Index;
                        Index++;
                        Result = ParseSyntax(ref Index);
                        if (Result && Tokens[Index].Category == "op sep")
                        {
                            while (Result && Tokens[Index].Category == "op sep")
                            {
                                Index++;
                                Result = ParseSyntax(ref Index);
                            }
                            if (Tokens[Index].Category != "bracket close")
                            {
                                AddError(Tokens[Index], $"Expected {Operations["\\)"]} instead of {"{0}"} token '{Tokens[Index].Value}' on index {Tokens[Index].Index}");
                                Index++;
                                //AddError(Tokens[Index], $", expected {Operations[")"]} instead");
                                //Result = false;
                                //THROW ERROR
                            } else
                            {
                                Index++;
                                Result = true;
                            }
                        } else if (Tokens[Index].Category != "bracket close")
                        {
                            AddError(Tokens[Index], $"Expected {Operations["\\)"]} instead of {"{0}"} token '{Tokens[Index].Value}' on index {Tokens[Index].Index}");
                            Index++;
                            //AddError(Tokens[Index], $", expected {Operations[")"]} instead");
                            //Result = false;
                            //THROW ERROR
                        } else
                        {
                            AddError(Tokens[Index], $"Redundant {"{0}"} token '{Tokens[Index].Value}' on index {Tokens[Index].Index}");
                            Index++;
                            //AddError(Tokens[BracketIndex], "reduntant");
                            //Result = false;
                            //THROW ERROR
                        }
                    } else
                    {
                        Result = true;
                    }
                }
                else if (Tokens[Index].Category == "num")
                {
                    Index++;
                    Result = true;
                } else if (Tokens[Index].Category == "unknown")
                {
                    AddError(Tokens[Index], $"Unexpected unknown token '{Tokens[Index].Value}' on index {Tokens[Index].Index} found");
                    Index++;
                    Result = ParseValueOrBr(ref Index);
                } /*else
                {
                    Index--;
                    if (Tokens[Index].Category == "num" || Tokens[Index].Category == "name")
                    {
                        Result = true;
                    }
                }*/
            }
            catch (ArgumentOutOfRangeException Exception)
            {
                //AddError(Tokens[Index], $"Expected {Operations[")"]} instead of {"{0}"} token '{Tokens[Index].Value}' on index {Tokens[Index].Index}");
                AddError(Tokens[Tokens.Count - 1], $"Expected {Operations["\\)"]} on closing of expression on index {Tokens[Tokens.Count - 1].Index + 1}");

                //AddError(Tokens[Index], $", expected {Operations[")"]} instead");
                //return false;
            }
            //else
            //{
            //AddError(Tokens[Index], "expected value, id or");
            //Result = false;
            //THROW ERROR
            //}
            return Result;
        }

        private void AddError(Token Token, string Reason = "{0}")
        {
            var Keys = Operations.Keys.ToArray();
            string Found = "";
            for (int i = 0; i < Keys.Length && Found == ""; i++)
            {
                if (Regex.IsMatch(Token.Value, Keys[i]))
                {
                    Found = Operations[Keys[i]];
                }
            }
            if (Found == "")
            {
                Found = "unknown";
            }

            //{ "unknown", "Unknown token"}
            /*if (Token.Category == "unknown")
            {
                Errors.Add(new FormatException($"Unexpected '{Token.Value}' {Token.Category} token at {Token.Index}"));
            }*/
            //var Results = from result in Operations
            //where Regex.Match(Token.Value, result.Key, RegexOptions.Singleline).Success
            //select result.Value;
            //var Result = Operations[Token.Value];
            //Errors.Add(new FormatException($"Unexpected '{Token.Value}' {Result/*Results.First()*/} {Reason} on index {Token.Index}"));
            Errors.Add(new FormatException(String.Format(Reason, Found)));

        }
    }
}
