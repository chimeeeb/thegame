using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using GameLibrary.Configuration;
using GameLibrary.GUI;
using GameLibrary.GUI.Controls;
using Newtonsoft.Json;

namespace Game.GUI
{
    public class Settings : SettingsBase
    {
        private TextBox _serverIpInput;
        private NumericBoxInt
            _serverPortInput,
            _numberOfPlayersInput,
            _mapWidthInput,
            _mapHeightInput,
            _numberOfGoalsInput,
            _pieceGenerationIntervalInput,
            _waitBaseInput,
            _waitMoveInput,
            _waitPickPieceInput,
            _waitTestPieceInput,
            _waitPutPieceInput,
            _waitDestroyPieceInput,
            _waitDiscoveryInput,
            _waitInfoExchangeInput,
            _numberOfPiecesInput,
            _goalAreaHeightInput;
        private NumericBoxDouble
            _probabilityOfBadPieceInput;
        private CheckBox _isLoggingEnabledCheck;
        private Button _okButton, _setDefaultButton, _cancelButton;

        public Settings(GameSettings settings)
        {
            LoadSettings(settings);
        }

        protected override void InitializeComponents()
        {
            Width = 385;
            var settingsFactory = new SettingsInputFactory(
                new Size(100, 25),
                new Size(250, 0),
                new Size(80, 25),
                new Font(FontFamily.GenericSerif, 12),
                10, 25, 5);

            Controls.Add(settingsFactory.GenerateLabel("Number of players:"));
            _numberOfPlayersInput = settingsFactory.GenerateInput<NumericBoxInt, int>(2, 16);
            _numberOfPlayersInput.ValueChanged += delegate
            {
                if (_numberOfPlayersInput.Value % 2 != 0)
                    _numberOfPlayersInput.Value--;
            };
            Controls.Add(_numberOfPlayersInput);
            settingsFactory.NextLine();

            Controls.Add(settingsFactory.GenerateLabel("Map width:"));
            _mapWidthInput = settingsFactory.GenerateInput<NumericBoxInt, int>(5, 50);
            _mapWidthInput.ValueChanged += delegate
            {
                _numberOfPlayersInput.MaxValue = _mapHeightInput.Value * _mapWidthInput.Value - 1;
                _numberOfGoalsInput.MaxValue = _goalAreaHeightInput.Value * _mapWidthInput.Value;
                _numberOfPiecesInput.MaxValue =
                    (_mapHeightInput.Value - 2 * _goalAreaHeightInput.Value) * _mapWidthInput.Value;
            };
            Controls.Add(_mapWidthInput);
            settingsFactory.NextLine();

            Controls.Add(settingsFactory.GenerateLabel("Map height:"));
            _mapHeightInput = settingsFactory.GenerateInput<NumericBoxInt, int>(5, 50);
            _mapHeightInput.ValueChanged += delegate
            {
                _goalAreaHeightInput.MaxValue = _mapHeightInput.Value / 2 - 2;
                _numberOfPlayersInput.MaxValue = _mapHeightInput.Value * _mapWidthInput.Value - 1;
                _numberOfGoalsInput.MaxValue = _goalAreaHeightInput.Value * _mapWidthInput.Value;
                _numberOfPiecesInput.MaxValue =
                    (_mapHeightInput.Value - 2 * _goalAreaHeightInput.Value) * _mapWidthInput.Value;
            };
            Controls.Add(_mapHeightInput);
            settingsFactory.NextLine();

            Controls.Add(settingsFactory.GenerateLabel("Goal area height:"));
            _goalAreaHeightInput = settingsFactory.GenerateInput<NumericBoxInt, int>(1, 3);
            _goalAreaHeightInput.ValueChanged += delegate
            {
                _numberOfGoalsInput.MaxValue = _goalAreaHeightInput.Value * _mapWidthInput.Value;
                _numberOfPiecesInput.MaxValue =
                    (_mapHeightInput.Value - 2 * _goalAreaHeightInput.Value) * _mapWidthInput.Value;
            };
            Controls.Add(_goalAreaHeightInput);
            settingsFactory.NextLine();

            Controls.Add(settingsFactory.GenerateLabel("Number of pieces:"));
            _numberOfPiecesInput = settingsFactory.GenerateInput<NumericBoxInt, int>(2, 100);
            Controls.Add(_numberOfPiecesInput);
            settingsFactory.NextLine();

            Controls.Add(settingsFactory.GenerateLabel("Number of goals per team:"));
            _numberOfGoalsInput = settingsFactory.GenerateInput<NumericBoxInt, int>(2, 100);
            Controls.Add(_numberOfGoalsInput);
            settingsFactory.NextLine();

            Controls.Add(settingsFactory.GenerateLabel("Piece generation time:"));
            _pieceGenerationIntervalInput = settingsFactory.GenerateInput<NumericBoxInt, int>(1, 5000);
            Controls.Add(_pieceGenerationIntervalInput);
            settingsFactory.NextLine();

            Controls.Add(settingsFactory.GenerateLabel("Base time penalty:"));
            _waitBaseInput = settingsFactory.GenerateInput<NumericBoxInt, int>(1, 5000);
            Controls.Add(_waitBaseInput);
            settingsFactory.NextLine();

            Controls.Add(settingsFactory.GenerateLabel("Move time:"));
            _waitMoveInput = settingsFactory.GenerateInput<NumericBoxInt, int>(1, 5000);
            Controls.Add(_waitMoveInput);
            settingsFactory.NextLine();

            Controls.Add(settingsFactory.GenerateLabel("Pick piece time:"));
            _waitPickPieceInput = settingsFactory.GenerateInput<NumericBoxInt, int>(1, 5000);
            Controls.Add(_waitPickPieceInput);
            settingsFactory.NextLine();

            Controls.Add(settingsFactory.GenerateLabel("Put piece time:"));
            _waitPutPieceInput = settingsFactory.GenerateInput<NumericBoxInt, int>(1, 5000);
            Controls.Add(_waitPutPieceInput);
            settingsFactory.NextLine();

            Controls.Add(settingsFactory.GenerateLabel("Test piece time:"));
            _waitTestPieceInput = settingsFactory.GenerateInput<NumericBoxInt, int>(1, 5000);
            Controls.Add(_waitTestPieceInput);
            settingsFactory.NextLine();

            Controls.Add(settingsFactory.GenerateLabel("Destroy piece time:"));
            _waitDestroyPieceInput = settingsFactory.GenerateInput<NumericBoxInt, int>(1, 5000);
            Controls.Add(_waitDestroyPieceInput);
            settingsFactory.NextLine();

            Controls.Add(settingsFactory.GenerateLabel("Discovery time:"));
            _waitDiscoveryInput = settingsFactory.GenerateInput<NumericBoxInt, int>(1, 5000);
            Controls.Add(_waitDiscoveryInput);
            settingsFactory.NextLine();

            Controls.Add(settingsFactory.GenerateLabel("Exchange info time:"));
            _waitInfoExchangeInput = settingsFactory.GenerateInput<NumericBoxInt, int>(1, 5000);
            Controls.Add(_waitInfoExchangeInput);
            settingsFactory.NextLine();

            Controls.Add(settingsFactory.GenerateLabel("Probability of generating bad piece:"));
            _probabilityOfBadPieceInput = settingsFactory.GenerateInput<NumericBoxDouble, double>(0, 1);
            Controls.Add(_probabilityOfBadPieceInput);
            settingsFactory.NextLine();

          
            Controls.Add(settingsFactory.GenerateLabel("Server IP:"));
            _serverIpInput = settingsFactory.GenerateInput();
            Controls.Add(_serverIpInput);
            settingsFactory.NextLine();
            
            Controls.Add(settingsFactory.GenerateLabel("Server port:"));
            _serverPortInput = settingsFactory.GenerateInput<NumericBoxInt, int>(1024, 65535);
            Controls.Add(_serverPortInput);
            settingsFactory.NextLine();

            Controls.Add(settingsFactory.GenerateLabel("Enable logging:"));
            _isLoggingEnabledCheck = settingsFactory.GenerateCheckBox();
            Controls.Add(_isLoggingEnabledCheck);
            settingsFactory.NextLine();

            var buttons = settingsFactory.GenerateButtons("OK", "Set default", "Cancel").ToList();

            _okButton = buttons[0];
            _okButton.MouseClick += delegate
            {
                SaveSettingsToFile();
                DialogResult = DialogResult.OK;
            };
            Controls.Add(_okButton);

            _setDefaultButton = buttons[1];
            _setDefaultButton.MouseClick += delegate { LoadSettings(GameSettings.GetDefault()); };
            Controls.Add(_setDefaultButton);

            _cancelButton = buttons[2];
            _cancelButton.MouseClick += delegate { DialogResult = DialogResult.Cancel; };
            Controls.Add(_cancelButton);

            Height = settingsFactory.CurrentHeight + 80;
        }

        private void LoadSettings(GameSettings settings)
        {
            _numberOfPlayersInput.Value = settings.NumberOfPlayers;
            _goalAreaHeightInput.Value = settings.GoalAreaHeight;
            _mapHeightInput.Value = settings.MapHeight;
            _mapWidthInput.Value = settings.MapWidth;
            _numberOfGoalsInput.Value = settings.NumberOfGoalsPerTeam;
            _numberOfPiecesInput.Value = settings.NumberOfPieces;
            _pieceGenerationIntervalInput.Value = settings.PieceGenerationInterval;
            _waitBaseInput.Value = settings.WaitBase;
            _waitDestroyPieceInput.Value = settings.WaitDestroyPiece;
            _waitDiscoveryInput.Value = settings.WaitDiscovery;
            _waitMoveInput.Value = settings.WaitMove;
            _waitPickPieceInput.Value = settings.WaitPickPiece;
            _waitPutPieceInput.Value = settings.WaitPutPiece;
            _waitTestPieceInput.Value = settings.WaitTestPiece;
            _waitInfoExchangeInput.Value = settings.WaitInfoExchange;
            _probabilityOfBadPieceInput.Value = settings.ProbabilityOfBadPiece;
            _serverIpInput.Text = settings.ServerIp;
            _serverPortInput.Value = settings.ServerPort;
            _isLoggingEnabledCheck.Checked = settings.IsLoggingEnabled;
        }

        public override object GetSettings()
        {
            return new GameSettings
            {
                NumberOfPlayers = _numberOfPlayersInput.Value,
                GoalAreaHeight = _goalAreaHeightInput.Value,
                MapHeight = _mapHeightInput.Value,
                MapWidth = _mapWidthInput.Value,
                NumberOfGoalsPerTeam = _numberOfGoalsInput.Value,
                NumberOfPieces = _numberOfPiecesInput.Value,
                PieceGenerationInterval = _pieceGenerationIntervalInput.Value,
                WaitBase = _waitBaseInput.Value,
                WaitDestroyPiece = _waitDestroyPieceInput.Value,
                WaitDiscovery = _waitDiscoveryInput.Value,
                WaitMove = _waitMoveInput.Value,
                WaitPickPiece = _waitPickPieceInput.Value,
                WaitPutPiece = _waitPutPieceInput.Value,
                WaitTestPiece = _waitTestPieceInput.Value,
                WaitInfoExchange = _waitInfoExchangeInput.Value,
                ProbabilityOfBadPiece = _probabilityOfBadPieceInput.Value,
                ServerIp = _serverIpInput.Text,
                ServerPort = _serverPortInput.Value,
                IsLoggingEnabled = _isLoggingEnabledCheck.Checked
            };
        }

        public void SaveSettingsToFile()
        {
            var settings = GetSettings();
            File.WriteAllText(GameSettings.CustomConfigPath, JsonConvert.SerializeObject(settings, Formatting.Indented));
        }
    }
}