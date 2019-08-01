using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GameLibrary.GUI.Controls
{
    public class Status : Control
    {
        private Brush _statusBrush;

        public Status()
        {
            StatusBrush = Brushes.LightGray;
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }

        public Brush StatusBrush
        {
            get => _statusBrush;
            set
            {
                _statusBrush = value;
                Refresh();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _statusBrush != null)
            {
                _statusBrush.Dispose();
                _statusBrush = null;
            }

            base.Dispose(disposing);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.FillEllipse(StatusBrush, new Rectangle(new Point(0, 0), Size));
            base.OnPaint(e);
        }
    }
}