using Microsoft.VisualStudio.TestTools.UnitTesting;
using Translation.Expression;
using Translation.RegexExt;

namespace CommentTest
{
    [TestClass]
    public class UnitTest1
    {
    /*
    1.          var a = "//"
    Excepted    var {varName:a} = {varValue:"//"}
    Wrong1      var {varName:a} = "//{Comment:"}
    To be like the excepted, the comment must has lower precedence than IValue


    2.          var a = "" // Assign 
    Excepted    var {varName:a} = {varValue:""} //{Comment:"Assign"}
    Wrong1      var {varName:a} = {varValue:"" // Assign}
    To be like the excepted, the comment must has higher precedence than IValue
    
    1, 2 contradictory, so the comment now supports only single-line comments
    //TODO The comment does not support cross, row, and footer comments         
    */
        /*
        [TestMethod]
        public void TestVarExpression()
        {
            var varname = LocalVaribleName.Is;
            var vartypename = MemberName.Is;
            var assign = $"( ?= ?(?<VarStatement_Value>{Statement.Value.Is}))";
            var comment = @"(?<Comment>\s*(//[^\n]*))?";
            var regex = 
                new Regex($"var (?<VarStatement_VarName>{varname})( ?: ?(?<VarStatement_Type>{vartypename}))?(?(VarStatement_Type){assign}?|{assign}){1,1}?{comment}");

            TestExpression(@"var a = 0 //declare varible", regex);
        }
        */

        [TestMethod]
        public void TestVarExpressionSimplified()
        {
            var regex =
                new Regex(@"(?<VarStatement_Value>.*?)\s*(?<Comment>//[^\n]*)");
            TestExpression(@"aaa // egbr", regex);
        }
        /*
        [TestMethod]
        public void TestEmptyExpression() =>
            TestExpression(@"  //declare varible",
                Statement.Comment.Map["Translation.Expression.Statement+EmptyStatement"]);
        //No Empty Statement now.
        */
        [TestMethod]
        public void TestFuncCallExpression()=>
            TestExpression(@"Console.WriteLine(""Hello world"")//output to console",
                Statement.Comment.Map["Translation.Expression.Statement+FuncCallStatement"]);
        

        public void TestExpression(string line, Regex regex)
        {
            /*
            var groupNames = regex.GetGroupNames();
            foreach (var groupName in regex.GetGroupNames())
            {
                
            }*/
            var match = regex.MatchesAll(line);
            var comment = match.Groups["Comment"];
            foreach (var capture in comment.Captures)
                if(capture.ToString() != "")
                    return;
            Assert.Fail();
        }
    }
}
