using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using GameLibrary.Enum;
using GameLibrary.Interface;

namespace GameLibrary.GUI
{
    public abstract class MapWindowBase : Form
    {
        protected static readonly Font Font1 = new Font(FontFamily.GenericSansSerif, 20);

        protected static readonly Color BackColor1 = Color.FromArgb(24, 48, 89);
        protected static readonly Color BackColor2 = Color.FromArgb(39, 111, 191);
        protected static readonly Color BackColor3 = Color.Black;
        protected static readonly Color ForeColor1 = Color.White;

        protected PictureBox MapContainer;
        protected Bitmap MapImage;
        protected Graphics GMap;
        protected Label Description;

        protected int TileSize;
        protected Font TileFont;
        protected Point InGameLogsPoint;
        protected Dictionary<TileType, Image> TileAssets;
        protected Image RealPieceAsset;
        protected Image FakePieceAsset;
        protected Image RedPlayerAsset;
        protected Image BluePlayerAsset;

        protected Stopwatch FrameTimer;
        protected bool IsShown;
        protected bool IsPlaying;

        protected List<string> InGameLogList;

        protected const string AssetDirectory = "Assets";

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        protected MapWindowBase()
        {
            InitializeComponent();
        }


        protected virtual void InitializeComponent()
        {
            
            var size = new Size(800, 800);
            var mapContainerSize = new Size(size.Width, size.Height - 50);
            var mapContainerLocation = new Point(0, 50);
            var labelSize = new Size(size.Width, 50);
            var labelLocation = new Point(0, 0);

            TileAssets = new Dictionary<TileType, Image>
            {
                { TileType.Unknown, new Bitmap($"{AssetDirectory}/unknown.png")},
                { TileType.Task, new Bitmap($"{AssetDirectory}/task.png")},
                { TileType.Goal, new Bitmap($"{AssetDirectory}/real-goal.png")},
                { TileType.DiscoveredGoal, new Bitmap($"{AssetDirectory}/discovered-goal.png")},
                { TileType.NoGoal, new Bitmap($"{AssetDirectory}/not-real-goal.png")},
            };
            RealPieceAsset = new Bitmap($"{AssetDirectory}/real-piece.png");
            FakePieceAsset = new Bitmap($"{AssetDirectory}/not-real-piece.png");
            RedPlayerAsset = new Bitmap($"{AssetDirectory}/red-player.png");
            BluePlayerAsset = new Bitmap($"{AssetDirectory}/blue-player.png");

            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = size;
            Text = "GameMap";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = BackColor1;
            
            MapImage = new Bitmap(mapContainerSize.Width, mapContainerSize.Height);
            GMap = Graphics.FromImage(MapImage);

            MapContainer = new PictureBox();
            MapContainer.Size = mapContainerSize;
            MapContainer.Location = mapContainerLocation;
            MapContainer.Image = MapImage;
            Controls.Add(MapContainer);

            Description = new Label();
            Description.Size = labelSize;
            Description.Location = labelLocation;
            Description.Font = Font1;
            Description.TextAlign = ContentAlignment.MiddleLeft;
            Description.ForeColor = ForeColor1;
            Controls.Add(Description);

            InGameLogList = new List<string>();

            VisibleChanged += delegate 
            {
                if (!Visible) return;
                IsShown = true;
            };
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            Owner.Enabled = !Visible;
            base.OnVisibleChanged(e);
            if (!Visible) return;
            if (FrameTimer == null)
                FrameTimer = new Stopwatch();
            FrameTimer.Restart();
            Application.Idle += Tick;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            Owner.Enabled = true;
            FrameTimer.Stop();
            Application.Idle -= Tick;
        }

        public abstract void PrepareWindow();

        public void WriteLogInGame(string log, TimeSpan time)
        {
            Invoke((MethodInvoker)delegate ()
            {
                var timer = new System.Timers.Timer(time.TotalMilliseconds);
                InGameLogList.Add(log);
                timer.Elapsed += delegate
                {
                    InGameLogList.Remove(log);
                };
                timer.AutoReset = false;
                timer.Enabled = true;

            });

        }

        protected void DrawMap<T>(IMap<T> map) where T : class, ITile
        {
            if (!IsShown) return;
            GMap.Clear(BackColor1);
            TileSize = Math.Min(MapImage.Width / map.Width, MapImage.Height / map.Height);
            TileFont = new Font(FontFamily.GenericSansSerif, TileSize / 2f);

            var widthGap = MapImage.Width - TileSize * map.Width;
            int currentX = widthGap / 2;
            int currentY = 0;

            for (int i = 0; i < map.Width; i++)
            {
                currentY = 0;
                for (int j = map.Height-1; j >= 0; j--)
                {
                    DrawTile(map[i, j], currentX, currentY);
                    currentY += TileSize;
                }
                currentX += TileSize;
            }
            InGameLogsPoint = new Point(widthGap / 2, currentY);
        }

        protected abstract void DrawTile(ITile tile, int x, int y);

        protected void DrawStringOnTile(string value, int x, int y)
        {
            var idStringSize = GMap.MeasureString(value, TileFont);
            GMap.DrawString(value, TileFont, Brushes.Black,
                x + (TileSize - idStringSize.Width) / 2,
                y + 3 + (TileSize - idStringSize.Height) / 2);
        }

        protected void DrawImageOnTile(Image image, int x, int y, float scale)
        {
            var imageSize = TileSize * scale;
            var imageTileFrame = (TileSize - imageSize) / 2;

            GMap.DrawImage(image, x + imageTileFrame, y + imageTileFrame, imageSize, imageSize);
        }

        protected void DrawLog(string log, int x, int bottomY)
        {
            var logSize = GMap.MeasureString(log, Font1);
            GMap.DrawString(log, Font1, Brushes.Red, x + 5,
                bottomY - 5 - logSize.Height);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NativeMessage
        {
            public IntPtr Handle;
            public uint Message;
            public IntPtr WParameter;
            public IntPtr LParameter;
            public uint Time;
            public Point Location;
        }
        [DllImport("user32.dll")]
        public static extern int PeekMessage(out NativeMessage message, IntPtr window, uint filterMin, uint filterMax, uint remove);
        /// <summary>
        /// Checks if the window is idle
        /// </summary>
        /// <returns>Returns true if it's idle, otherwise false</returns>
        private bool IsApplicationIdle() => PeekMessage(out var result, IntPtr.Zero, (uint)0, (uint)0, (uint)0) == 0;
        /// <summary>
        /// Frames render loop
        /// </summary>
        private void Tick(object sender, EventArgs e)
        {
            while (IsApplicationIdle())
            {
                TimeSpan diff = FrameTimer.Elapsed - TimeSpan.FromMilliseconds(1000 / 60);
                if(diff.Milliseconds > 0)
                    Thread.Sleep(diff);
                FrameTimer.Restart();

                Update();
                Render();
            }
        }

        protected abstract new void Update();

        protected abstract void Render();

        protected override void Dispose(bool disposing)
        {
            IsShown = false;
            Hide();
        }
    }
}