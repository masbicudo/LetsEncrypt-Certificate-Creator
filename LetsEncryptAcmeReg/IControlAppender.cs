using System.Windows.Forms;

namespace LetsEncryptAcmeReg
{
    internal interface IControlAppender
    {
        void AddGroup(Label label, Control control, params Control[] other);
    }
}