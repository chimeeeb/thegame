using System.Drawing;
using System.Windows.Forms;

namespace GameLibrary.GUI.Controls
{
    public class Line : Control
    {
        private int _lineWidth;
        private Color _lineColor;
        private Brush _linePen;

        public Line()
        {
            LineColor = Color.LightGray;
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }

        public Color LineColor
        {
            get => _lineColor;
            set
            {
                _lineColor = value;
                _linePen = new SolidBrush(_lineColor);
                Refresh();
            }
        }

        public int LineWidth
        {
            get => _lineWidth;
            set
            {
                _lineWidth = value;
                Refresh();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _linePen != null)
            {
                _linePen.Dispose();
                _linePen = null;
            }

            base.Dispose(disposing);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(_linePen, 0, 0, Size.Width, Size.Height);
            base.OnPaint(e);
        }
    }
}