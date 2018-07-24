using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Translation;
using Translation.Expression;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            const String input = @"using System
namespace Cspnet.FirstApp
main = () : int
{
    var i = 2
    return 0
}";
            var structLine = CspFile.Find(input);//Important. Make CspFile structuralization.
            Assert.IsNotNull(structLine);
        }
    }
}
