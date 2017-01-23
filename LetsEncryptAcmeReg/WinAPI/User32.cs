using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace LetsEncryptAcmeReg.WinAPI
{
    static class User32
    {

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern bool SetWindowPos(
            int hWnd, // window handle
            int hWndInsertAfter, // placement-order handle
            int X, // horizontal position
            int Y, // vertical position
            int cx, // width
            int cy, // height
            uint uFlags); // window positioning flags

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(Point p);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, ShowWindowCommands nCmdShow);
    }
}
