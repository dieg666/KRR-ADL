using MultiAgentLanguageGUI;
using MultiAgentLanguageModels.Queries;
using NUnit.Framework;

namespace ExecutableQuery
{
    public class NecessaryExecutableTest
    {
        [Test]
        public void Test1()
        {
            string YaleShootingProblemStory = @"
Agent a
Fluent loaded
Fluent alive
Action LOAD
Action SHOOT
initially [~loaded]
initially [alive]
LOAD causes [loaded]
SHOOT causes [~loaded] if [loaded]
SHOOT causes [~alive] if [loaded]
";
            // GIVEN
            var tokens = Tokenizer.Tokenize(YaleShootingProblemStory);
            var parserState = Parser.Parse(tokens);
            var expressions = parserState.Story;

            // WHEN
            string query = @"
necessary executable (SHOOT, [a]) from [~loaded]
";
            Query q = Parser.ParseQuery(Tokenizer.Tokenize(query), parserState);
            var res = q.Solve(expressions);

            // THEN
            Assert.AreEqual(true, res);
        }



        [Test]
        public void Test2()
        {
            string str = @"
Agent a
Fluent loaded
Fluent alive
Action LOAD
Action SHOOT
initially [~loaded]
initially [alive]
LOAD releases loaded
impossible SHOOT if [~loaded]
SHOOT causes [~loaded] if [loaded]
SHOOT causes [~alive] if [loaded]
";
            // GIVEN
            var tokens = Tokenizer.Tokenize(str);
            var parserState = Parser.Parse(tokens);
            var expressions = parserState.Story;

            // WHEN
            string query = @"
necessary executable (LOAD, [a]), (SHOOT, [a]) from [~loaded]
";
            Query q = Parser.ParseQuery(Tokenizer.Tokenize(query), parserState);
            var res = q.Solve(expressions);

            // THEN
            Assert.AreEqual(false, res);
        }


        [Test]
        public void Test3()
        {
            string str = @"
Agent a
Fluent loaded
Fluent alive
Action LOAD
Action SHOOT
initially [~loaded]
initially [alive]
LOAD causes [loaded]
SHOOT causes [~loaded] if [loaded]
SHOOT causes [~alive] if [loaded]
";
            // GIVEN
            var tokens = Tokenizer.Tokenize(str);
            var parserState = Parser.Parse(tokens);
            var expressions = parserState.Story;

            // WHEN
            string query = @"
necessary executable (LOAD, [a]), (SHOOT, [a]) from [~loaded]
";
            Query q = Parser.ParseQuery(Tokenizer.Tokenize(query), parserState);
            var res = q.Solve(expressions);

            // THEN
            Assert.AreEqual(true, res);
        }

        [Test]
        public void Test4()
        {
            string str = @"
Agent a
Fluent loaded
Fluent alive
Action LOAD
Action SHOOT
initially [~loaded]
initially [alive]
impossible LOAD by [a] if [loaded || ~loaded]
SHOOT causes [~loaded] if [loaded]
SHOOT causes [~alive] if [loaded]
";
            // GIVEN
            var tokens = Tokenizer.Tokenize(str);
            var parserState = Parser.Parse(tokens);
            var expressions = parserState.Story;

            // WHEN
            string query = @"
necessary executable (LOAD, [a]), (SHOOT, [a]) from [~loaded]
";
            Query q = Parser.ParseQuery(Tokenizer.Tokenize(query), parserState);
            var res = q.Solve(expressions);

            // THEN
            Assert.AreEqual(false, res);
        }

        [Test]
        public void Test5()
        {
            string str = @"
Action fire
Action spin
Fluent loaded
Fluent alive
fire causes [~loaded] 
fire causes [~alive] if [loaded]
spin causes [loaded]
initially [alive] 
observable [alive] after (spin, []), (fire, [])
";
            // GIVEN
            var tokens = Tokenizer.Tokenize(str);
            var parserState = Parser.Parse(tokens);
            var expressions = parserState.Story;

            // WHEN
            string query = @"
necessary executable (fire, []), (fire, []) from [~loaded]
";
            Query q = Parser.ParseQuery(Tokenizer.Tokenize(query), parserState);
            var res = q.Solve(expressions);

            // THEN
            Assert.AreEqual(true, res);
        }

        [Test]
        public void Test6()
        {
            string str = @"
Action fire
Action spin
Fluent loaded
Fluent alive
impossible fire if [~loaded]
fire causes [~loaded] 
fire causes [~alive]
spin causes [loaded || ~loaded]
spin releases loaded
initially [alive] 
[~alive] after (spin, []), (fire, [])
";
            // GIVEN
            var tokens = Tokenizer.Tokenize(str);
            var parserState = Parser.Parse(tokens);
            var expressions = parserState.Story;

            // WHEN
            string query = @"
necessary executable (spin, []), (fire, []) from [~loaded && alive]
";
            Query q = Parser.ParseQuery(Tokenizer.Tokenize(query), parserState);
            var res = q.Solve(expressions);

            // THEN
            Assert.AreEqual(true, res);
        }


        [Test]
        public void Test7()
        {
            string str = @"
            Agent Monica
            Action eat
            Fluent hungry
 
            initially [hungry] 
            eat causes [~hungry] if [hungry]
            ";

            string query = @"
            necessary executable (eat, [Monica]) from [~hungry]";

            Assert.AreEqual(true, TestQuery(str, query));
        }

        [Test]
        public void Test8()
        {
            string str = @"
            Agent Monica
            Action eat
            Action buy
            Fluent hungry
            Fluent has_food
 
            initially [~has_food && hungry]
        
            buy causes [has_food]
            eat causes [~hungry] if [has_food]
            ";

            string query = @"
            necessary executable (buy, [Monica]),(eat, [Monica]) from [~has_food && hungry]";

            Assert.AreEqual(true, TestQuery(str, query));
        }

        public bool TestQuery(string str, string query)
        {
            // GIVEN
            var tokens = Tokenizer.Tokenize(str);
            var parserState = Parser.Parse(tokens);
            var expressions = parserState.Story;

            Query q = Parser.ParseQuery(Tokenizer.Tokenize(query), parserState);
            var res = q.Solve(expressions);

            // THEN
            return res;
        }
    }
}


