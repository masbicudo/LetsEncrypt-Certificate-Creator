using LetsEncryptAcmeReg;
using LetsEncryptAcmeReg.SSG;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Replaceables;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class LibsTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var str1a = PathUtils.Simplify(".");
            var str1b = PathUtils.Simplify(".\\");
            var str1c = PathUtils.Simplify(".\\xpto");
            var str2 = PathUtils.Simplify("..\\");
            var str3 = PathUtils.Simplify("..");
            var str4 = PathUtils.Simplify("..\\..");
            var str5 = PathUtils.Simplify("..\\..\\");
            var str6 = PathUtils.Simplify("..\\abc\\..\\");
            var str7 = PathUtils.Simplify("..\\abc\\..");
            var str8a = PathUtils.Simplify("abc\\.");
            var str8b = PathUtils.Simplify("abc\\.\\");
            var str8c = PathUtils.Simplify("abc\\.\\.");
        }
    }
}
