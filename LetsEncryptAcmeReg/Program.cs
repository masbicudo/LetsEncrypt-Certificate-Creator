using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Windows.Forms;
using LetsEncryptAcmeReg.Config;
using Newtonsoft.Json;

namespace LetsEncryptAcmeReg
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //WindowsIdentity identity = WindowsIdentity.GetCurrent();
            //WindowsPrincipal principal = new WindowsPrincipal(identity);
            //var isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
            //if (!isElevated)
            //{
            //    MessageBox.Show("This program requires Administrator privileges.", "Error!", MessageBoxButtons.OK);
            //    return;
            //}

            if (Environment.OSVersion.Version.Major >= 6)
                SetProcessDpiAwareness(_Process_DPI_Awareness.Process_Per_Monitor_DPI_Aware);

            if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("Desktop")))
                Environment.SetEnvironmentVariable("Desktop", Environment.GetFolderPath(Environment.SpecialFolder.Desktop));


            Root rootCfg = new Root();
            if (File.Exists("config.json"))
                rootCfg = JsonConvert.DeserializeObject<Root>(File.ReadAllText("config.json"));
            else
                File.WriteAllText("config.json", JsonConvert.SerializeObject(rootCfg, Formatting.Indented));

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var mainForm = new Form2(rootCfg);

            GlobalMouseHandler globalClick = new GlobalMouseHandler(mainForm);
            Application.AddMessageFilter(globalClick);

            Application.Run(mainForm);
        }
        //[DllImport("user32.dll")]
        //private static extern bool SetProcessDPIAware();

        [DllImport("shcore.dll")]
        static extern int SetProcessDpiAwareness(_Process_DPI_Awareness value);

        enum _Process_DPI_Awareness
        {
            //Process_DPI_Unaware = 0,
            //Process_System_DPI_Aware = 1,
            Process_Per_Monitor_DPI_Aware = 2
        }
    }
}
