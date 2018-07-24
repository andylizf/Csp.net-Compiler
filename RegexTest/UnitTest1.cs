using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Translation.Expression;
using Translation.RegexExt;

namespace RegexTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Regex r = new Regex("[A-Za-z]+");
            Assert.IsTrue(r.IsMatch("afgrgZDDWWRFG"));
        }
        [TestMethod]
        public void TestMethod2()
        {
            Regex r = new Regex("[A-Za-z]+");
            
            var m = r.MatchesAll("dfge33223eafgrgZDDWWRFG");
            Assert.IsNull(m);
        }
        [TestMethod]
        public void TestMethod3()
        {
            Regex r = new Regex("[a-zA-Z]+?(degrvbntbdfr)?");
            var match = r.MatchesAll("adegrvbntbdfr");
            Assert.IsNotNull(match);
        }
    }
}
