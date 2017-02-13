using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LetsEncryptAcmeReg.SSG
{
    public class BaseSsg :
        ISsg
    {
        private ISsgMasterModel mainModel;
        private ISsgController controller;
        private BindResult bindResult;

        public bool Initialize(ISsgController controller, ISsgMasterModel mainModel, IControlCreatorAndBinder createAndBind)
        {
            this.controller = controller;
            this.mainModel = mainModel;

            BindResult init = BindResult.Null;
            init += mainModel.Files.BindExpression(
                () => this.Files_Value(
                    mainModel.SiteRoot.Value,
                    mainModel.FileRelativePath.Value));

            init.InitAction?.Invoke();

            this.bindResult = new BindResult(null, init.UnbindAction);
            return true;
        }

        public IEnumerable<Exception> GetErrors()
        {
            yield break;
        }

        public void Patch()
        {
            foreach (var exception in this.GetErrors())
                throw exception;

            Directory.CreateDirectory(this.mainModel.FilePath.Value);

            using (var fs = File.Open(Path.Combine(this.mainModel.FilePath.Value, "index.html"), FileMode.Create, FileAccess.ReadWrite))
            using (var sw = new StreamWriter(fs))
                sw.Write(this.mainModel.Key.Value);
        }

        public void Dispose()
        {
            this.bindResult.UnbindAction?.Invoke();
        }

        private string[] Files_Value(string siteRoot, string indexRelative)
        {
            return this.controller.CatchError(() =>
            {
                if (new[] { siteRoot, indexRelative }.AnyNullOrWhiteSpace())
                    return new string[0];

                var items = Enumerable.Empty<string>();
                items = items.Append(Path.Combine(siteRoot, indexRelative, "index.html"));
                return items.Where(x => x != null).ToArray();
            });
        }
    }
}
