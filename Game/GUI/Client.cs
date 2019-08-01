using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GameLibrary.GUI;
using GameLibrary.Configuration;
using GameLibrary.GUI.Controls;
using GameLibrary.Enum;

namespace Game.GUI
{
    public class Client : ClientBase
    {
        private readonly GameMaster _gameMaster;

        private static int _matchmakingWidth = 300;
        private static int _defaultWidth;
        private Line _separator;
        private BindingList<PlayerInfo> _playersList;
        private DataGridView _playersGrid;

        public Client(GameMaster gm, GameSettings settings)
        {
            _gameMaster = gm;
            MapWindow = new MapWindow(gm);
            SettingsWindow = new Settings(settings);
            SettingsWindow.FormClosing += delegate
            {
                if (SettingsWindow.DialogResult == DialogResult.OK)
                    SettingsModified();
            };
            _gameMaster.PlayersReady += PlayersReady;
            _gameMaster.PlayersNotReady += PlayersNotReady;
            _gameMaster.GameEnded += GameEnded;
            _gameMaster.PlayerConnected += PlayerConnected;
            _gameMaster.PlayerDisconnected += PlayerDisconnected;
            _gameMaster.ServerDisconnected += ServerDisconnected;
        }

        protected override void InitializeComponent()
        {
            base.InitializeComponent();
            _defaultWidth = Width;
            _matchmakingWidth += Width;
            
            StartButton.Text = "Create game";

            _separator = new Line();
            _separator.Size = new Size(2, ClientSize.Height);
            _separator.Location = new Point(ClientSize.Width, 0);
            _separator.LineWidth = 2;
            _separator.LineColor = BackColor3;

            _playersList = new BindingList<PlayerInfo>();

            _playersGrid = new DataGridView();
            _playersGrid.Size = new Size(_matchmakingWidth - _defaultWidth - 10, ClientSize.Height - 10);
            _playersGrid.Location = new Point(ClientSize.Width + 5, 5);
            _playersGrid.DataSource = _playersList;
            _playersGrid.ReadOnly = true;
            _playersGrid.Enabled = false;
            _playersGrid.RowHeadersVisible = false;
            _playersGrid.BackgroundColor = BackColor2;
            _playersGrid.ScrollBars = ScrollBars.None;
            _playersGrid.AllowUserToAddRows = false;
        }

        protected override string GetStartButtonText() => "Create game";

        protected override void SettingsModified()
        {
            base.SettingsModified();
            _gameMaster.Settings = SettingsWindow.GetSettings() as GameSettings;
            _gameMaster.ResetGame();
        }

        private void PlayersReady()
        {
            Invoke((MethodInvoker)delegate ()
            {
                StartButton.Enabled = true;
            });
        }

        private void PlayersNotReady()
        {
            Invoke((MethodInvoker)delegate ()
            {
                StartButton.Enabled = false;
            });
        }

        protected override void ConnectToGame()
        {
            base.ConnectToGame();
            if (ConnectState == ConnectState.Connecting)
            {
                if (_gameMaster.ConnectClientToServer())
                {
                    _gameMaster.ConnectToServer();
                    ChangeConnectState(ConnectState.Connected);
                    OpenMatchmakingTab();
                }
                else
                {
                    MessageBox.Show("Game not found", "Connecting error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ChangeConnectState(ConnectState.Disconnected);
                }
            }
            else if (ConnectState == ConnectState.Disconnected)
            {
                CloseMatchmakingTab();
                _gameMaster.Disconnect();
            }
            StartButton.Enabled = false;
        }

        protected override void StartGame()
        {
            base.StartGame();
            GameState = GameState.Started;
            _gameMaster.StartGame();
            Invoke((MethodInvoker)delegate ()
            {
                if (MapWindow != null)
                {
                    MapWindow.PrepareWindow();
                    MapWindow.Show(this);
                }
            });
        }

        private void OpenMatchmakingTab()
        {
            Controls.Add(_separator);
            _playersList.Clear();
            Controls.Add(_playersGrid);
            Width = _matchmakingWidth;
        }

        private void CloseMatchmakingTab()
        {
            Width = _defaultWidth;
            _playersList.Clear();
            Controls.Remove(_playersGrid);
            Controls.Remove(_separator);
        }

        private void PlayerConnected(PlayerInfo playerInfo)
        {
            Invoke((MethodInvoker)delegate ()
            {
                _playersList.Add(playerInfo);
            });
        }

        private void PlayerDisconnected(int id)
        {
            Invoke((MethodInvoker)delegate ()
            {
                var playersToRemove = _playersList.Where(player => player.Id == id);
                foreach (var player in playersToRemove.ToList())
                    _playersList.Remove(player);
            });
        }

        private void ServerDisconnected()
        {
            Invoke((MethodInvoker)delegate ()
            {
                ChangeConnectState(ConnectState.Disconnected);
                GameState = GameState.Stopped;
                CloseMatchmakingTab();
            });
        }

        private void GameEnded(Team winningTeam, int blueTeamPoints, int redTeamPoints)
        {
            Invoke((MethodInvoker)delegate ()
            {
                GameState = GameState.Stopped;
                CloseMatchmakingTab();
            });
        }
    }
}
