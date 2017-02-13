using System.IO;
using System.Linq;
using System.Text;

namespace LetsEncryptAcmeReg.SSG
{
    public class GitHubSsg :
        ISsg
    {
        private ISsgMasterModel mainModel;
        private Model model;
        private ISsgController controller;
        private BindResult bindResult;

        public bool Initialize(ISsgController controller, ISsgMasterModel mainModel, IControlCreatorAndBinder createAndBind)
        {
            this.controller = controller;
            this.mainModel = mainModel;

            var mo = new Model();
            this.model = mo;

            BindResult init = BindResult.Null;
            init += mainModel.Files.BindExpression(
                () => this.Files_Value(
                    mainModel.SiteRoot.Value,
                    mainModel.FileRelativePath.Value,
                    mo.UpdateCname.Value,
                    mo.UpdateConfigYml.Value));

            init.InitAction?.Invoke();

            init += createAndBind.ForBool(this.model.UpdateCname, "Update CNAME", Messages.ToolTipForCname);
            init += createAndBind.ForBool(this.model.UpdateConfigYml, "Update _config.yml", Messages.ToolTipForConfigYml);

            this.bindResult = init;
            return true;
        }

        public bool IsValid()
        {
            return true;
        }

        public void Patch()
        {
            Directory.CreateDirectory(this.mainModel.FilePath.Value);

            using (var fs = File.Open(Path.Combine(this.mainModel.FilePath.Value, "index.html"), FileMode.Create, FileAccess.ReadWrite))
            using (var sw = new StreamWriter(fs))
                sw.Write(this.mainModel.Key.Value);

            if (this.model.UpdateConfigYml.Value)
                using (var fs = File.Open(Path.Combine(this.mainModel.SiteRoot.Value, "_config.yml"), FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    string allText;
                    using (var sr = new StreamReader(fs, Encoding.UTF8, false, 1024, true))
                        allText = sr.ReadToEnd();

                    if (!allText.Contains(@""".well-known"""))
                        using (var sw = new StreamWriter(fs))
                            sw.Write(@"
# Handling Reading
include:      ["".well-known""]
");
                }

            if (this.model.UpdateCname.Value)
                using (var fs = File.Open(Path.Combine(this.mainModel.SiteRoot.Value, "CNAME"), FileMode.Create, FileAccess.ReadWrite))
                using (var sw = new StreamWriter(fs))
                    sw.Write(this.mainModel.Domain.Value);
        }

        public void Dispose()
        {
            this.bindResult.UnbindAction?.Invoke();
        }

        private string[] Files_Value(string siteRoot, string indexRelative, bool updateCname, bool updateConfigYml)
        {
            return this.controller.CatchError(() =>
            {
                if (new[] { siteRoot, indexRelative }.AnyNullOrWhiteSpace())
                    return new string[0];

                var items = Enumerable.Empty<string>();
                items = items.Append(Path.Combine(siteRoot, indexRelative, "index.html"));
                if (updateCname) items = items.Append(Path.Combine(siteRoot, "CNAME"));
                if (updateConfigYml) items = items.Append(Path.Combine(siteRoot, "_config.yml"));
                return items.Where(x => x != null).ToArray();
            });
        }

        class Model :
            ISsgModel
        {
            public Bindable<bool> UpdateConfigYml { get; } = new Bindable<bool>(nameof(UpdateConfigYml));
            public Bindable<bool> UpdateCname { get; } = new Bindable<bool>(nameof(UpdateCname));
        }
    }
}
