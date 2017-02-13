using System.Windows.Forms;

namespace LetsEncryptAcmeReg
{
    internal interface IControlAppender
    {
        AppenderResult AddGroup(Label label, Control control, params Control[] other);
    }
}