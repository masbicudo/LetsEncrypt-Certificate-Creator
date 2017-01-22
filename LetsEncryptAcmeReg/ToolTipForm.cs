using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LetsEncryptAcmeReg
{
    public partial class ToolTipForm : Form
    {
        private readonly Control control;
        private string currentMessage;
        private bool canShow;
        private Size currentTextSize;

        public ToolTipForm(Control control)
        {
            this.control = control;
            InitializeComponent();
        }

        public void ShowMessage(string message)
        {
            if (message != this.currentMessage)
                this.Invalidate();

            this.currentMessage = message;
            this.currentTextSize = message == null ? Size.Empty : TextRenderer.MeasureText(this.currentMessage, this.Font);

            this.UpdateFormLocation();
        }

        private void UpdateFormLocation()
        {
            var frm = this.control.FindForm();

            this.canShow = Form.ActiveForm == frm;

            var size = this.currentTextSize;

            var cw = this.control.Width;
            var ch = this.control.Height;

            var padding = this.Padding;
            var tw = size.Width + 2 * padding;
            var th = size.Height + 2 * padding;

            var margin = this.Margin;

            var order = this.PositionPreferences.Split(',');

            var rectsDic = new Dictionary<string, Rectangle>
            {
                { ">", new Rectangle(+cw + 2*margin, (+ch - th)/2, tw, th) }, // >
                { "v", new Rectangle((+cw - tw)/2, +ch + 2*margin, tw, th) }, // v
                { "^", new Rectangle((+cw - tw)/2, -th - 2*margin, tw, th) }, // ^
                { "<", new Rectangle(-tw - 2*margin, (+ch - th)/2, tw, th) }, // <

                { ">v", new Rectangle(+cw + 2*margin, +ch + 2*margin, tw, th) }, // >v
                { ">^", new Rectangle(+cw + 2*margin, -th - 2*margin, tw, th) }, // >^
                { "<v", new Rectangle(-tw - 2*margin, +ch + 2*margin, tw, th) }, // <v
                { "<^", new Rectangle(-tw - 2*margin, -th - 2*margin, tw, th) }, // <^
            };

            var rects = order.Where(x => rectsDic.ContainsKey(x)).Select(x => rectsDic[x]).ToArray();

            int max = 0;
            Rectangle choice = Rectangle.Empty;
            if (size != Size.Empty)
                foreach (var r in rects)
                {
                    var a = this.control.PointToScreen(new Point(r.Left, r.Top));
                    var b = this.control.PointToScreen(new Point(r.Left, r.Bottom));
                    var c = this.control.PointToScreen(new Point(r.Right, r.Top));
                    var d = this.control.PointToScreen(new Point(r.Right, r.Bottom));
                    var sa = Screen.FromPoint(a);
                    var sb = Screen.FromPoint(b);
                    var sc = Screen.FromPoint(c);
                    var sd = Screen.FromPoint(d);

                    var rect = new Rectangle(a, new Size(d.X - a.X, d.Y - a.Y));

                    var val = 0;
                    val += (sa.Bounds.Contains(a) ? 4 : 0)
                         + (sb.Bounds.Contains(b) ? 4 : 0)
                         + (sc.Bounds.Contains(c) ? 4 : 0)
                         + (sd.Bounds.Contains(d) ? 4 : 0);
                    val += (sa.DeviceName == sb.DeviceName ? 1 : 0)
                         + (sa.DeviceName == sc.DeviceName ? 1 : 0)
                         + (sa.DeviceName == sd.DeviceName ? 1 : 0);

                    if (val > max && !rect.Contains(Cursor.Position))
                    {
                        max = val;
                        choice = rect;
                    }
                }

            if (max > 0)
            {
                if (this.Location != choice.Location)
                    this.Location = choice.Location;
                if (this.Size != choice.Size)
                    this.Size = choice.Size;
            }

            if (max > 0 && this.canShow)
            {
                if (!this.Visible)
                {
                    if (frm != null)
                    {
                        frm.Activated += Frm_Activated;
                        frm.Deactivate += Frm_Deactivate;
                        frm.Resize += Frm_Resize;
                        frm.Move += Frm_Resize;
                    }
                    ShowWindow(this.Handle, 8);
                }
            }
            else
            {
                if (this.Visible)
                {
                    if (frm != null)
                    {
                        frm.Activated -= Frm_Activated;
                        frm.Deactivate -= Frm_Deactivate;
                        frm.Resize -= Frm_Resize;
                        frm.Move -= Frm_Resize;
                    }
                    this.Hide();
                }
            }
        }

        public string PositionPreferences { get; set; } = ">,v,^,<,>v,>^,<v,<^";
        public new int Margin { get; set; } = -1;
        public new int Padding { get; set; } = 4;

        private void Frm_Activated(object sender, EventArgs e)
        {
            this.canShow = true;
            this.UpdateFormLocation();
        }

        private void Frm_Deactivate(object sender, EventArgs e)
        {
            this.canShow = false;
            this.UpdateFormLocation();
        }

        private void Frm_Resize(object sender, EventArgs e)
        {
            this.UpdateFormLocation();
        }

        private void ToolTipForm_MouseMove(object sender, MouseEventArgs e)
        {
            this.Frm_Resize(null, null);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            e.Graphics.DrawRectangle(Pens.Black, new Rectangle(0, 0, this.Width - 1, this.Height - 1));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.DrawString(this.currentMessage, this.Font, Brushes.Black, new PointF(4f, 4f));
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams baseParams = base.CreateParams;

                const int WS_EX_NOACTIVATE = 0x08000000;
                const int WS_EX_TOOLWINDOW = 0x00000080;
                baseParams.ExStyle |= WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW;

                return baseParams;
            }
        }

        protected override bool ShowWithoutActivation => true;

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    }
}
