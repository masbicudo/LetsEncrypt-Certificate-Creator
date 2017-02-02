using System;
using System.Runtime.InteropServices;
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
            if (Environment.OSVersion.Version.Major >= 6)
                SetProcessDpiAwareness(_Process_DPI_Awareness.Process_Per_Monitor_DPI_Aware);

            Environment.SetEnvironmentVariable("Desktop", Environment.GetFolderPath(Environment.SpecialFolder.Desktop));

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frm());
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
