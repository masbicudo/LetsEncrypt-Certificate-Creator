using System.Drawing;

namespace LetsEncryptAcmeReg
{
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