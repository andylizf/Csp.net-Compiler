using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Translation.Expression;
using Translation.RegexExt;

namespace ValueTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Assert.IsNotNull(Value.statements.MatchesAll("(){}"));
        }
        [TestMethod]
        public void TestMethod2()
        {
            Assert.IsNotNull(Value.Is.MatchesAll("2333333"));
        }
    }
}
