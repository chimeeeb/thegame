using System;
using System.Drawing;
using System.Windows.Forms;
using GameLibrary.Enum;
using GameLibrary.GUI;
using GameLibrary.Interface;

namespace Player.GUI
{
    public class MapWindow : MapWindowBase
    {
        private Agent _agent;

        private Label _teamDescription;

        public MapWindow(Agent agent)
        {
            _agent = agent;

            var teamDescriptionSize = new Size(Size.Width / 2, 50);
            var teamDescriptionLocation = new Point(Size.Width / 2, 0);

            _teamDescription = new Label();
            _teamDescription.Size = teamDescriptionSize;
            _teamDescription.Location = teamDescriptionLocation;
            _teamDescription.Font = Font1;
            _teamDescription.TextAlign = ContentAlignment.MiddleCenter;
            _teamDescription.ForeColor = ForeColor1;
            Controls.Add(_teamDescription);

            VisibleChanged += delegate(object sender, EventArgs e) 
            {
                if (!Visible) return;
                UpdateAgentInfo();
                _agent.ServerDisconnected += ServerDisconnected;
                _agent.GameEnded += GameEnded;
            };

            FormClosed += delegate
            {
                agent.ServerDisconnected -= ServerDisconnected;
                agent.GameEnded -= GameEnded;
            };
        }

        public override void PrepareWindow()
        {
            IsPlaying = true;
            UpdateAgentInfo();
            DrawMap(_agent.Map);
        }

        private void UpdateAgentInfo()
        {
            Description.Text = $"Agent {_agent.Id}";
            if (_agent.IsLeader) Description.Text += " ♛";
            _teamDescription.Text = $"Team {_agent.Team:G}";
        }

        protected override void DrawTile(ITile tile, int x, int y)
        {
            var agentTile = tile as Tile;
            if (agentTile == null) throw new ArgumentException($"Expected tile of type: {typeof(Tile)}");
            var tileAsset = TileAssets[tile.Type];
            GMap.DrawImage(tileAsset, x, y, TileSize, TileSize);
            if (tile == _agent.Tile)
            {
                if (_agent.Team == Team.None)
                    throw new ArgumentException("Tried to draw a player without team");
                DrawImageOnTile(_agent.Team == Team.Blue ? BluePlayerAsset : RedPlayerAsset, x, y, 0.75f);
            }
            if (agentTile.DistanceToPiece == 0)
                DrawImageOnTile(RealPieceAsset, x, y, 0.75f);
            else if (agentTile.DistanceToPiece > 0)
                DrawStringOnTile(agentTile.DistanceToPiece == int.MaxValue ?
                    "INF" : agentTile.DistanceToPiece.ToString(), x, y);
        }

        protected override void Render()
        {
            if (_agent.Map != null && IsPlaying)
                DrawMap(_agent.Map);
            if (InGameLogsPoint != null && InGameLogsPoint != Point.Empty)
                DrawLog(string.Join("\n", InGameLogList), InGameLogsPoint.X, InGameLogsPoint.Y);
            MapContainer.Refresh();
        }

        protected override void Update()
        {
        }

        private void ServerDisconnected()
        {
            if (!IsPlaying) return;
            Invoke((MethodInvoker)delegate ()
            {
                var result = MessageBox.Show("Server disconnected. Game will be closed", "Server disconnected",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (result == DialogResult.OK)
                    Dispose(true);
            });
        }

        private void GameDisconnected()
        {
            if (!IsPlaying) return;
            Invoke((MethodInvoker)delegate ()
            {
                var result = MessageBox.Show("Game disconnected and will be closed", "Game disconnected",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (result == DialogResult.OK)
                    Dispose(true);
            });
        }

        private void GameEnded(Team winningTeam)
        {
            if (!IsPlaying) return;
            Invoke((MethodInvoker)delegate ()
            {
                IsPlaying = false;
                WriteLogInGame($"Team {winningTeam:G} won.", TimeSpan.FromSeconds(5));
            });
        }
    }
}