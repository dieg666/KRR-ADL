using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiAgentLanguageModels;
using MultiAgentLanguageModels.Expressions;

namespace MultiAgentLanguageGUI
{
    public class ParserState
    {
        public List<Token> TokenList { get; set; }
        public Dictionary<string, MultiAgentLanguageModels.Fluent> Fluent { get; set; }
        public Dictionary<string, MultiAgentLanguageModels.Action> Action { get; set; }
        public List<MultiAgentLanguageModels.Expressions.Expression> Expression { get; set; }



        public ParserState(List<Token> tokenList)
        {
            TokenList = tokenList;
            Fluent = new Dictionary<string, MultiAgentLanguageModels.Fluent>();
            Action = new Dictionary<string, MultiAgentLanguageModels.Action>();
            Expression = new List<MultiAgentLanguageModels.Expressions.Expression>();
        }

        public Token PopToken()
        {
            if (TokenList.Count == 0)
            {
                return null;
            }
            Token a = TokenList[0];
            TokenList.RemoveAt(0);
            return a;
        }

        public Token PeepToken()
        {
            if (TokenList.Count == 0)
            {
                return null;
            }
            return TokenList[0];
        }

        public void AddFluent(string name)
        {
            Fluent.Add(name, new MultiAgentLanguageModels.Fluent(name));
        }
        public void AddAction(string name, int duration)
        {
            Action.Add(name, new MultiAgentLanguageModels.Action(name,duration));
        }


        public bool NameAvailable(Token fallbackToken, string name)
        {
            if (Tokenizer.Keyword.ContainsKey(name))
            {
                fallbackToken.ThrowException("Attempting to use a special keyword as a name.");
            }
            if (Fluent.ContainsKey(name))
                return false;
            if (Action.ContainsKey(name))
                return false;
            return true;
        }

        public string FluentList()
        {
            string output = "";
            foreach (string key in Fluent.Keys)
            {
                output += key + ", ";
            }
            return output;
        }
        public string ActionList()
        {
            string output = "";
            foreach (string key in Action.Keys)
            {
                output += key + ", ";
            }
            return output;
        }


        public string ExpressionList()
        {
            string output = "";
            foreach (var exp in Expression)
            {
                //output += exp.ToProlog() + "\n";
            }
            return output;
        }
    }

    public class Parser
    {
        public static readonly Dictionary<TokenType, System.Action<ParserState, Token>> TokenTypeHandle = new Dictionary<TokenType, System.Action<ParserState, Token>>
        {
            { TokenType.Agent, ParseAgent },
            { TokenType.Fluent, ParseFluent },
            { TokenType.Action, ParseAction },
            { TokenType.Keyword, ParseKeyword }
        };

        public static void ParseAgent(ParserState state, Token firstToken)
        {
            Token a = state.PopToken();
            if (a == null)
            {
                firstToken.ThrowException("No agent name");
            }
            else
            {
                firstToken.ThrowException("Attempt to use an already used name");
            }
        }
        public static void ParseFluent(ParserState state, Token firstToken)
        {
            Token a = state.PopToken();
            if (a == null)
            {
                firstToken.ThrowException("No fluent name");
            }
            if (state.NameAvailable(a,a.Name))
            {
                state.AddFluent(a.Name);
            }
            else
            {
                firstToken.ThrowException("Attempt to use an already used name");
            }
        }
        public static void ParseAction(ParserState state, Token firstToken)
        {
            Token a = state.PopToken();
            if (a == null)
            {
                firstToken.ThrowException("No action name");
            }
            if (state.NameAvailable(a,a.Name))
            {

                //deleting duration word
                Token durationWord = state.PopToken();
                //selecting the time
                Token duration = state.PopToken();
                int n;

                if (!int.TryParse(duration.Name, out n))
                {
                    firstToken.ThrowException("Action has to have numerical duration");
                }
                else
                {
                    state.AddAction(a.Name, int.Parse(duration.Name));
                    
                }
            }
            else
            {
                firstToken.ThrowException("Attempt to use an already used name");
            }
        }

        public static LogicElement EntryC1(ParserState state)
        {
            Token a;
            a = state.PopToken();
            if (a.Type != TokenType.Operator || a.Name != "[")
            {
                a.ThrowException("Expected '[' at the beginning of a logic expression.");
            }
            LogicElement c = C1(state);
            a = state.PopToken();
            if (a.Type != TokenType.Operator || a.Name != "]")
            {
                a.ThrowException("Expected ']' at the end of a logic expression.");
            }
            return c;
        }

        public static LogicElement C1(ParserState state)
        {
            LogicElement fluent = C2(state);
            if (fluent == null) return null;
            LogicElement exp = C1prime(state);
            if (exp != null)
            {
                exp.Left = fluent;
                return exp;
            }
            return fluent;
        }

        public static LogicElement C2(ParserState state)
        {
            LogicElement fluent = C3(state);
            if (fluent == null) return null;
            LogicElement exp = C2prime(state);
            if (exp != null)
            {
                exp.Left = fluent;
                return exp;
            }
            return fluent;
        }

        public static LogicElement C3(ParserState state)
        {
            LogicElement fluent = C4(state);
            if (fluent == null) return null;
            LogicElement exp = C3prime(state);
            if (exp != null)
            {
                exp.Left = fluent;
                return exp;
            }
            return fluent;
        }

        public static LogicElement C4(ParserState state)
        {
            LogicElement fluent = C5(state);
            if (fluent == null) return null;
            LogicElement exp = C4prime(state);
            if (exp != null)
            {
                exp.Left = fluent;
                return exp;
            }
            return fluent;
        }

        public static LogicElement C5(ParserState state)
        {
            Token t = state.PopToken();
            if (t.Name == "(")
            {
                LogicElement inside = C1(state);
                Token close = state.PopToken();
                if (close == null || close.Name != ")")
                {
                    t.ThrowException("No closing brackets");
                }
                return inside;
            }
            else if (t.Name == "~")
            {
                Token name = state.PopToken();
                if (!state.Fluent.ContainsKey(name.Name))
                    name.ThrowException("Expected fluent name");
                Fluent f = new Fluent(name.Name);
                Not retVal = new Not(f);
                return retVal;
            }
            else if (state.Fluent.ContainsKey(t.Name))
            {
                Fluent f = new Fluent(t.Name);
                f.Value = true;
                return f;
            }
            else
            {
                t.ThrowException("Error in logical expression. Mismatched bracekts or operator in wrong places.");
            }
            return null;
        }

        public static LogicElement C1prime(ParserState state)
        {
            Token t = state.PeepToken();
            if (t == null) return null;
            if (t.Name == "&&")
            {
                state.PopToken();
                LogicElement exp = C2(state);
                if (exp.Right == null) exp.Right = C1prime(state);
                And and = new And(null, exp);
                return and;
            }
            else if (t.Type == TokenType.Operator)
            {
                return null;
            }
            else if (t.Type != TokenType.Keyword && !state.Action.ContainsKey(t.Name))
            {
                t.ThrowException("Expected keyword token.");
            }
            return null;

        }
        public static LogicElement C2prime(ParserState state)
        {
            Token t = state.PeepToken();
            if (t == null) return null;
            if (t.Name == "||")
            {
                state.PopToken();
                LogicElement exp = C3(state);
                if (exp.Right == null) exp.Right = C2prime(state);
                Or or = new Or(null, exp);
                return or;
            }
            else if (t.Type == TokenType.Operator)
            {
                return null;
            }
            else if (t.Type != TokenType.Keyword && !state.Action.ContainsKey(t.Name))
            {
                t.ThrowException("Expected keyword token or action.");
            }
            return null;
        }

        public static LogicElement C3prime(ParserState state)
        {
            Token t = state.PeepToken();
            if (t == null) return null;
            if (t.Name == "<->")
            {
                state.PopToken();
                LogicElement exp = C4(state);
                if (exp.Right == null) exp.Right = C3prime(state);
                Iff iff = new Iff(null, exp);
                return iff;
            }
            else if (t.Type == TokenType.Operator)
            {
                return null;
            }
            else if (t.Type != TokenType.Keyword && !state.Action.ContainsKey(t.Name))
            {
                t.ThrowException("Expected keyword token.");
            }
            return null;
        }

        public static LogicElement C4prime(ParserState state)
        {
            Token t = state.PeepToken();
            if (t == null) return null;
            if (t.Name == "->")
            {
                state.PopToken();
                LogicElement exp = C5(state);
                if (exp.Right == null) exp.Right = C4prime(state);
                If if_exp = new If(null, exp);
                return if_exp;
            }
            else if (t.Type == TokenType.Operator)
            {
                return null;
            }
            else if (t.Type != TokenType.Keyword && !state.Action.ContainsKey(t.Name))
            {
                t.ThrowException("Expected keyword token.");
            }
            return null;
        }

        public static void ParseKeyword(ParserState state, Token firstToken)
        {
            switch (firstToken.Name)
            {
                case "causes":
                    MultiAgentLanguageModels.Action act =
                        new MultiAgentLanguageModels.Action(state.TokenList[state.TokenList.Count - 1].Name);
                    state.TokenList.RemoveAt(state.TokenList.Count - 1);
                    LogicElement effect = EntryC1(state);
                    Token if_exp = state.PeepToken();
                    if (if_exp != null && if_exp.Name == "if")
                    {
                        state.PopToken();
                        LogicElement con = EntryC1(state);
                        state.Expression.Add(new CausesIf(act, effect, con));
                    }
                    else
                    {
                        state.Expression.Add(new Causes(act, effect));
                    }
                    break;
                case "releases":
                    MultiAgentLanguageModels.Action act1 =
                        new MultiAgentLanguageModels.Action(state.TokenList[state.TokenList.Count - 1].Name);
                    state.TokenList.RemoveAt(state.TokenList.Count - 1);
                    Token eff1 = state.PopToken();
                    if (eff1 == null)
                        firstToken.ThrowException("Expected fluent after release.");
                    else if (!state.Fluent.ContainsKey(eff1.Name)) firstToken.ThrowException("Attempting to use undeclared fluent.");
                    Token if_expr = state.PeepToken();
                    if (if_expr != null && if_expr.Name == "if")
                    {
                        state.PopToken();
                        LogicElement con = EntryC1(state);
                        state.Expression.Add(new ReleasesIf(act1, state.Fluent[eff1.Name], con));
                        state.Expression.Add(new CausesIf(act1, new Or(state.Fluent[eff1.Name], new Not(state.Fluent[eff1.Name])), con));
                    }
                    else
                    {
                        state.Expression.Add(new Releases(act1, state.Fluent[eff1.Name]));
                        state.Expression.Add(new Causes(act1, new Or(state.Fluent[eff1.Name], new Not(state.Fluent[eff1.Name]))));
                    }
                    break;
                case "if":
                    firstToken.ThrowException("Unexpected 'if' token.");
                    break;


            }
        }


        private static List<MultiAgentLanguageModels.Action> GetActionList(ParserState state)
        {
            List<MultiAgentLanguageModels.Action> actions = new List<MultiAgentLanguageModels.Action>();
            do
            {
                if (state.PeepToken().Name == ",") state.PopToken();               
                Token a = state.PopToken();
                if (a == null || state.Action.ContainsKey(a.Name) == false)
                {
                    throw new Exception("Expected action.");
                }
                actions.Add(new MultiAgentLanguageModels.Action(a.Name));
            } while (state.PeepToken() != null && state.PeepToken().Name == ",");
            return actions;
        }

        //public static Query ParseQuery(List<Token> tokenList, ParserState story)
        //{
        //    ParserState state = new ParserState(tokenList);
        //    state.Action = story.Action;
            
        //    state.Noninertial = story.Noninertial;
        //    state.Fluent = story.Fluent;
        //    if(tokenList.Count == 0)
        //    {
        //        throw new Exception("Empty query");
        //    }

        //    Token first = state.PopToken();
        //    if(first.Name == "necessary" || first.Name == "possibly")
        //    {
        //        Token t = state.PopToken();
        //        Token next = state.PeepToken();
        //        if (t == null) first.ThrowException("Expected: executable, agents list or logic expression.");
        //        if(t.Name == "executable") // necessary executable
        //        {
        //            if (state.PeepToken() == null) t.ThrowException("Expected program.");
        //            Instruction inst = GetInstructions(state, t);
        //            Token from = state.PopToken();
        //            if (from == null)
        //            {
        //                if (first.Name == "necessary") return new NecessaryExecutable(inst);
        //                else return new PossiblyExecutable(inst);
        //            }
        //            if(from.Name != "from") t.ThrowException("Expected from after program.");
        //            LogicElement cond = EntryC1(state);
        //            if (first.Name == "necessary") return new NecessaryExecutableFrom(inst,cond);
        //            else return new PossiblyExecutableFrom(inst,cond);
        //        }
        //        else if(next != null && (state.Fluent.ContainsKey(next.Name) || 
        //            state.Noninertial.ContainsKey(next.Name) || next.Name == "(" || next.Name == "~")) // necessary value
        //        {
        //            state.TokenList.Insert(0, t);
        //            LogicElement result = EntryC1(state);
        //            Token after = state.PopToken();
        //            if (after == null || after.Name != "after")
        //            {
        //                t.ThrowException("Expected 'after' after result.");
        //            }
        //            Instruction inst = GetInstructions(state, t);
        //            Token from = state.PopToken();
        //            if (from == null)
        //            {
        //                if (first.Name == "necessary") return new NecessaryAfter(inst,result);
        //                else return new PossiblyAfter(inst, result);
        //            }
        //            if (from.Name != "from") t.ThrowException("Expected from after program.");
        //            LogicElement cond = EntryC1(state);
        //            if (first.Name == "necessary") return new NecessaryAfterFrom(inst,result,cond);
        //            else return new PossiblyAfterFrom(inst, result, cond);
        //        }
        //        else
        //        {
        //            throw new Exception("Incorrect query.");
        //        }
        //    }
        //    else
        //    {
        //        first.ThrowException("Expected 'necessary' or 'possibly'.");
        //    }
        //    return null;
        //}

        public static ParserState Parse(List<Token> tokenList)
        {
            ParserState state = new ParserState(tokenList);

            while (tokenList.Count > 0)
            {
                Token token = state.PopToken();
                if (TokenTypeHandle.ContainsKey(token.Type))
                {
                    Action<ParserState, Token> action = TokenTypeHandle[token.Type];
                    action(state, token);
                }
                else if (state.Action.ContainsKey(token.Name))
                {
                    Token keyword = state.PopToken();
                    if (keyword.Name == "by" || keyword.Name == "causes" || keyword.Name == "releases" || keyword.Name == "not")
                        state.TokenList.Add(token);
                    ParseKeyword(state, keyword);
                }
                else if (token.Name == "(" || token.Name == "~" || token.Name == "[" ||
                    state.Fluent.ContainsKey(token.Name))
                {
                    state.TokenList.Insert(0, token);
                    Token kw = new Token(token.LineNumber, token.ColumnNumber);
                    kw.Name = "after";
                    ParseKeyword(state, kw);
                }
                //else if(token.n)
                else
                {
                    token.ThrowException("Illegal token at position.");
                }
            }

            return state;
        }
    }
}
