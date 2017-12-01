using System.Windows.Forms;

namespace LetsEncryptAcmeReg
{
    public class GlobalMouseEventArgs : MouseEventArgs
    {
        public MouseEvents MouseEvent { get; }

        public GlobalMouseEventArgs(MouseEvents mouseEvent, MouseButtons button, int clicks, int x, int y, int delta)
            : base(button, clicks, x, y, delta)
        {
            this.MouseEvent = mouseEvent;
        }
    }
}