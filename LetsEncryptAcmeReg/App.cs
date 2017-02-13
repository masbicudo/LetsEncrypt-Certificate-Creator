using System.Deployment.Application;
using System.Reflection;

namespace LetsEncryptAcmeReg
{
    static class App
    {
        public static string CurrentVersion
        {
            get
            {
                return ApplicationDeployment.IsNetworkDeployed
                    ? ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString()
                    : Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

    }
}