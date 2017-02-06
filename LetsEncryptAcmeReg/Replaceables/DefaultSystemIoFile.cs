using System.IO;

namespace Replaceables
{
    public class DefaultSystemIoFile :
        ISystemIoFile
    {
        public bool Exists(string path) => File.Exists(path);
        public string ReadAllText(string path) => File.ReadAllText(path);
        public void WriteAllText(string path, string contents) => File.WriteAllText(path, contents);
    }
}