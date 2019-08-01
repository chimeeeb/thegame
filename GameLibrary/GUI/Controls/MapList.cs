using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GameLibrary.GUI.Controls
{
    public class MapList : ComboBox
    {
        private const int WM_PAINT = 0xF;
        private int buttonWidth = SystemInformation.HorizontalScrollBarArrowWidth;

        public Color BorderColor { get; set; } = Color.Black;
        public Color ArrowBoxColor { get; set; } = Color.LightGray;
        public Color ArrowColor { get; set; } = Color.Black;

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_PAINT)
            {
                using (var g = Graphics.FromHwnd(Handle))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    using (var p = new Pen(BorderColor, 5))
                    {
                        g.DrawRectangle(p, 0, 0, Width - 1, Height - 1);
                        g.DrawLine(p, Width - buttonWidth, 0, Width - buttonWidth, Height);
                    }

                    var arrowBoxP1 = new Point(Width - buttonWidth + 2, 2);
                    var arrowBoxP2 = new Point(Width - 2, Height - 2);
                    var arrowBoxSize = new Size(arrowBoxP2.X - arrowBoxP1.X, arrowBoxP2.Y - arrowBoxP1.Y);
                    var arrowBoxCenter = new Point(arrowBoxP1.X + arrowBoxSize.Width / 2,
                        arrowBoxP1.Y + arrowBoxSize.Height / 2);

                    using (var p = new SolidBrush(ArrowBoxColor))
                    {
                        g.FillRectangle(p, arrowBoxP1.X, arrowBoxP1.Y, arrowBoxP2.X, arrowBoxP2.Y);
                    }

                    using (var p = new SolidBrush(ArrowColor))
                    {
                        g.FillClosedCurve(p, new[]
                        {
                            new Point(arrowBoxCenter.X - 4, arrowBoxCenter.Y - 3),
                            new Point(arrowBoxCenter.X + 4, arrowBoxCenter.Y - 3),
                            new Point(arrowBoxCenter.X, arrowBoxCenter.Y + 3)
                        });
                    }
                }
            }
        }
    }
}