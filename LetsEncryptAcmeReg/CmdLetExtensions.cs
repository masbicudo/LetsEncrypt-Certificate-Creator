using System.Linq;
using ACMESharp.POSH;

namespace LetsEncryptAcmeReg
{
    internal static class CmdLetExtensions
    {
        public static void Run(this XCmdlet @this)
        {
            foreach (var k in @this.Invoke())
            {
            }
        }

        public static T GetValue<T>(this XCmdlet @this)
        {
            foreach (T k in @this.Invoke())
                return k;
            return default(T);
        }

        public static dynamic GetValue(this XCmdlet @this)
        {
            foreach (dynamic k in @this.Invoke())
                return k;
            return null;
        }

        public static dynamic[] GetValues(this XCmdlet @this)
        {
            return @this.Invoke<dynamic>().ToArray();
        }
    }
}