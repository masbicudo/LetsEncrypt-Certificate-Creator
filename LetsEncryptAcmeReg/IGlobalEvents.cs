using System.Windows.Forms;

namespace LetsEncryptAcmeReg
{
    public interface IGlobalEvents
    {
        void GlobalMouseMove(Control target, GlobalMouseEventArgs args);
    }
}