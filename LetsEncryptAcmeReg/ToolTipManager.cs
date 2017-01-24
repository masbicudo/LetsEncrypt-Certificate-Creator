using System.Collections.Generic;
using System.Windows.Forms;

namespace LetsEncryptAcmeReg
{
    public class ToolTipManager : Control
    {
        private readonly Dictionary<Control, Dictionary<object, ToolTipForm>> dic = new Dictionary<Control, Dictionary<object, ToolTipForm>>();

        public ToolTipForm ToolTipFor(Control control, object key = null)
        {
            lock (dic)
            {
                Dictionary<object, ToolTipForm> dic2;
                if (!this.dic.TryGetValue(control, out dic2))
                {
                    dic2 = new Dictionary<object, ToolTipForm>();
                    this.dic.Add(control, dic2);
                }

                ToolTipForm form;
                if (!dic2.TryGetValue(key ?? control, out form))
                {
                    form = new ToolTipForm(control);
                    dic2.Add(key ?? control, form);
                }

                return form;
            }
        }
    }
}