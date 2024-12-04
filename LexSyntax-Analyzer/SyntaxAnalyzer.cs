using System;
using System.Collections.Generic;
using System.Data;
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
            {"+", "addition"},
            {"-", "subtraction"},
            {"*", "multiplication"},
            {"/", "division"},
            {"%", "mod division"},
            {"^", "power"},
            {"(", "opening bracket"},
            {")", "closing bracket"},
            {"unknown", "Unknown token"}
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
            ParseSyntax(ref Index);
        }

        private bool ParseSyntax(ref int Index)
        {
            //try
            //{
            /*if ("+-".Contains(Tokens[Index].Value))
            {
                ++Index;
            }*/
            /*while (TokenEnumerator.MoveNext())
            {
                Token Token = TokenEnumerator.Current;
                bool Sign = "+-".Contains(Token.Value);
                if (Sign && !ParseExpression())
                {
                    AddError(Token);
                } else if (Sign)
                {
                    return true;
                }
            }*/
            int StartIndex = Index;
            bool Result = false;
            do
            {
                try { 
                    bool Sign = false;
                    if ("+-".Contains(Tokens[Index].Value))
                    {
                        Sign = true;
                        ++Index;
                    }
                    //int NewIndex = Index + 1;
                    bool Expression = ParseExpression(ref Index);
                    if (Sign && Expression)
                    {
                        //Sign = "+-".Contains(Tokens[Index].Value);
                        //if (Sign)
                        //{
                        ++Index;
                        Result = StartIndex != Index || Result;
                        //}
                    }
                    else if (Sign)
                    {
                        AddError(Tokens[Index], "after operator");
                        Result = false || Result;
                        break;
                    } else
                    {
                        break;
                    }
                } catch (Exception Exception)
                {

                }
                //++Index;
                /*bool Sign = "+-".Contains(Tokens[Index].Value);
                if (Sign && !ParseExpression(Index + 1))
                {
                    AddError(Tokens[Index + 1], "after operator");
                }
                else if (Sign)
                {
                    ++Index;
                }
                else
                {
                    Index += 2;
                }*/
            }
            while (Index < Tokens.Count - 1);

            //int AddIndex = "+-".Contains(Tokens[Index].Value) ? Index + 1 : Index;
            /*if (Tokens[AddIndex].Category == "number" || Tokens[AddIndex].Category == "name" )
            {
                ++AddIndex;
                if (Tokens[AddIndex].Category.StartsWith("op"))
                {
                    while (Tokens[AddIndex].Category == "op add")
                    {
                        ++AddIndex;
                        if (Tokens[AddIndex].Category != "number" && Tokens[AddIndex].Category != "name" && !Tokens[AddIndex].Category.StartsWith("bracket"))
                        {
                            AddError(Tokens[AddIndex], "after operation");
                        }
                    }
                }
                else
                {
                    AddError(Tokens[AddIndex], "after operation");
                }
            }*/


            /*else if (Tokens[AddIndex].Category.StartsWith("bracket"))
            {
                // var li = +(+345 - 4-(-345));
            }
            else
            {

                return false;
            }*/
            //} finally
            //{

            //}
            /*catch(IndexOutOfRangeException Exception)
            {
                return true;
            }*/
            return Result;
        }

        private bool ParseExpression(ref int Index)
        {
            bool Result = false;
            //if (!ParsePower(ref Index))
            if (!ParseValueOrBr(ref Index))
            {
                return Result;
            } else
            {
                Result = true;
            }
            int StartIndex = Index;
            while (Index < Tokens.Count)
            {
                //Token Token = TokenEnumerator.Current;
                bool Operator = false;
                if ("*%/^".Contains(Tokens[Index].Value))
                {
                    Operator = true;
                    ++Index;
                }
                bool Expression = ParseValueOrBr(ref Index);
                if (Operator && Expression)//!ParsePower(ref Index))
                {
                    ++Index;
                    Result = StartIndex != Index || Result;
                }
                else if (Operator)
                {
                    //AddError(Tokens[Index], "after operator");
                    Result = false || Result;
                    break;
                } else
                {
                    break;
                }
            }
            /*while (TokenEnumerator.MoveNext())
            {
                Token Token = TokenEnumerator.Current;
                bool Operator = "*%/".Contains(Token.Value);
                if (Operator && !ParseExpression())
                {

                }
            }*/
            return Result;
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
            int StartIndex = Index;
            try
            {
                bool Result;
                if (Tokens[Index].Category == "bracket open")
                {
                    Index++;
                    Result = ParseSyntax(ref Index);
                    if (Tokens[Index].Category == "bracket close" && Result)
                    {
                        Result = true;
                    }
                    else
                    {
                        AddError(Tokens[Index], "instead of closing bracket");
                        Result = false;
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
                                AddError(Tokens[Index], $", expected {Operations[")"]} instead");
                                Result = false;
                                //THROW ERROR
                            } 
                        } else if (Tokens[Index].Category != "bracket close")
                        {
                            AddError(Tokens[Index], $", expected {Operations[")"]} instead");
                            Result = false;
                            //THROW ERROR
                        } else
                        {
                            AddError(Tokens[BracketIndex], "reduntant");
                            Result = false;
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
                }
                else
                {
                    //AddError(Tokens[Index], "expected value, id or");
                    Result = false;
                    //THROW ERROR
                }
                return Result;
            }
            catch (Exception Exception)
            {
                return false;
            }
        }

        private void AddError(Token Token, string Reason = "")
        {
            /*if (Token.Category == "unknown")
            {
                Errors.Add(new FormatException($"Unexpected '{Token.Value}' {Token.Category} token at {Token.Index}"));
            }*/
            //var Results = from result in Operations
            //where Regex.Match(Token.Value, result.Key, RegexOptions.Singleline).Success
            //select result.Value;
            var Result = Operations[Token.Value];
            Errors.Add(new FormatException($"Unexpected '{Token.Value}' {Result/*Results.First()*/} {Reason} on index {Token.Index}"));

        }
    }
}
