using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Replaceables;

namespace LetsEncryptAcmeReg.SSG
{
    public class Nikola :
        ISsg
    {
        private string dir;

        public bool Initialize(ISsgController controller, ISsgMasterModel mainModel, IControlCreatorAndBinder createAndBind)
        {
            this.dir = mainModel.SiteRoot.Value;

            var confPath = Path.Combine(this.dir, "conf.py");
            if (!Singletons.File.Exists(confPath))
                return false;

            var conf = Singletons.File.ReadAllText(confPath);

            var matches = Regex.Matches(conf, @"(?<=^|\n|\r\n)\s*#?\s*FILES_FOLDERS\s*=\s*\{('.*?':\s*'.*?'(?:,'.*?':\s*'.*?')*)\}").Cast<Match>();
            var commented = matches.Where(m => m.Value.Trim().StartsWith("#")).ToArray();
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
                    var nextLine = conf.IndexOf("\n", lastComment.Index);
                    if (nextLine < 0) nextLine = conf.Length;
                    else nextLine++;
                    var strBuilder = new StringBuilder(conf);
                    strBuilder.Insert(nextLine, "FILES_FOLDERS = {'files': '',\r\n'.well-known': '.well-known'}\r\n");
                    conf = strBuilder.ToString();
                }
            }

            Singletons.File.WriteAllText(confPath, conf);

            return true;
        }

        public bool IsValid()
        {
            return true;
        }

        public void Patch()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}
