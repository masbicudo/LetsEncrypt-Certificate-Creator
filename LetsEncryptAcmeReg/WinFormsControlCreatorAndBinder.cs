using System.Windows.Forms;
using LetsEncryptAcmeReg.SSG;

namespace LetsEncryptAcmeReg
{
    class WinFormsControlCreatorAndBinder :
        IControlCreatorAndBinder
    {
        private readonly IControlAppender controlAppender;
        private readonly ITooltipCreator parent;

        public WinFormsControlCreatorAndBinder(IControlAppender controlAppender, ITooltipCreator parent)
        {
            this.controlAppender = controlAppender;
            this.parent = parent;
        }

        public BindResult ForBool(Bindable<bool> bindable, string label, string tooltip)
        {
            var checkBox = new CheckBox
            {
                Text = label,
                Margin = new Padding(1),
                AutoSize = true,
                Anchor = 0,
            };

            this.controlAppender.AddGroup(null, checkBox);

            this.parent.ToolTipFor(checkBox, tooltip);

            var init = bindable.BindControl(checkBox);
            return init;
        }

        public BindResult ForString(Bindable<string> bindable, string label, string tooltip)
        {
            var labelCtl = new Label
            {
                Text = label,
                Margin = new Padding(1),
                Anchor = 0,
            };
            var textBox = new TextBox
            {
                Margin = new Padding(1),
                Anchor = 0,
            };

            this.controlAppender.AddGroup(labelCtl, textBox);

            this.parent.ToolTipFor(textBox, tooltip);

            var init = bindable.BindControl(textBox);
            return init;
        }
    }
}