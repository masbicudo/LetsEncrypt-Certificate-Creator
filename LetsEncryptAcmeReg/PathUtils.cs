using System;

namespace LetsEncryptAcmeReg
{
    static class PathUtils
    {
        public static string CreateRelativePath(string path, string basePath)
        {
            if (path.StartsWith(basePath))
                return path.Substring(basePath.Length);
            throw new NotImplementedException();
        }
    }
}