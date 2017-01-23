﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace LetsEncryptAcmeReg
{
    public partial class ToolTipForm : Form
    {
        private readonly Control control;
        private string currentMessage;
        private MessagePart[] messageItems;
        private Func<bool> shouldShow;
        private bool isMouseOverControl;
        private bool isActiveForm;
        private SizeF currentTextSize;
        private bool canShow;
        private bool useMarkdown;

        public ToolTipForm(Control control)
        {
            this.control = control;
            InitializeComponent();
        }

        public string PositionPreferences { get; set; } = ">,v,^,<,>v,>^,<v,<^";
        public new int Margin { get; set; } = -1;
        public new int Padding { get; set; } = 6;
        public Color BorderColor { get; set; } = Color.LightSlateGray;

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

            var ownerHandle = GetRootWindow(this.control.Handle);

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

                    var wndA = GetRootWindow(WindowFromPoint(a));
                    var wndB = GetRootWindow(WindowFromPoint(b));
                    var wndC = GetRootWindow(WindowFromPoint(c));
                    var wndD = GetRootWindow(WindowFromPoint(d));

                    // must at least be over the owner window
                    if (ownerHandle == wndA || ownerHandle == wndB || ownerHandle == wndC || ownerHandle == wndD)
                        if (val > max && !rect.Contains(Cursor.Position))
                        {
                            max = val;
                            choice = rect;
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
                    ShowWindow(this.Handle, 8);
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
            this.UpdateFormLocation();
            //this.ShowOrHide();
        }

        private void Frm_Deactivate(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void Frm_Resize(object sender, EventArgs e)
        {
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

        private IntPtr GetRootWindow(IntPtr hWnd)
        {
            var parent = GetParent(hWnd);
            return parent == IntPtr.Zero ? hWnd : GetRootWindow(parent);
        }

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern IntPtr WindowFromPoint(Point p);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    }

    internal class MessagePart
    {
        public Font Font { get; set; }
        public string Text { get; set; }
        public Brush BackBrush { get; set; }
        public Brush TextBrush { get; set; }
        public Pen UnderlinePen { get; set; }
        public PointF Location { get; set; }
        public SizeF Size { get; set; }
        public int Line { get; set; }
    }
}
