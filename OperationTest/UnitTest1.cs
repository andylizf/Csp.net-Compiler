using System;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OperationTest
{
    public class Operation
    {
        public static Regex Is;
        public static Regex PreOperand, Operator, PostOperand;
        static string getEnumRegex(string[] enums)
        {
            var enumsRegex = new StringBuilder();
            foreach (var i in enums)
            {
                enumsRegex.Append($"({i})");
            }
            return $"[{enumsRegex}]";
        }
        static Operation()
        {
            var AllName = new Regex(@"[A-Z|a-z][A-Z|a-z|0-9]*(\.[A-Z|a-z][A-Z|a-z|0-9]*)*");
            //var preOperator = getEnumRegex(new[] { "!", "+", "-" });
            var mainOperator = getEnumRegex(new[] { @"\+", @"\-", @"\*", "/", "and", "or", "&&", @"\|\|", "!=", "==", @"\+\+", @"\-\-", "=" });
            PreOperand = new Regex($"(?<PreOperand>{AllName})");
            Operator = new Regex($"(?<Operator>{mainOperator})");
            PostOperand = new Regex($"(?<PostOperand>{AllName})?");
            
            Is = new Regex($"(?<PreOperand>{AllName}) ?((?<Operator>{mainOperator}) ?(?<PostOperand>{AllName})?)*");
        }
    }

    public static class RegexTestExt
    {
        public static MatchCollection TestIsMatchesAll(this Regex regex, string str)
        {
            var matchCollection = regex.Matches(str);
            var success = false;
            foreach (Match match in matchCollection)
            {
                if (match.Success && match.Length == str.Length)
                    success = true;
            }
            if (!success)
                Assert.Fail();

            return matchCollection;
        }
    }
    [TestClass]
    public class TestClass1
    {
        
        public bool IsAllMatch(Regex regex, string str)
        {
            foreach (Match match in regex.Matches(str))
            {
                if (match.Success && match.Length == str.Length)
                    return true;
            }

            return false;
        }
        [TestMethod]
        public void SegmentationTestOperation()
        {
            var a = new Regex(@"([A-Z|a-z][A-Z|a-z|0-9]*(\.[A-Z|a-z][A-Z|a-z|0-9]*)*)");
            var b = new Regex(@"([(\+)(\-)(\*)(/)(and)(or)(&&)(\|\|)(!=)(==)(\+\+)(\-\-)(=)])");
            var c = new Regex(@"([A-Z|a-z][A-Z|a-z|0-9]*(\.[A-Z|a-z][A-Z|a-z|0-9]*)*)?");
            var abc = new Regex($"{a}{b}{c}");

            Assert.IsTrue(IsAllMatch(a, "a"));
            Assert.IsTrue(IsAllMatch(b, "+"));
            Assert.IsTrue(IsAllMatch(c, "b"));
            Assert.IsTrue(IsAllMatch(abc, "a+b"));

        }

        void TestOperation()
        {
            Assert.IsTrue(IsAllMatch(Operation.Is, "a + b + c + c + e + d"));
            Assert.IsTrue(IsAllMatch(Operation.Is, "a * b + c"));
            Assert.IsFalse(IsAllMatch(Operation.Is, "a *efb rgbbgrfbgfedfevfv +"));
        }



        [TestMethod]
        public void TestPreOperand()
        {
            var match = Operation.PreOperand.Match("a");
            Assert.IsTrue(match.Success && match.Length == 1);
        }

        [TestMethod]
        public void TestPostOperand()
        {
            var PostOperand = new Regex(@"(?<PostOperand>[A-Z|a-z][A-Z|a-z|0-9]*(\.[A-Z|a-z][A-Z|a-z|0-9]*)*)?");

            var match = PostOperand.Match("b");
            Assert.IsTrue(match.Success && match.Length == 1);
        }

        [TestMethod]
        public void TestOperator()
        {
            var Operator = new Regex("(?<Operator>[(+)(-)(*)(/)(and)(or)(&&)(||)(!=)(==)(++)(--)(=)])");
            var match = Operator.Match("+");
            Assert.IsTrue(match.Success && match.Length == 1);
        }
    }
}
