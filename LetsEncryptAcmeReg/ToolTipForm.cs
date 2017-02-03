using LetsEncryptAcmeReg.WinAPI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace LetsEncryptAcmeReg
{
    public partial class ToolTipForm : Form
    {
        private readonly ToolTipManager manager;
        private readonly Control control;
        private string currentMessage;
        private MessagePart[] messageItems;
        private Func<bool> shouldShow;
        private bool isMouseOverControl;
        private bool isActiveForm;
        private SizeF currentTextSize;
        private bool canShow;
        private bool useMarkdown;
        private Point? fixedLocation;

        public ToolTipForm(ToolTipManager manager, Control control)
        {
            this.manager = manager;
            this.control = control;
            InitializeComponent();
        }

        public string PositionPreferences { get; set; } = ">,v,^,<,>v,>^,<v,<^";
        public new int Margin { get; set; } = -1;
        public new int Padding { get; set; } = 6;
        public Color BorderColor { get; set; } = Color.LightSlateGray;
        public int Priority { get; set; }

        /// <summary>
        /// Indicates that the tool tip should show up automatically when the mouse is over the designated control.
        /// </summary>
        /// <param name="message">The message to display when the mouse is over the control.</param>
        public ToolTipForm AutoPopup(string message, bool useMarkdown = false)
        {
            this.useMarkdown = useMarkdown;
            this.shouldShow = this.ShouldShowAutoPopup;

            bool invalidate = message != this.currentMessage;

            if (invalidate)
                this.InitMessage(message);

            this.control.MouseEnter -= Control_MouseEnter;
            this.control.MouseHover -= Control_MouseHover;
            this.control.MouseLeave -= Control_MouseLeave;
            if (!string.IsNullOrWhiteSpace(message))
            {
                this.control.MouseEnter += Control_MouseEnter;
                this.control.MouseHover += Control_MouseHover;
                this.control.MouseLeave += Control_MouseLeave;
            }
            else
            {
                this.Hide();
            }

            return this;
        }

        private bool ShouldShowAutoPopup()
        {
            return this.isMouseOverControl;
        }

        public ToolTipForm ShowMessage(string message, bool useMarkdown = false)
        {
            this.fixedLocation = null;
            this.useMarkdown = useMarkdown;
            this.shouldShow = this.ShouldShowFixed;
            this.ShowMessageInternal(message);
            return this;
        }

        public ToolTipForm ShowMessageAt(string message, Point location, bool useMarkdown = false)
        {
            this.fixedLocation = location;
            this.useMarkdown = useMarkdown;
            this.shouldShow = this.ShouldShowFixed;
            this.ShowMessageInternal(message);
            return this;
        }

        protected virtual void ShowMessageInternal(string message)
        {
            bool invalidate = message != this.currentMessage;

            if (invalidate)
                this.InitMessage(message);

            this.UpdateFormLocation();
            this.ShowOrHide();

            if (invalidate)
                this.Invalidate();
        }

        private bool ShouldShowFixed()
        {
            return this.isActiveForm;
        }

        private void InitMessage(string message)
        {
            this.currentMessage = message;
            SizeF size;
            if (string.IsNullOrWhiteSpace(message))
            {
                size = SizeF.Empty;
                this.messageItems = null;
            }
            else
            {
                using (var g = this.CreateGraphics())
                {
                    if (this.useMarkdown)
                    {
                        this.messageItems = this.GetMarkDownMessageParts(message, g, out size);
                    }
                    else
                    {
                        var format = StringFormat.GenericTypographic;
                        format.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
                        size = g.MeasureString(message, this.Font, new PointF(0f, 0f), format);
                        this.messageItems = new[] { new MessagePart { Text = message, Size = size } };
                    }
                }
            }

            this.currentTextSize = size;
        }

        private MessagePart[] GetMarkDownMessageParts(string message, Graphics g, out SizeF size)
        {
            var list = new List<MessagePart>();
            var parts = Regex.Split(message, @"(`|\*\*|\*|_)", RegexOptions.None);
            var lineCount = 0;
            bool isCode = false;
            bool isBold = false;
            bool isItalic = false;
            bool isUnderline = false;
            for (int i = 0; i < parts.Length; i++)
            {
                var text = parts[i];
                if (text == "_" && !isCode) isUnderline = !isUnderline;
                else if (text == "`") isCode = !isCode;
                else if (text == "**") isBold = !isBold;
                else if (text == "*") isItalic = !isItalic;
                else
                {
                    var lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

                    for (int index = 0; index < lines.Length; index++)
                    {
                        var line = lines[index];

                        if (index > 0) lineCount++;

                        if (string.IsNullOrEmpty(line))
                            continue;

                        var part = new MessagePart
                        {
                            Line = lineCount,
                            Text = line,
                        };

                        // font
                        if (isBold && isItalic) part.Font = new Font(this.Font, FontStyle.Bold | FontStyle.Italic);
                        else if (isBold) part.Font = new Font(this.Font, FontStyle.Bold);
                        else if (isItalic) part.Font = new Font(this.Font, FontStyle.Italic);
                        else part.Font = this.Font;

                        // back-color
                        if (isCode) part.BackBrush = Brushes.LightGray;

                        // underline
                        if (isUnderline) part.UnderlinePen = Pens.DarkSlateGray;

                        list.Add(part);
                    }
                }
            }

            var prevLine = 0;
            float x = 0f;
            float width = 0f;
            float height = 0f;
            float lineSpace = this.Font.GetHeight(g);
            foreach (var part in list)
            {
                float y = part.Line * lineSpace;
                if (prevLine != part.Line)
                {
                    x = 0f;
                    prevLine = part.Line;
                }

                part.Location = new PointF(x, y);
                //g.TextRenderingHint = TextRenderingHint.AntiAlias;
                var format = StringFormat.GenericTypographic;
                format.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
                part.Size = g.MeasureString(part.Text, part.Font, new PointF(0f, 0f), format);
                if (x + part.Size.Width > width)
                    width = x + part.Size.Width;
                if (y + part.Size.Height > height)
                    height = y + part.Size.Height;

                x += part.Size.Width;
            }
            size = new SizeF(width, height);
            return list.ToArray();
        }

        private void Control_MouseHover(object sender, EventArgs e)
        {
            this.isMouseOverControl = true;
            this.ShowMessageInternal(this.currentMessage);
        }

        private void Control_MouseLeave(object sender, EventArgs e)
        {
            this.isMouseOverControl = false;
            this.Hide();
        }

        private void Control_MouseEnter(object sender, EventArgs e)
        {
            this.isMouseOverControl = true;
            this.ShowMessageInternal(this.currentMessage);
        }

        private void UpdateFormLocation()
        {
            var size = this.currentTextSize.ToSize();

            var cw = this.fixedLocation == null ? this.control.Width : 0;
            var ch = this.fixedLocation == null ? this.control.Height : 0;

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

                { "v>", new Rectangle(+cw + 2*margin, +ch + 2*margin, tw, th) }, // >v
                { "^>", new Rectangle(+cw + 2*margin, -th - 2*margin, tw, th) }, // >^
                { "v<", new Rectangle(-tw - 2*margin, +ch + 2*margin, tw, th) }, // <v
                { "^<", new Rectangle(-tw - 2*margin, -th - 2*margin, tw, th) }, // <^
            };

            var rects = order.Where(x => rectsDic.ContainsKey(x)).Select(x => rectsDic[x]).ToArray();

            var ownerHandle = GetRootWindow(this.control.Handle);
            var myHandle = GetRootWindow(this.Handle);

            int max = int.MinValue;
            Rectangle choice = Rectangle.Empty;
            if (size != Size.Empty)
            {
                var others = this.manager.GetToolTipsFor(this.control)
                    .Where(tt => tt != this)
                    .Where(tt => tt.Visible)
                    .Where(tt => tt.Priority < this.Priority)
                    .ToArray();

                var baseScore = -0;

                foreach (var r2 in rects)
                {
                    var r = this.fixedLocation == null ? r2 : new Rectangle(this.fixedLocation.Value + (Size)r2.Location, r2.Size);

                    Vec2 a = this.control.PointToScreen(new Point(r.Left, r.Top));
                    Vec2 b = this.control.PointToScreen(new Point(r.Left, r.Bottom));
                    Vec2 c = this.control.PointToScreen(new Point(r.Right, r.Top));
                    Vec2 d = this.control.PointToScreen(new Point(r.Right, r.Bottom));
                    var sa = Screen.FromPoint(a);
                    var sb = Screen.FromPoint(b);
                    var sc = Screen.FromPoint(c);
                    var sd = Screen.FromPoint(d);

                    var rect = new Rectangle(a, d - a);

                    var val = baseScore;

                    val += sa.Bounds.Contains(a) ? 40 : 0;
                    val += sb.Bounds.Contains(b) ? 40 : 0;
                    val += sc.Bounds.Contains(c) ? 40 : 0;
                    val += sd.Bounds.Contains(d) ? 40 : 0;

                    val += sa.DeviceName == sb.DeviceName ? 10 : 0;
                    val += sa.DeviceName == sc.DeviceName ? 10 : 0;
                    val += sa.DeviceName == sd.DeviceName ? 10 : 0;

                    // counting the number of other tooltip forms under the current region
                    var overlapArea = others.Sum(tt =>
                    {
                        var i = rect;
                        i.Intersect(tt.Bounds);
                        return i.Width * i.Height;
                    });
                    val -= overlapArea >> 5;

                    var wndLT = GetRootWindow(User32.WindowFromPoint(a));
                    var wndLB = GetRootWindow(User32.WindowFromPoint(b - new Size(0, 1)));
                    var wndRT = GetRootWindow(User32.WindowFromPoint(c - new Size(1, 0)));
                    var wndRB = GetRootWindow(User32.WindowFromPoint(d - new Size(1, 1)));
                    var wndL = GetRootWindow(User32.WindowFromPoint((Point)(0.5 * (a + b))));
                    var wndT = GetRootWindow(User32.WindowFromPoint((Point)(0.5 * (a + c))));
                    var wndR = GetRootWindow(User32.WindowFromPoint((Point)(0.5 * (c + d))));
                    var wndB = GetRootWindow(User32.WindowFromPoint((Point)(0.5 * (b + d))));

                    val += ownerHandle == wndLT ? 4 : 0;
                    val += ownerHandle == wndLB ? 4 : 0;
                    val += ownerHandle == wndRT ? 4 : 0;
                    val += ownerHandle == wndRB ? 4 : 0;
                    val += ownerHandle == wndL ? 4 : 0;
                    val += ownerHandle == wndB ? 4 : 0;
                    val += ownerHandle == wndT ? 4 : 0;
                    val += ownerHandle == wndR ? 4 : 0;

                    val += myHandle == wndLT ? 4 : 0;
                    val += myHandle == wndLB ? 4 : 0;
                    val += myHandle == wndRT ? 4 : 0;
                    val += myHandle == wndRB ? 4 : 0;
                    val += myHandle == wndL ? 4 : 0;
                    val += myHandle == wndB ? 4 : 0;
                    val += myHandle == wndT ? 4 : 0;
                    val += myHandle == wndR ? 4 : 0;

                    // avoid mouse cursor
                    val += !rect.Contains(Cursor.Position) ? 16 : 0;

                    if (val > max)
                    {
                        max = val;
                        choice = rect;
                    }

                    baseScore += baseScore / 5 - 8;
                }
            }

            this.canShow = max > 0 && choice != Rectangle.Empty;
            if (this.canShow)
            {
                if (this.Location != choice.Location)
                    this.Location = choice.Location;
                if (this.Size != choice.Size)
                    this.Size = choice.Size;
            }
        }

        private void ShowOrHide()
        {
            var frm = this.control.FindForm();

            var active = Form.ActiveForm;
            this.isActiveForm = active == frm || (active as ToolTipForm)?.control.FindForm() == frm;

            if (this.canShow && this.shouldShow?.Invoke() != false)
            {
                if (!this.Visible)
                {
                    if (frm != null)
                    {
                        frm.Enter -= Frm_Activated;
                        frm.GotFocus -= Frm_Activated;
                        frm.Activated -= Frm_Activated;
                        frm.Deactivate -= Frm_Deactivate;
                        frm.Resize -= Frm_Resize;
                        frm.Move -= Frm_Resize;

                        frm.Enter += Frm_Activated;
                        frm.GotFocus += Frm_Activated;
                        frm.Activated += Frm_Activated;
                        frm.Deactivate += Frm_Deactivate;
                        frm.Resize += Frm_Resize;
                        frm.Move += Frm_Resize;
                    }

                    this.ShowInactiveTopmost();

                    // moving other tooltips out of the way
                    var othersToMove = this.manager.GetToolTipsFor(this.control)
                        .Where(tt => tt != this)
                        .Where(tt => tt.Visible)
                        .Where(tt => tt.Priority > this.Priority)
                        .ToArray();

                    foreach (var toolTipForm in othersToMove)
                        toolTipForm.UpdateFormLocation();
                }
            }
            else
            {
                if (this.Visible)
                {
                    if (frm != null)
                    {
                        frm.Enter -= Frm_Activated;
                        frm.GotFocus -= Frm_Activated;
                        frm.Activated -= Frm_Activated;
                        frm.Deactivate -= Frm_Deactivate;
                        frm.Resize -= Frm_Resize;
                        frm.Move -= Frm_Resize;
                    }
                    this.Hide();
                }
            }
        }

        private void Frm_Activated(object sender, EventArgs e)
        {
            if (this.Visible)
                this.UpdateFormLocation();
            //this.ShowOrHide();
        }

        private void Frm_Deactivate(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void Frm_Resize(object sender, EventArgs e)
        {
            if (this.Visible)
                this.UpdateFormLocation();
            //this.ShowOrHide();
        }

        private void ToolTipForm_MouseMove(object sender, MouseEventArgs e)
        {
            this.Frm_Resize(null, null);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            using (var pen = new Pen(this.BorderColor, 3f))
                e.Graphics.DrawRectangle(pen, new Rectangle(1, 1, this.Width - 3, this.Height - 3));
            e.Graphics.DrawRectangle(Pens.Black, new Rectangle(0, 0, this.Width - 1, this.Height - 1));

            var padding = new SizeF(this.Padding, this.Padding);
            if (this.messageItems != null)
                foreach (var messageItem in this.messageItems)
                {
                    var point = messageItem.Location + padding;
                    if (messageItem.BackBrush != null && messageItem.Size != SizeF.Empty)
                        e.Graphics.FillRectangle(messageItem.BackBrush, new RectangleF(point, messageItem.Size));
                }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var padding = new SizeF(this.Padding, this.Padding);
            if (this.messageItems != null)
                foreach (var messageItem in this.messageItems)
                {
                    var point = messageItem.Location + padding;
                    if (messageItem.UnderlinePen != null)
                    {
                        float lineSpace = this.Font.FontFamily.GetLineSpacing(Font.Style);
                        float ascent = this.Font.FontFamily.GetCellAscent(Font.Style);
                        var baseLine = Font.GetHeight(e.Graphics) * ascent / lineSpace;
                        var pu0 = point + new SizeF(0f, baseLine + messageItem.UnderlinePen.Width);
                        var pu1 = point + new SizeF(messageItem.Size.Width, baseLine + messageItem.UnderlinePen.Width);
                        e.Graphics.DrawLine(messageItem.UnderlinePen, pu0, pu1);
                    }
                    //e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                    var format = StringFormat.GenericTypographic;
                    format.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
                    e.Graphics.DrawString(messageItem.Text, messageItem.Font ?? this.Font, messageItem.TextBrush ?? Brushes.Black, point, StringFormat.GenericTypographic);
                }
        }

        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        CreateParams baseParams = base.CreateParams;
        //        var exstyle = (WindowStyles)baseParams.ExStyle;
        //        exstyle |= WindowStyles.WS_EX_NOACTIVATE | WindowStyles.WS_EX_TOOLWINDOW | WindowStyles.WS_EX_PALETTEWINDOW;
        //        baseParams.ExStyle = (int)exstyle;
        //        return baseParams;
        //    }
        //}

        //protected override bool ShowWithoutActivation => true;

        void ShowInactiveTopmost()
        {
            const int HWND_TOPMOST = -1;
            const uint SWP_NOACTIVATE = 0x0010;

            var active = Form.ActiveForm;

            var frm = this;
            User32.ShowWindow(frm.Handle, ShowWindowCommands.ShowNoActivate);
            User32.SetWindowPos(frm.Handle.ToInt32(), HWND_TOPMOST,
                frm.Left, frm.Top, frm.Width, frm.Height,
                SWP_NOACTIVATE);

            if (active != null)
                this.control.FindForm()?.Activate();
        }

        private IntPtr GetRootWindow(IntPtr hWnd)
        {
            var parent = User32.GetParent(hWnd);
            return parent == IntPtr.Zero ? hWnd : GetRootWindow(parent);
        }
    }

    public struct Vec2
    {
        public int X { get; }
        public int Y { get; }

        public Vec2(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public static implicit operator Point(Vec2 v) => new Point(v.X, v.Y);
        public static implicit operator Vec2(Point pt) => new Vec2(pt.X, pt.Y);
        public static implicit operator Size(Vec2 v) => new Size(v.X, v.Y);
        public static implicit operator Vec2(Size sz) => new Vec2(sz.Width, sz.Height);
        public static implicit operator Vec2f(Vec2 v) => new Vec2f(v.X, v.Y);
        public static Vec2 operator *(Vec2 a, Vec2 b) => new Vec2(a.X * b.X, a.Y * b.Y);
        public static Vec2 operator *(Point a, Vec2 b) => new Vec2(a.X * b.X, a.Y * b.Y);
        public static Vec2 operator *(Vec2 a, Point b) => new Vec2(a.X * b.X, a.Y * b.Y);
        public static Vec2 operator *(Size a, Vec2 b) => new Vec2(a.Width * b.X, a.Height * b.Y);
        public static Vec2 operator *(Vec2 a, Size b) => new Vec2(a.X * b.Width, a.Y * b.Height);
        public static Vec2 operator *(int a, Vec2 b) => new Vec2(a * b.X, a * b.Y);
        public static Vec2 operator *(Vec2 a, int b) => new Vec2(a.X * b, a.Y * b);
        public static Vec2f operator *(float a, Vec2 b) => new Vec2f(a * b.X, a * b.Y);
        public static Vec2f operator *(Vec2 a, float b) => new Vec2f(a.X * b, a.Y * b);
        public static Vec2f operator *(double a, Vec2 b) => new Vec2f((float)(a * b.X), (float)(a * b.Y));
        public static Vec2f operator *(Vec2 a, double b) => new Vec2f((float)(a.X * b), (float)(a.Y * b));
        public static Vec2 operator +(Vec2 a, Vec2 b) => new Vec2(a.X + b.X, a.Y + b.Y);
        public static Vec2 operator +(Point a, Vec2 b) => new Vec2(a.X + b.X, a.Y + b.Y);
        public static Vec2 operator +(Vec2 a, Point b) => new Vec2(a.X + b.X, a.Y + b.Y);
        public static Vec2 operator +(Size a, Vec2 b) => new Vec2(a.Width + b.X, a.Height + b.Y);
        public static Vec2 operator +(Vec2 a, Size b) => new Vec2(a.X + b.Width, a.Y + b.Height);
        public static Vec2 operator +(int a, Vec2 b) => new Vec2(a + b.X, a + b.Y);
        public static Vec2 operator +(Vec2 a, int b) => new Vec2(a.X + b, a.Y + b);
        public static Vec2f operator +(float a, Vec2 b) => new Vec2f(a + b.X, a + b.Y);
        public static Vec2f operator +(Vec2 a, float b) => new Vec2f(a.X + b, a.Y + b);
        public static Vec2f operator +(double a, Vec2 b) => new Vec2f((float)(a + b.X), (float)(a + b.Y));
        public static Vec2f operator +(Vec2 a, double b) => new Vec2f((float)(a.X + b), (float)(a.Y + b));
        public static Vec2 operator -(Vec2 a, Vec2 b) => new Vec2(a.X - b.X, a.Y - b.Y);
        public static Vec2 operator -(Point a, Vec2 b) => new Vec2(a.X - b.X, a.Y - b.Y);
        public static Vec2 operator -(Vec2 a, Point b) => new Vec2(a.X - b.X, a.Y - b.Y);
        public static Vec2 operator -(Size a, Vec2 b) => new Vec2(a.Width - b.X, a.Height - b.Y);
        public static Vec2 operator -(Vec2 a, Size b) => new Vec2(a.X - b.Width, a.Y - b.Height);
        public static Vec2 operator -(int a, Vec2 b) => new Vec2(a - b.X, a - b.Y);
        public static Vec2 operator -(Vec2 a, int b) => new Vec2(a.X - b, a.Y - b);
        public static Vec2f operator -(float a, Vec2 b) => new Vec2f(a - b.X, a - b.Y);
        public static Vec2f operator -(Vec2 a, float b) => new Vec2f(a.X - b, a.Y - b);
        public static Vec2f operator -(double a, Vec2 b) => new Vec2f((float)(a - b.X), (float)(a - b.Y));
        public static Vec2f operator -(Vec2 a, double b) => new Vec2f((float)(a.X - b), (float)(a.Y - b));
        public static Vec2 operator /(Vec2 a, Vec2 b) => new Vec2(a.X / b.X, a.Y / b.Y);
        public static Vec2 operator /(Point a, Vec2 b) => new Vec2(a.X / b.X, a.Y / b.Y);
        public static Vec2 operator /(Vec2 a, Point b) => new Vec2(a.X / b.X, a.Y / b.Y);
        public static Vec2 operator /(Size a, Vec2 b) => new Vec2(a.Width / b.X, a.Height / b.Y);
        public static Vec2 operator /(Vec2 a, Size b) => new Vec2(a.X / b.Width, a.Y / b.Height);
        public static Vec2 operator /(int a, Vec2 b) => new Vec2(a / b.X, a / b.Y);
        public static Vec2 operator /(Vec2 a, int b) => new Vec2(a.X / b, a.Y / b);
        public static Vec2f operator /(float a, Vec2 b) => new Vec2f(a / b.X, a / b.Y);
        public static Vec2f operator /(Vec2 a, float b) => new Vec2f(a.X / b, a.Y / b);
        public static Vec2f operator /(double a, Vec2 b) => new Vec2f((float)(a / b.X), (float)(a / b.Y));
        public static Vec2f operator /(Vec2 a, double b) => new Vec2f((float)(a.X / b), (float)(a.Y / b));
    }

    public struct Vec2f
    {
        public float X { get; }
        public float Y { get; }

        public Vec2f(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public static explicit operator Point(Vec2f v) => new Point((int)v.X, (int)v.Y);
        public static implicit operator Vec2f(Point pt) => new Vec2f(pt.X, pt.Y);
        public static explicit operator Size(Vec2f v) => new Size((int)v.X, (int)v.Y);
        public static implicit operator Vec2f(Size sz) => new Vec2f(sz.Width, sz.Height);
        public static explicit operator Vec2(Vec2f v) => new Vec2((int)v.X, (int)v.Y);
        public static Vec2f operator *(Vec2f a, Vec2f b) => new Vec2f(a.X * b.X, a.Y * b.Y);
        public static Vec2f operator *(Point a, Vec2f b) => new Vec2f(a.X * b.X, a.Y * b.Y);
        public static Vec2f operator *(Vec2f a, Point b) => new Vec2f(a.X * b.X, a.Y * b.Y);
        public static Vec2f operator *(Size a, Vec2f b) => new Vec2f(a.Width * b.X, a.Height * b.Y);
        public static Vec2f operator *(Vec2f a, Size b) => new Vec2f(a.X * b.Width, a.Y * b.Height);
        public static Vec2f operator *(int a, Vec2f b) => new Vec2f(a * b.X, a * b.Y);
        public static Vec2f operator *(Vec2f a, int b) => new Vec2f(a.X * b, a.Y * b);
        public static Vec2f operator *(float a, Vec2f b) => new Vec2f(a * b.X, a * b.Y);
        public static Vec2f operator *(Vec2f a, float b) => new Vec2f(a.X * b, a.Y * b);
        public static Vec2f operator *(double a, Vec2f b) => new Vec2f((float)(a * b.X), (float)(a * b.Y));
        public static Vec2f operator *(Vec2f a, double b) => new Vec2f((float)(a.X * b), (float)(a.Y * b));
        public static Vec2f operator +(Vec2f a, Vec2f b) => new Vec2f(a.X + b.X, a.Y + b.Y);
        public static Vec2f operator +(Point a, Vec2f b) => new Vec2f(a.X + b.X, a.Y + b.Y);
        public static Vec2f operator +(Vec2f a, Point b) => new Vec2f(a.X + b.X, a.Y + b.Y);
        public static Vec2f operator +(Size a, Vec2f b) => new Vec2f(a.Width + b.X, a.Height + b.Y);
        public static Vec2f operator +(Vec2f a, Size b) => new Vec2f(a.X + b.Width, a.Y + b.Height);
        public static Vec2f operator +(int a, Vec2f b) => new Vec2f(a + b.X, a + b.Y);
        public static Vec2f operator +(Vec2f a, int b) => new Vec2f(a.X + b, a.Y + b);
        public static Vec2f operator +(float a, Vec2f b) => new Vec2f(a + b.X, a + b.Y);
        public static Vec2f operator +(Vec2f a, float b) => new Vec2f(a.X + b, a.Y + b);
        public static Vec2f operator +(double a, Vec2f b) => new Vec2f((float)(a + b.X), (float)(a + b.Y));
        public static Vec2f operator +(Vec2f a, double b) => new Vec2f((float)(a.X + b), (float)(a.Y + b));
        public static Vec2f operator -(Vec2f a, Vec2f b) => new Vec2f(a.X - b.X, a.Y - b.Y);
        public static Vec2f operator -(Point a, Vec2f b) => new Vec2f(a.X - b.X, a.Y - b.Y);
        public static Vec2f operator -(Vec2f a, Point b) => new Vec2f(a.X - b.X, a.Y - b.Y);
        public static Vec2f operator -(Size a, Vec2f b) => new Vec2f(a.Width - b.X, a.Height - b.Y);
        public static Vec2f operator -(Vec2f a, Size b) => new Vec2f(a.X - b.Width, a.Y - b.Height);
        public static Vec2f operator -(int a, Vec2f b) => new Vec2f(a - b.X, a - b.Y);
        public static Vec2f operator -(Vec2f a, int b) => new Vec2f(a.X - b, a.Y - b);
        public static Vec2f operator -(float a, Vec2f b) => new Vec2f(a - b.X, a - b.Y);
        public static Vec2f operator -(Vec2f a, float b) => new Vec2f(a.X - b, a.Y - b);
        public static Vec2f operator -(double a, Vec2f b) => new Vec2f((float)(a - b.X), (float)(a - b.Y));
        public static Vec2f operator -(Vec2f a, double b) => new Vec2f((float)(a.X - b), (float)(a.Y - b));
        public static Vec2f operator /(Vec2f a, Vec2f b) => new Vec2f(a.X / b.X, a.Y / b.Y);
        public static Vec2f operator /(Point a, Vec2f b) => new Vec2f(a.X / b.X, a.Y / b.Y);
        public static Vec2f operator /(Vec2f a, Point b) => new Vec2f(a.X / b.X, a.Y / b.Y);
        public static Vec2f operator /(Size a, Vec2f b) => new Vec2f(a.Width / b.X, a.Height / b.Y);
        public static Vec2f operator /(Vec2f a, Size b) => new Vec2f(a.X / b.Width, a.Y / b.Height);
        public static Vec2f operator /(int a, Vec2f b) => new Vec2f(a / b.X, a / b.Y);
        public static Vec2f operator /(Vec2f a, int b) => new Vec2f(a.X / b, a.Y / b);
        public static Vec2f operator /(float a, Vec2f b) => new Vec2f(a / b.X, a / b.Y);
        public static Vec2f operator /(Vec2f a, float b) => new Vec2f(a.X / b, a.Y / b);
        public static Vec2f operator /(double a, Vec2f b) => new Vec2f((float)(a / b.X), (float)(a / b.Y));
        public static Vec2f operator /(Vec2f a, double b) => new Vec2f((float)(a.X / b), (float)(a.Y / b));
    }
}
