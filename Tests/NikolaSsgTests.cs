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
    public class NikolaSsgTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            MockFile mockFile;
            Singletons.File = mockFile = new MockFile(new Dictionary<string, string>
            {
                { @"C:\Projetos\masb-blog-gitlab-pages\conf.py", @"
# Default is:
# FILES_FOLDERS = {'files': ''}
# Which means copy 'files' into 'output'
"}
            });

            var mockCreator = new Mock<IControlCreatorAndBinder>(MockBehavior.Loose);
            var mockController = new Mock<ISsgController>(MockBehavior.Loose);
            var model = new WizardBindableModel();
            model.SiteRoot.Value = @"C:\Projetos\masb-blog-gitlab-pages";
            var nikola = new NikolaSsg();
            nikola.Initialize(mockController.Object, model, mockCreator.Object);
            nikola.Patch();

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
            Singletons.File = mockFile = new MockFile(new Dictionary<string, string>
            {
                { @"C:\Projetos\masb-blog-gitlab-pages\conf.py", @"
# Default is:
FILES_FOLDERS = {'files': ''}
# Which means copy 'files' into 'output'
"}
            });

            var mockCreator = new Mock<IControlCreatorAndBinder>(MockBehavior.Loose);
            var mockController = new Mock<ISsgController>(MockBehavior.Loose);
            var model = new WizardBindableModel();
            model.SiteRoot.Value = @"C:\Projetos\masb-blog-gitlab-pages";
            var nikola = new NikolaSsg();
            nikola.Initialize(mockController.Object, model, mockCreator.Object);
            nikola.Patch();

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
            Singletons.File = mockFile = new MockFile(new Dictionary<string, string>
            {
                { @"C:\Projetos\masb-blog-gitlab-pages\conf.py", @"
# Default is:
FILES_FOLDERS = {'files': '', '.well-known': '.well-known'}
# Which means copy 'files' into 'output'
"}
            });

            var mockCreator = new Mock<IControlCreatorAndBinder>(MockBehavior.Loose);
            var mockController = new Mock<ISsgController>(MockBehavior.Loose);
            var model = new WizardBindableModel();
            model.SiteRoot.Value = @"C:\Projetos\masb-blog-gitlab-pages";
            var nikola = new NikolaSsg();
            nikola.Initialize(mockController.Object, model, mockCreator.Object);
            nikola.Patch();

            Assert.AreEqual(mockFile.Dic.First().Value, @"
# Default is:
FILES_FOLDERS = {'files': '', '.well-known': '.well-known'}
# Which means copy 'files' into 'output'
");
        }

        [TestMethod]
        public void TestMethod_Error1()
        {
            Singletons.File = new MockFile(new Dictionary<string, string>());
            var mockCreator = new Mock<IControlCreatorAndBinder>(MockBehavior.Loose);
            var mockController = new Mock<ISsgController>(MockBehavior.Loose);
            var model = new WizardBindableModel();
            model.SiteRoot.Value = @"C:\Projetos\masb-blog-gitlab-pages";
            var nikola = new NikolaSsg();
            nikola.Initialize(mockController.Object, model, mockCreator.Object);
            var errors = nikola.GetErrors().OfType<FileNotFoundException>().ToArray();

            Assert.IsTrue(errors.Length > 0);
        }
    }
}
