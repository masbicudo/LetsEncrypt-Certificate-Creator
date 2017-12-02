using System;

namespace LetsEncryptAcmeReg
{
    public static class PathUtils
    {
        public static string CreateRelativePath(string path, string basePath)
        {
            if (path.StartsWith(basePath))
                return path.Substring(basePath.Length);
            throw new NotImplementedException();
        }

        public static string Simplify(string path)
        {
            char[] result = new char[path.Length];
            int[] starts = new int[path.Length + 1];
            int curStart = 0;
            int level = 0;
            var dots = 0;
            var posWrite = 0;
            for (int it = 0; it < path.Length; it++)
            {
                var ch = path[it];
                if (ch == '\\' || ch == '/')
                {
                    level++;
                    level -= dots;

                    if (level < 0)
                        starts[level = 0] = posWrite + 1;

                    if (dots == 0)
                        starts[level] = posWrite + 1;
                    else
                        posWrite = starts[level] - 1;

                    curStart = it + 1;
                    dots = 0;
                }
                else if (ch == '.' && dots == it - curStart)
                {
                    dots++;

                    if (dots > 2)
                        throw new Exception("Invalid path");
                }

                if (posWrite >= 0)
                    result[posWrite] = ch;
                posWrite++;
            }

            level++;
            level -= dots;

            if (level < 0)
                starts[level = 0] = posWrite + 1;

            if (dots == 0)
                starts[level] = posWrite + 1;
            else
                posWrite = starts[level] - 1;

            if (posWrite >= 0)
                return new string(result, 0, posWrite);

            return "";
        }
    }
}