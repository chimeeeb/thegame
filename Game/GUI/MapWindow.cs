using System;
using System.Drawing;
using GameLibrary.Enum;
using GameLibrary.GUI;
using GameLibrary.Interface;
using System.Windows.Forms;

namespace Game.GUI
{
    public class MapWindow : MapWindowBase
    {
        private GameMaster _gameMaster;
        private MapContainer _mapContainer;

        public MapWindow(GameMaster gm)
        {
            _gameMaster = gm;
            _mapContainer = new MapContainer();
            Description.TextAlign = ContentAlignment.MiddleCenter;

            VisibleChanged += delegate
            {
                if (!Visible) return;
                _gameMaster.PlayerDisconnected += PlayerDisconnected;
                _gameMaster.ServerDisconnected += ServerDisconnected;
                _gameMaster.GameEnded += GameEnded;
            };

            FormClosed += delegate
            {
                _gameMaster.PlayerDisconnected -= PlayerDisconnected;
                _gameMaster.ServerDisconnected -= ServerDisconnected;
                _gameMaster.GameEnded -= GameEnded;
            };
        }
        
        protected override void DrawTile(ITile tile, int x, int y)
        {
            var gmTile = tile as Tile;
            if (gmTile == null) throw new ArgumentException($"Excepted tile of type: {typeof(Tile)}");
            var tileAsset = TileAssets[tile.Type];
            GMap.DrawImage(tileAsset, x, y, TileSize, TileSize);
            if (gmTile.AgentId >= 0)
            {
                DrawImageOnTile(_gameMaster.Agents[gmTile.AgentId].Team == Team.Blue ? BluePlayerAsset : RedPlayerAsset, x, y, 0.75f);
                DrawStringOnTile(gmTile.AgentId.ToString(), x, y);
            }
            if (gmTile.Piece != Piece.Null)
                DrawImageOnTile(gmTile.Piece == Piece.Real ? 
                    RealPieceAsset : FakePieceAsset, x, y, 0.75f);
        }

        public override void PrepareWindow()
        {
            IsPlaying = true;
            _mapContainer.Maps.Add(_gameMaster.Map);
        }

        protected override void Render()
        {
            if(_gameMaster.Map != null && IsPlaying)
                DrawMap(_gameMaster.Map);
            if(InGameLogsPoint != null && InGameLogsPoint != Point.Empty)
                DrawLog(string.Join("\n", InGameLogList), InGameLogsPoint.X, InGameLogsPoint.Y);
            MapContainer.Refresh();
        }

        protected override void Update()
        {
            Description.Text = $"Blue {_gameMaster.BlueTeamPoints}:{_gameMaster.RedTeamPoints} Red";
        }

        private void PlayerDisconnected(int id)
        {
            if (!IsPlaying) return;
            Invoke((MethodInvoker)delegate ()
            {
                WriteLogInGame($"Agent {id} disconnected.", TimeSpan.FromSeconds(5));
            });
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

        private void GameEnded(Team winningTeam, int blueTeamPoints, int redTeamPoints)
        {
            if (!IsPlaying) return;
            Invoke((MethodInvoker)delegate ()
            {
                IsPlaying = false;
                WriteLogInGame($"Team {winningTeam:G} won. Result: Blue {blueTeamPoints}:{redTeamPoints} Red.", TimeSpan.FromSeconds(5));
            });
        }
    }
}