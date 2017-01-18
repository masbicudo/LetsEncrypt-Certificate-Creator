using System.Linq;
using System.Management.Automation;

namespace LetsEncryptAcmeReg
{
    internal static class CmdLetExtensions
    {
        public static void Run(this Cmdlet @this)
        {
            foreach (var k in @this.Invoke())
            {
            }
        }

        public static T GetValue<T>(this Cmdlet @this)
        {
            foreach (T k in @this.Invoke())
                return k;
            return default(T);
        }

        public static dynamic GetValue(this Cmdlet @this)
        {
            foreach (dynamic k in @this.Invoke())
                return k;
            return null;
        }

        public static dynamic[] GetValues(this Cmdlet @this)
        {
            return @this.Invoke<dynamic>().ToArray();
        }
    }
}