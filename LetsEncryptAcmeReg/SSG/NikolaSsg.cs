using Replaceables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LetsEncryptAcmeReg.SSG
{
    public class NikolaSsg :
        ISsg
    {
        private ISsgController controller;
        private ISsgMasterModel mainModel;
        private Model model;
        private BindResult bindResult;
        private Action removeUI;

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
                    mo.UpdateConfPy.Value));

            var confPy = createAndBind.ForBool(this.model.UpdateConfPy, "Update conf.py", Messages.ToolTipForConfPy);

            init += confPy.BindResult;

            init.InitAction?.Invoke();

            this.bindResult = new BindResult(null, init.UnbindAction);

            this.removeUI =
                confPy.RemoveTooltips + confPy.RemoveControls;

            return true;
        }

        public IEnumerable<Exception> GetErrors()
        {
            var confPath = Path.Combine(this.mainModel.SiteRoot.Value, "conf.py");
            if (!Singletons.File.Exists(confPath))
                yield return new FileNotFoundException("Nikola configuration file not found.");
        }

        public void Patch()
        {
            foreach (var exception in this.GetErrors())
                throw exception;

            // Creating the main challenge file
            Directory.CreateDirectory(this.mainModel.FilePath.Value);

            using (var fs = File.Open(Path.Combine(this.mainModel.FilePath.Value, "index.html"), FileMode.Create, FileAccess.ReadWrite))
            using (var sw = new StreamWriter(fs))
                sw.Write(this.mainModel.Key.Value);

            // Patching Nikola specific files
            var confPath = Path.Combine(this.mainModel.SiteRoot.Value, "conf.py");
            var conf = Singletons.File.ReadAllText(confPath);

            var matches = Regex.Matches(conf, @"(?<=^|\n|\r\n)\s*#?\s*FILES_FOLDERS\s*=\s*\{('.*?':\s*'.*?'(?:,'.*?':\s*'.*?')*)\}", RegexOptions.Singleline).Cast<Match>();
            // ReSharper disable once PossibleMultipleEnumeration
            var commented = matches.Where(m => m.Value.Trim().StartsWith("#")).ToArray();
            // ReSharper disable once PossibleMultipleEnumeration
            var coded = matches.Where(m => !m.Value.Trim().StartsWith("#")).ToArray();
            if (coded.Any())
            {
                var lastCoded = coded.Last();
                var strBuilder = new StringBuilder(conf);
                var values = lastCoded.Groups[1].Value.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                if (!values.Any(x => Regex.IsMatch(x, @"\.well-known':\s*'\.well-known'")))
                {
                    var lastLine = values.Last();
                    var codeAndComment = lastLine.Split("#".ToCharArray(), 2);
                    var linStart = Regex.Match(codeAndComment[0], @"^\s*").Value;
                    var newLine = $"{codeAndComment.FirstOrDefault()?.TrimEnd().TrimEnd(',')},{linStart}{codeAndComment.Skip(1).FirstOrDefault() ?? ""}\r\n'.well-known': '.well-known'";
                    values[values.Length - 1] = newLine;
                    var newValue = $"FILES_FOLDERS = {{{string.Join("\r\n", values)}}}";
                    strBuilder.Replace(lastCoded.Value, newValue, lastCoded.Index, lastCoded.Length);
                    conf = strBuilder.ToString();
                }
            }
            else
            {
                if (commented.Any())
                {
                    var lastComment = commented.Last();
                    var nextLine = conf.IndexOf("\n", lastComment.Index, StringComparison.Ordinal);

                    if (nextLine < 0) nextLine = conf.Length;
                    else nextLine++;

                    var strBuilder = new StringBuilder(conf);
                    strBuilder.Insert(nextLine, "FILES_FOLDERS = {'files': '',\r\n'.well-known': '.well-known'}\r\n");
                    conf = strBuilder.ToString();
                }
            }

            Singletons.File.WriteAllText(confPath, conf);
        }

        public void Dispose()
        {
            this.bindResult.UnbindAction?.Invoke();
            this.removeUI?.Invoke();
        }

        private string[] Files_Value(string siteRoot, string indexRelative, bool updateConfPy)
        {
            return this.controller.CatchError(() =>
            {
                if (new[] { siteRoot, indexRelative }.AnyNullOrWhiteSpace())
                    return new string[0];

                var items = Enumerable.Empty<string>();
                items = items.Append(Path.Combine(siteRoot, indexRelative, "index.html"));
                if (updateConfPy) items = items.Append(Path.Combine(siteRoot, "conf.py"));
                return items.Where(x => x != null).ToArray();
            });
        }

        class Model :
            ISsgModel
        {
            public Bindable<bool> UpdateConfPy { get; } = new Bindable<bool>(nameof(UpdateConfPy));
        }
    }
}
