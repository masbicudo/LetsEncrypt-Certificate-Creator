using System.IO;

namespace Replaceables
{
    public class DefaultSystemIoDirectory :
        ISystemIoDirectory
    {
        public bool Exists(string path) => File.Exists(path);
    }
}