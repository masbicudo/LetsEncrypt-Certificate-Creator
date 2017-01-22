using System.Collections.Generic;
using System.Windows.Forms;

namespace LetsEncryptAcmeReg
{
    public class ToolTipManager : Control
    {
        private readonly Dictionary<Control, ToolTipForm> dic = new Dictionary<Control, ToolTipForm>();

        public ToolTipForm ToolTipFor(Control control)
        {
            ToolTipForm form;
            if (!this.dic.TryGetValue(control, out form))
            {
                form = new ToolTipForm(control);
                this.dic.Add(control, form);
            }

            return form;
        }
    }
}