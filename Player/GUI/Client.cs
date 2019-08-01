using System.Windows.Forms;
using GameLibrary.Configuration;
using GameLibrary.Enum;
using GameLibrary.GUI;

namespace Player.GUI
{
    public class Client : ClientBase
    {
        private readonly Agent _agent;

        public Client(Agent agent, AgentSettings settings)
        {
            _agent = agent;
            MapWindow = new MapWindow(agent);
            MapWindow.Disposed += delegate
            {
                ChangeConnectState(ConnectState.Disconnected);
                GameState = GameState.Stopped;
            };
            SettingsWindow = new Settings(settings);
            SettingsWindow.FormClosing += delegate
            {
                if (SettingsWindow.DialogResult == DialogResult.OK)
                    SettingsModified();
            };
            _agent.ServerDisconnected += ServerDisconnected;
            _agent.GameEnded += GameEnded;
        }

        protected override string GetStartButtonText() => "Search game";

        protected override void InitializeComponent()
        {
            base.InitializeComponent();
            StartButton.Text = "Search game";
        }

        protected override void SettingsModified()
        {
            base.SettingsModified();
            _agent.Settings = SettingsWindow.GetSettings() as AgentSettings;
        }

        protected override void ConnectToGame()
        {
            base.ConnectToGame();
            if (ConnectState == ConnectState.Connecting)
            {
                if (_agent.ConnectToServer())
                {
                    ChangeConnectState(ConnectState.Connected);
                }
                else
                {
                    MessageBox.Show("Server not found", "Connecting error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ChangeConnectState(ConnectState.Disconnected);
                }
            }
            else if (ConnectState == ConnectState.Disconnected)
            {
                _agent.Disconnect();
            }
        }

        protected override void StartGame()
        {
            base.StartGame();
            _agent.ConnectResponse += ConnectResponse;
            _agent.ConnectToGame();
        }

        private void ConnectResponse(bool connected)
        {
            if (connected)
            {
                GameState = GameState.WaitingForStart;
                _agent.GameStarted += GameStarted;
            }
            else
            {
                MessageBox.Show("Game not found", "Connecting error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                GameState = GameState.Stopped;
            }
            _agent.ConnectResponse -= ConnectResponse;
        }

        private void GameStarted()
        {
            GameState = GameState.Started;
            _agent.GameStarted -= GameStarted;
            Invoke((MethodInvoker) delegate()
            {
                if (MapWindow != null)
                {
                    MapWindow.PrepareWindow();
                    MapWindow.Show(this);
                }
            });

        }

        private void ServerDisconnected()
        {
            Invoke((MethodInvoker)delegate ()
            {
                ChangeConnectState(ConnectState.Disconnected);
                GameState = GameState.Stopped;
            });
        }

        private void GameDisconnected()
        {
            Invoke((MethodInvoker) delegate()
            {
                GameState = GameState.Stopped;
            });
        }

        private void GameEnded(Team winningTeam)
        {
            Invoke((MethodInvoker)delegate ()
            {
                GameState = GameState.Stopped;
                _agent.GameStarted += GameStarted;
            });
        }
    }
}
