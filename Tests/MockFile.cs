using System;
using System.Collections.Generic;
using Replaceables;

namespace Tests
{
    public class MockFile :
        ISystemIoFile
    {
        public readonly Dictionary<string, string> Dic;

        public MockFile(Dictionary<string, string> dic)
        {
            if (dic == null) throw new ArgumentNullException(nameof(dic));
            this.Dic = dic;
        }

        public bool Exists(string path)
        {
            return this.Dic.ContainsKey(path);
        }

        public string ReadAllText(string path)
        {
            return this.Dic[path];
        }

        public void WriteAllText(string path, string contents)
        {
            this.Dic[path] = contents;
        }
    }
}