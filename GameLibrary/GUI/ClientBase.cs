using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using GameLibrary.Enum;
using GameLibrary.GUI.Controls;

namespace GameLibrary.GUI
{
    public class ClientBase : Form
    {
        private ConnectState _connectState;
        private GameState _gameState;
        
        protected static readonly Font Font1 = new Font(FontFamily.GenericSansSerif, 12);

        protected static readonly Color BackColor1 = Color.FromArgb(24, 48, 89);
        protected static readonly Color BackColor2 = Color.FromArgb(39, 111, 191);
        protected static readonly Color BackColor3 = Color.Black;
        protected static readonly Color ForeColor1 = Color.White;

        protected Line Divider;
        protected Button StartButton;
        protected Button ConnectButton;
        protected Bitmap Logo;
        protected Status ConnectionStatus;
        protected Label ConnectionLabel;
        protected Button SettingsButton;

        protected SettingsBase SettingsWindow;
        protected MapWindowBase MapWindow;

        protected ConnectState ConnectState
        {
            get => _connectState;
            set
            {
                _connectState = value;
                if(_connectState != ConnectState.Disconnected)
                    SettingsWindow.ShowReconnectGameInfo = true;
                switch (value)
                {
                    case ConnectState.Disconnected:
                        ConnectionStatus.StatusBrush = Brushes.Red;
                        break;
                    case ConnectState.Connecting:
                        ConnectionStatus.StatusBrush = Brushes.Yellow;
                        break;
                    case ConnectState.Connected:
                        ConnectionStatus.StatusBrush = Brushes.LimeGreen;
                        break;
                }
            }
        }

        protected GameState GameState
        {
            get => _gameState;
            set
            {
                Invoke((MethodInvoker)delegate ()
                {
                    _gameState = value;
                    switch (value)
                    {
                        case GameState.Stopped:
                            StartButton.Enabled = _connectState == ConnectState.Connected;
                            StartButton.Text = GetStartButtonText();
                            break;
                        case GameState.Searching:
                            StartButton.Enabled = false;
                            StartButton.Text = "Searching game...";
                            break;
                        case GameState.WaitingForStart:
                            StartButton.Enabled = false;
                            StartButton.Text = "Waiting for players...";
                            break;
                        case GameState.Started:
                            StartButton.Enabled = false;
                            StartButton.Text = "Game started";
                            break;
                    }
                });
            }
        }

        protected virtual string GetStartButtonText() => "Start game";

        protected ClientBase()
        {
            InitializeComponent();
        }

        protected virtual void InitializeComponent()
        {
            var size = new Size(400, 200);
            var connectionStatusSize = new Size(20, 20);
            var connectionStatusLocation = new Point(5, size.Height - 5 - connectionStatusSize.Height);
            var dividerLocation = new Point(0, size.Height - 50);
            var dividerSize = new Size(size.Width, 2);
            var startButtonSize = new Size(120, 60);
            var startButtonLocation = new Point(size.Width - startButtonSize.Width - 20, 20);
            var connectButtonSize = new Size(startButtonSize.Width, connectionStatusSize.Height * 2);
            var connectButtonLocation = new Point(size.Width - connectButtonSize.Width - 20, connectionStatusLocation.Y - 20);;
            var logoSize = new Size(130, 130);
            var logoLocation = new Point(15, 15);
            var connectionLabelSize = new Size(size.Width - 70 - connectButtonSize.Width, connectionStatusSize.Height);
            var connectionLabelLocation = new Point(connectionStatusLocation.X + connectionStatusSize.Width + 5, connectionStatusLocation.Y);
            var settingsButtonLocation = new Point(startButtonLocation.X,
                startButtonLocation.Y + startButtonSize.Height + 10);
            var settingsButtonSize = new Size(startButtonSize.Width, 30);

            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = size;
            Text = "GameClient";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = BackColor1;

            Divider = new Line();
            Divider.Size = dividerSize;
            Divider.Location = dividerLocation;
            Divider.LineWidth = 2;
            Divider.LineColor = BackColor3;
            Controls.Add(Divider);

            StartButton = new Button();
            StartButton.Size = startButtonSize;
            StartButton.Location = startButtonLocation;
            StartButton.Font = Font1;
            StartButton.BackColor = BackColor2;
            StartButton.ForeColor = ForeColor1;
            StartButton.FlatStyle = FlatStyle.Flat;
            StartButton.FlatAppearance.BorderColor = BackColor2;
            StartButton.Enabled = false;
            Controls.Add(StartButton);

            ConnectButton = new Button();
            ConnectButton.Size = connectButtonSize;
            ConnectButton.Location = connectButtonLocation;
            ConnectButton.Text = "Connect";
            ConnectButton.Font = Font1;
            ConnectButton.BackColor = BackColor2;
            ConnectButton.ForeColor = ForeColor1;
            ConnectButton.FlatStyle = FlatStyle.Flat;
            ConnectButton.FlatAppearance.BorderColor = BackColor2;
            Controls.Add(ConnectButton);

            ConnectionStatus = new Status();
            ConnectionStatus.StatusBrush = Brushes.Red;
            ConnectionStatus.Size = connectionStatusSize;
            ConnectionStatus.Location = connectionStatusLocation;
            Controls.Add(ConnectionStatus);

            ConnectionLabel = new Label();
            ConnectionLabel.Size = connectionLabelSize;
            ConnectionLabel.Location = connectionLabelLocation;
            ConnectionLabel.Text = "Disconnected"; 
            ConnectionLabel.Font = Font1;
            ConnectionLabel.ForeColor = ForeColor1;
            ConnectionLabel.TextAlign = ContentAlignment.MiddleLeft;
            Controls.Add(ConnectionLabel);

            SettingsButton = new Button();
            SettingsButton.Text = "Settings";
            SettingsButton.Size = settingsButtonSize;
            SettingsButton.Location = settingsButtonLocation;
            SettingsButton.ForeColor = ForeColor1;
            SettingsButton.BackColor = BackColor2;
            SettingsButton.ForeColor = ForeColor1;
            SettingsButton.FlatStyle = FlatStyle.Flat;
            SettingsButton.FlatAppearance.BorderColor = BackColor2;
            SettingsButton.Click += delegate { SettingsWindow.ShowDialog(this); };
            Controls.Add(SettingsButton);

            Logo = new Bitmap(logoSize.Width, logoSize.Height);
            using (var g = Graphics.FromImage(Logo))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Center
                };
                var font = new Font(FontFamily.GenericSansSerif, 30);

                g.DrawString("Game", font, Brushes.White, new PointF(logoSize.Width/2f, 15), format);
                g.DrawString("Client", font, Brushes.White, new PointF(logoSize.Width/2f, 55), format);
            }

            var logoContainer = new PictureBox();
            logoContainer.Size = logoSize;
            logoContainer.Location = logoLocation;
            logoContainer.Image = Logo;
            Controls.Add(logoContainer);

            Closing += (o, e) => Application.Exit();
            ConnectButton.Click += delegate { ConnectToGame(); };
            StartButton.Click += delegate { StartGame(); };
        }

        protected virtual void SettingsModified()
        {

        }

        protected virtual void ConnectToGame()
        {
            if(ConnectState == ConnectState.Disconnected)
                ChangeConnectState(ConnectState.Connecting);
            else if (ConnectState == ConnectState.Connected)
            {
                ChangeConnectState(ConnectState.Disconnected);
                GameState = GameState.Stopped;
            }
        }

        protected void ChangeConnectState(ConnectState toState)
        {
            ConnectState = toState;
            switch (toState)
            {
                case ConnectState.Disconnected:
                    ConnectionLabel.Text = "Disconnected";
                    ConnectButton.Text = "Connect";
                    ConnectButton.Enabled = true;
                    StartButton.Enabled = false;
                    break;
                case ConnectState.Connecting:
                    ConnectionLabel.Text = "Connecting...";
                    ConnectButton.Enabled = false;
                    StartButton.Enabled = false;
                    break;
                case ConnectState.Connected:
                    ConnectionLabel.Text = "Connected";
                    ConnectButton.Text = "Disconnect";
                    ConnectButton.Enabled = true;
                    StartButton.Enabled = true;
                    break;
            }
        }

        protected virtual void StartGame()
        {
            GameState = GameState.Searching;
        }

        protected override void Dispose(bool disposing)
        {
            Font1?.Dispose();
            MapWindow?.Dispose();
            base.Dispose(disposing);
        }
    }
}