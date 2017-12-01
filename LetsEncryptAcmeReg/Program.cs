using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Windows.Forms;

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

            Environment.SetEnvironmentVariable("Desktop", Environment.GetFolderPath(Environment.SpecialFolder.Desktop));

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var mainForm = new Form2();

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
