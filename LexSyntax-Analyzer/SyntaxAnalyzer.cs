/*using System;
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
            {@"\(", "opening parentheses"},
            {@"\)", "closing parentheses"}
        };
        public SyntaxAnalyzer(string Expression): base(Expression)
        {
            this.Expression = Expression;
            foreach (var Error in Errors)
            {
                continue;
            }
            int Index = 0;
            int OpenParentheses = 0;
            ParseSyntax(ref Index, ref OpenParentheses);
        }

        private bool ParseSyntax(ref int Index, ref int OpenParentheses)
        {
            int SignIndex = Index;
            bool Sign = false;
            while (Tokens[Index].Category == "unknown")
            {
                AddError(Tokens[Index], $"Unexpected '{Tokens[Index].Value}' unknown token, expected a number instead on index {Tokens[Index].Index}");
                Index++;
            }
            try {
                if (Tokens[Index].Category.StartsWith("op low"))
                {
                    Sign = true;
                    ++Index;
                }
                bool Expression = ParseExpression(ref Index, ref OpenParentheses);
                if (!Expression)
                {
                    if (Sign)
                    {
                        AddError(Tokens[Index], $"Unexpected '{Tokens[Index].Value}' {"{0}"}, expected identifier or expression instead on index {Tokens[Index].Index}");
                        ++Index;
                    }
                    return false;
                }

                while (Index < Tokens.Count)
                {
                    Sign = false;
                    SignIndex = Index;
                    while (Tokens[Index].Category == "unknown")
                    {
                        AddError(Tokens[Index], $"Unexpected unknown token '{Tokens[Index].Value}' on index {Tokens[Index].Index} found");
                        ++Index;
                    }
                    if (Tokens[Index].Category.StartsWith("op low"))
                    {
                        Sign = true;
                        ++Index;
                    }
                    Expression = ParseExpression(ref Index, ref OpenParentheses);
                    if (!Expression)
                    {
                        if (Sign)
                        {
                            AddError(Tokens[Index], $"Unexpected '{Tokens[Index].Value}' {"{0}"} after operator on index {Tokens[Index].Index}");
                            ++Index;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            catch (ArgumentOutOfRangeException Exception)
            {
                if (Sign)
                {
                    AddError(Tokens[Tokens.Count - 1], $"Redundant or incomplete {"{0}"} on index {Tokens[Tokens.Count - 1].Index}");
                }
            }

            return true;
        }

        private bool ParseExpression(ref int Index, ref int OpenParentheses)
        {
            while (Tokens[Index].Category == "unknown")
            {
                AddError(Tokens[Index], $"Unexpected unknown token '{Tokens[Index].Value}' on index {Tokens[Index].Index} found");
                ++Index;
            }
            bool Expression = ParseIds(ref Index, ref OpenParentheses);
            if (!Expression)
            {
                return false;
            }
            while (Index < Tokens.Count)
            {
                int OperatorIndex = Index;
                bool Operator = false;
                try
                {

                    while (Tokens[Index].Category == "unknown")
                    {
                        AddError(Tokens[Index], $"Unexpected unknown token '{Tokens[Index].Value}' on index {Tokens[Index].Index} found");
                        ++Index;
                    }
                    
                    if (Tokens[Index].Category.StartsWith("op high"))
                    {
                        Operator = true;
                        ++Index;
                    }
                    else if (Tokens[Index].Value == ")" && OpenParentheses == 0)
                    {
                        AddError(Tokens[Index], $"Unexpected {Operations["\\)"]} on index {Index}");
                        ++Index;
                        continue;
                    }
                    
                    Expression = ParseIds(ref Index, ref OpenParentheses);
                    if (!Expression)
                    {
                        if (!Operator)
                        {
                            break;
                        }
                        else
                        {
                            AddError(Tokens[Index], $"Unexpected '{Tokens[Index].Value}' {"{0}"} after operator on index {Tokens[Index].Index}");
                            ++Index;
                        }
                    }
                }  catch (ArgumentOutOfRangeException Exception)
                {
                    if (Operator)
                    {
                        AddError(Tokens[Tokens.Count - 1], $"Redundant or incomplete {"{0}"} on index {Tokens[Tokens.Count - 1].Index}");
                    }
                }
            }
            return true;
        }

        private bool ParseIds(ref int Index, ref int OpenParentheses)
        {
            bool Result = false;
            while (Tokens[Index].Category == "unknown")
            {
                AddError(Tokens[Index], $"Unexpected unknown token '{Tokens[Index].Value}' on index {Tokens[Index].Index} found");
                ++Index;
            }
            if (Tokens[Index].Category == "op parentheses")
            {
                if (Tokens[Index].Value == "(")
                {
                    ++OpenParentheses;
                    int ParenthesesIndex = Index;
                    try
                    {
                        ++Index;
                        Result = ParseSyntax(ref Index, ref OpenParentheses);
                        if (Tokens[Index].Value == ")")
                        {
                            --OpenParentheses;
                            if (Result)
                            {
                                ++Index;
                            }
                        }
                    }
                    catch (ArgumentOutOfRangeException Exception)
                    {
                        AddError(Tokens[Tokens.Count - 1], $"Expected {Operations["\\)"]} on closing of expression on index {Tokens[Tokens.Count - 1].Index + 1} to match opening parentheses on index {Tokens[ParenthesesIndex].Index}");
                    }
                }
            }
            else if (Tokens[Index].Category == "name")
            {
                ++Index;
                if(Index < Tokens.Count && Tokens[Index].Value == "(")
                {
                    ++OpenParentheses;
                    int ParenthesesIndex = Index;
                    try
                    {
                        ++Index;
                        Result = ParseSyntax(ref Index, ref OpenParentheses);
                        if (Result && Tokens[Index].Category == "op sep")
                        {
                            while (Result && Tokens[Index].Category == "op sep" && Index < Tokens.Count)
                            {
                                ++Index;
                                Result = ParseSyntax(ref Index, ref OpenParentheses);
                            }
                            if (Tokens[Index].Value != ")")
                            {
                                AddError(Tokens[Index], $"Incomplete {"{0}"} token '{Tokens[Index].Value}' instead of {Operations["\\)"]} on index {Tokens[Index].Index}");

                                ++Index;
                            }
                            else
                            {
                                --OpenParentheses;
                                ++Index;
                                Result = true;
                            }
                        }
                        else if (Tokens[Index].Value != ")")
                        {
                            AddError(Tokens[Index], $"Unexpected {"{0}"} token '{Tokens[Index].Value}' instead of {Operations["\\)"]} on index {Tokens[Index].Index}");
                            ++Index;
                        }
                        else
                        {
                            AddError(Tokens[Index], $"Redundant {"{0}"} token '{Tokens[Index].Value}' on index {Tokens[Index].Index}");
                            ++Index;
                        }
                    }
                    catch (ArgumentOutOfRangeException Exception)
                    {
                        AddError(Tokens[Tokens.Count - 1], $"Expected {Operations["\\)"]} on closing of expression on index {Tokens[Tokens.Count - 1].Index + 1} to match opening parentheses on index {Tokens[ParenthesesIndex].Index}");
                    }
                } else
                {
                    Result = true;
                }
            }
            else
            {
                Result = ParseValue(ref Index);
            }
            return Result;
        }

        private bool ParseValue(ref int Index)
        {
            bool Result = false;
            bool Unknown = false;
            while (Tokens[Index].Category == "unknown")
            {
                AddError(Tokens[Index], $"Unexpected unknown token '{Tokens[Index].Value}' on index {Tokens[Index].Index} found");
                ++Index;
                Unknown = true;
            }
            if (Tokens[Index].Category == "num")
            {
                ++Index;
                Result = true;
            } else if (Unknown)
            {
                Result = true;
            }
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
            Errors.Add(new SyntaxException(string.Format(Reason, Found), Token.Index, Token.Value.Length));
        }
    }
}
*/