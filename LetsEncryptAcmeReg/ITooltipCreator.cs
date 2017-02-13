using System.Windows.Forms;

namespace LetsEncryptAcmeReg
{
    public interface ITooltipCreator
    {
        void ToolTipFor(Control ctl, string message, string positionPreferences = ">,v,^,<,>v,>^,<v,<^");
    }
}