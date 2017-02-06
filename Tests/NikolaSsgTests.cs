using LetsEncryptAcmeReg.SSG;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Replaceables;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class NikolaSsgTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            MockFile mockFile;
            Singletons.File = mockFile = new MockFile(new Dictionary<string, string>()
            {
                { @"C:\Projetos\masb-blog-gitlab-pages\conf.py", @"
# Default is:
# FILES_FOLDERS = {'files': ''}
# Which means copy 'files' into 'output'
"}
            });

            var nikola = new Nikola();
            nikola.Init(@"C:\Projetos\masb-blog-gitlab-pages");

            var expected = @"
# Default is:
# FILES_FOLDERS = {'files': ''}
FILES_FOLDERS = {'files': '',
'.well-known': '.well-known'}
# Which means copy 'files' into 'output'
";
            var message = mockFile.Dic.First().Value;
            Assert.AreEqual(expected, message);
        }

        [TestMethod]
        public void TestMethod2()
        {
            MockFile mockFile;
            Singletons.File = mockFile = new MockFile(new Dictionary<string, string>()
            {
                { @"C:\Projetos\masb-blog-gitlab-pages\conf.py", @"
# Default is:
FILES_FOLDERS = {'files': ''}
# Which means copy 'files' into 'output'
"}
            });

            var nikola = new Nikola();
            nikola.Init(@"C:\Projetos\masb-blog-gitlab-pages");

            Assert.AreEqual(mockFile.Dic.First().Value, @"
# Default is:
FILES_FOLDERS = {'files': '',
'.well-known': '.well-known'}
# Which means copy 'files' into 'output'
");
        }

        [TestMethod]
        public void TestMethod3()
        {
            MockFile mockFile;
            Singletons.File = mockFile = new MockFile(new Dictionary<string, string>()
            {
                { @"C:\Projetos\masb-blog-gitlab-pages\conf.py", @"
# Default is:
FILES_FOLDERS = {'files': '', '.well-known': '.well-known'}
# Which means copy 'files' into 'output'
"}
            });

            var nikola = new Nikola();
            nikola.Init(@"C:\Projetos\masb-blog-gitlab-pages");

            Assert.AreEqual(mockFile.Dic.First().Value, @"
# Default is:
FILES_FOLDERS = {'files': '', '.well-known': '.well-known'}
# Which means copy 'files' into 'output'
");
        }
    }
}
