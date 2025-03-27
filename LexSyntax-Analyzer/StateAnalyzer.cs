using System.Net;
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
    public class StateAnalyzer: LexicalAnalyzer
    {
        private enum State {
          Begin,
          Object,
          Name,
          Num,
          Op
        }
        private State CurrentState;
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
            {@"\(", "opening parenthesis"},
            {@"\)", "closing parenthesis"}
        };

        public StateAnalyzer(string Expression): base(Expression)
        {
            CurrentState = State.Begin;
            this.Expression = Expression;
            int Index = 0;
            int OpenParentheses = 0;
            while (Index < Tokens.Count) {
                if (Tokens[Index].IsOp) {
                    if (CurrentState == State.Begin) {
                        if (Tokens[Index].Value == "-") { // Tokens[Index].Category.StartsWith("op low")
                            Token Phantom = new Token("0", Category.Number, Tokens[Index].Index);
                            Tokens.Insert(Index, Phantom);
                            ++Index;
                            CurrentState = State.Num;
                        } else {
                            Errors.Add(new SyntaxException($"Unexpected {GetName(Tokens[Index])} on index {Tokens[Index].Index} on beginning of the expression.\n" +
                                "Expression should start with a Number, Identifier, Function or '('", Tokens[Index].Index, Tokens[Index].Value.Length));
                            // AddError(Tokens[Index]); // 0
                        }
                    } else if (CurrentState == State.Op) {
                        Errors.Add(new SyntaxException($"Unexpected {GetName(Tokens[Index])} on index {Tokens[Index].Index}.\nMore than one operator is not allowed", Tokens[Index].Index, Tokens[Index].Value.Length));
                        // AddError(Tokens[Index]); // 4
                    } else {
                      CurrentState = State.Op;
                    }
                } else if (Tokens[Index].Value == "(") {
                    if (CurrentState == State.Object || CurrentState == State.Name) {
                        Errors.Add(new SyntaxException($"Unexpected {GetName(Tokens[Index])} on index {Tokens[Index].Index} not allowed, only as expression's or functions opening", Tokens[Index].Index, Tokens[Index].Value.Length));
                        // AddError(Tokens[Index]); // 6
                    } else {
                        CurrentState = State.Begin;
                    }
                    OpenParentheses++;
                } else if (Tokens[Index].Value == ")") {
                    if (CurrentState == State.Op) {
                        Errors.Add(new SyntaxException($"Unexpected {GetName(Tokens[Index])} on index {Tokens[Index].Index} can`t be placed after operator", Tokens[Index].Index, Tokens[Index].Value.Length));
                        // AddError(Tokens[Index]); // 7
                    }
                    if (CurrentState == State.Begin) {
                        Errors.Add(new SyntaxException($"Unexpected {GetName(Tokens[Index])} on index {Tokens[Index].Index} can`t be placed before open parenthesis or expression's opening", Tokens[Index].Index, Tokens[Index].Value.Length));
                        // AddError(Tokens[Index]); // 8
                    }
                    if (OpenParentheses == 0) {
                        Errors.Add(new SyntaxException($"{GetName(Tokens[Index])} on index {Tokens[Index].Index} can`t be placed immediatelly after number, function or another identifier", Tokens[Index].Index, Tokens[Index].Value.Length));
                        // AddError(Tokens[Index]); // 11
                    } else {
                        OpenParentheses--;
                    }
                    CurrentState = State.Object;
                } else if (Tokens[Index].Category == Category.Separator) {
                    if (CurrentState != State.Begin) {
                        if (OpenParentheses == 0) {
                            Errors.Add(new SyntaxException($"Unexpected {GetName(Tokens[Index])} on index {Tokens[Tokens.Count - 1].Index + Tokens[Tokens.Count - 1].Value.Length - 1}.\nAllowed only inside function arguments definition", Tokens[Tokens.Count - 1].Index + Tokens[Tokens.Count - 1].Value.Length, 0));
                            // AddError(Tokens[Index]); // ?? #1
                        } else {
                            Errors.Add(new SyntaxException($"Unexpected {GetName(Tokens[Index])} on index {Tokens[Tokens.Count - 1].Index + Tokens[Tokens.Count - 1].Value.Length - 1}", Tokens[Tokens.Count - 1].Index + Tokens[Tokens.Count - 1].Value.Length - 1, 0));
                            // AddError(Tokens[Index]); // ?? #2
                        }
                    }
                } else if (Tokens[Index].Category == Category.Name) {
                    if (CurrentState != State.Op && CurrentState != State.Begin) {
                        Errors.Add(new SyntaxException($"{GetName(Tokens[Index])} on index {Tokens[Index].Index} can`t be placed immediatelly after number, function or another identifier", Tokens[Index].Index, Tokens[Index].Value.Length));
                        // AddError(Tokens[Index]); // 11
                    }
                    CurrentState = State.Name;
                } else if (Tokens[Index].Category == Category.Number) {
                    if (CurrentState == State.Object) {
                        Errors.Add(new SyntaxException($"Unexpected {GetName(Tokens[Index])} on index {Tokens[Index].Index}.\n" +
                            "Expression should have only allowed operations, identifiers, numbers and parentheses ['(', ')']", Tokens[Index].Index, Tokens[Index].Value.Length));
                        // AddError(Tokens[Index]); // 3
                    }
                    CurrentState = State.Num;
                }
                Index++;
            }
            if (CurrentState == State.Op && Tokens.Count != 0) {
                Errors.Add(new SyntaxException($"Expected expression to end with number, identifier, function or nested expression in parantheses at the end instead of {GetName(Tokens[Tokens.Count - 1])} on index {Tokens[Tokens.Count - 1].Index + Tokens[Tokens.Count - 1].Value.Length - 1} ", Tokens[Tokens.Count - 1].Index + Tokens[Tokens.Count - 1].Value.Length - 1, Tokens[Tokens.Count - 1].Value.Length));
                // AddError(Tokens[Index]); // 1
            }
            if (OpenParentheses != 0 && Tokens.Count != 0) {
                if (OpenParentheses < 0) {
                    Errors.Add(new SyntaxException($"Expected a closing parenthesis ')' for every opening parenthesis '('", Tokens[Tokens.Count - 1].Index + Tokens[Tokens.Count - 1].Value.Length - 1, Tokens[Tokens.Count - 1].Value.Length));
                    // AddError(Tokens[Index]); // 9
                } else {
                    Errors.Add(new SyntaxException($"Expected an opening parenthesis '(' for every closing parenthesis ')'", Tokens[Tokens.Count - 1].Index + Tokens[Tokens.Count - 1].Value.Length - 1, Tokens[Tokens.Count - 1].Value.Length));
                    // AddError(Tokens[Index]); // 10
                }
            }
        }
           
        private string GetName(Token Token)
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
            return Found;
        }
    }
}
