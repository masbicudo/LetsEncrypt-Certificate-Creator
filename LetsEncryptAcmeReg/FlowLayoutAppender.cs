using System.Linq;
using System.Windows.Forms;

namespace LetsEncryptAcmeReg
{
    class FlowLayoutAppender :
        IControlAppender
    {
        private readonly FlowLayoutPanel flow;

        public FlowLayoutAppender(FlowLayoutPanel flow)
        {
            this.flow = flow;
        }

        public void AddGroup(Label label, Control control, params Control[] other)
        {
            other = other.Prepend(control).Prepend(label).Where(x => x != null).ToArray();
            var table = new TableLayoutPanel
            {
                Margin = new Padding(0),
                ColumnCount = other.Length,
                RowCount = 1,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(0),
            };
            table.RowStyles.Add(new ColumnStyle(SizeType.AutoSize));
            for (int it = 0; it < other.Length; it++)
                table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            table.Controls.AddRange(other);
            this.flow.Controls.Add(table);
        }
    }
}