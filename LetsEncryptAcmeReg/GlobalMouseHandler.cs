using System.Windows.Forms;
using LetsEncryptAcmeReg.WinAPI;

namespace LetsEncryptAcmeReg
{
    public class GlobalMouseHandler : IMessageFilter
    {
        private readonly IGlobalEvents defaultHandler;

        public GlobalMouseHandler(IGlobalEvents defaultHandler = null)
        {
            this.defaultHandler = defaultHandler;
        }

        public bool PreFilterMessage(ref Message m)
        {
            var msg = (WindowsMessage)m.Msg;
            if (msg == WindowsMessage.WM_TIMER)
                return false;

            var activeForm = Form.ActiveForm;
            var globalEvents = activeForm as IGlobalEvents ?? this.defaultHandler;
            if (globalEvents == null) return false;
            var ctl = Control.FromHandle(m.HWnd);
            if (ctl == null && activeForm == null) return false;
            if (msg == WindowsMessage.WM_MOUSEMOVE)
            {
                var wp = (MouseWParam)m.WParam;
                var lp = (MouseLParam)m.LParam;

                var e = new GlobalMouseEventArgs(MouseEvents.MouseMove, (MouseButtons)wp, 0, lp.X, lp.Y, 0);
                globalEvents.GlobalMouseMove(ctl ?? activeForm, e);
            }
            else if (msg == WindowsMessage.WM_LBUTTONDOWN)
            {
            }
            else if (msg == WindowsMessage.WM_RBUTTONDOWN)
            {
            }
            else if (msg == WindowsMessage.WM_LBUTTONUP)
            {
            }
            else if (msg == WindowsMessage.WM_RBUTTONUP)
            {
            }
            else if (msg == WindowsMessage.WM_LBUTTONDBLCLK)
            {
            }
            else if (msg == WindowsMessage.WM_RBUTTONDBLCLK)
            {
            }
            else if (msg == WindowsMessage.WM_MOUSEWHEEL)
            {
            }
            else if (msg == WindowsMessage.WM_MOUSEHWHEEL)
            {
            }
            else if (msg == WindowsMessage.WM_MOUSEHOVER)
            {
            }
            else if (msg == WindowsMessage.WM_MOUSELEAVE)
            {
            }
            else if (msg == WindowsMessage.WM_MOUSEACTIVATE)
            {
            }
            else if (msg == WindowsMessage.WM_MOUSEFIRST)
            {
            }
            return false;
        }
    }
}