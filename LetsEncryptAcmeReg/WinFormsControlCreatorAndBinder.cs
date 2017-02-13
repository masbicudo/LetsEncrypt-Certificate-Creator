using System;
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

        public CreateAndBindResult ForBool(Bindable<bool> bindable, string label, string tooltip)
        {
            var checkBox = new CheckBox
            {
                Text = label,
                Margin = new Padding(1),
                AutoSize = true,
                Anchor = 0,
            };

            var addResult = this.controlAppender.AddGroup(null, checkBox);

            Action killToolTipForm = null;
            if (!string.IsNullOrWhiteSpace(tooltip))
            {
                this.parent.ToolTipFor(checkBox, tooltip);
                killToolTipForm = () => this.parent.ToolTipFor(checkBox, null);
            }

            var init = bindable.BindControl(checkBox);
            return new CreateAndBindResult(init, killToolTipForm, addResult.RemoveGroup);
        }

        public CreateAndBindResult ForString(Bindable<string> bindable, string label, string tooltip)
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

            var addResult = this.controlAppender.AddGroup(labelCtl, textBox);

            Action killToolTipForm = null;
            if (!string.IsNullOrWhiteSpace(tooltip))
            {
                this.parent.ToolTipFor(textBox, tooltip);
                killToolTipForm = () => this.parent.ToolTipFor(textBox, null);
            }

            var init = bindable.BindControl(textBox);
            return new CreateAndBindResult(init, killToolTipForm, addResult.RemoveGroup);
        }
    }
}