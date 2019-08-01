using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using GameLibrary.Configuration;
using GameLibrary.Enum;
using GameLibrary.GUI;
using GameLibrary.GUI.Controls;
using Newtonsoft.Json;

namespace Player.GUI
{
    public class Settings : SettingsBase
    {
        private CheckBox _wantBeALeaderCheck;
        private CheckBox _isLoggingEnabledCheck;
        private ComboBox _teamList, _strategyList;
        private TextBox _serverIpInput;
        private NumericBoxInt _serverPortInput;
        private Button _okButton, _setDefaultButton, _cancelButton;

        public Settings(AgentSettings settings)
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

            Controls.Add(settingsFactory.GenerateLabel("Want be a leader:"));
            _wantBeALeaderCheck = settingsFactory.GenerateCheckBox();
            Controls.Add(_wantBeALeaderCheck);
            settingsFactory.NextLine();

            Controls.Add(settingsFactory.GenerateLabel("Team:"));
            _teamList = settingsFactory.GenerateComboBox();
            Controls.Add(_teamList);
            _teamList.DataSource = Enum.GetValues(typeof(Team));
            settingsFactory.NextLine();

            Controls.Add(settingsFactory.GenerateLabel("Strategy:"));
            _strategyList = settingsFactory.GenerateComboBox();
            Controls.Add(_strategyList);
            _strategyList.DataSource = Enum.GetValues(typeof(StrategyType));
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
            _setDefaultButton.MouseClick += delegate { LoadSettings(AgentSettings.GetDefault()); };
            Controls.Add(_setDefaultButton);

            _cancelButton = buttons[2];
            _cancelButton.MouseClick += delegate { DialogResult = DialogResult.Cancel; };
            Controls.Add(_cancelButton);

            Height = settingsFactory.CurrentHeight + 80;
        }

        private void LoadSettings(AgentSettings settings)
        {
            _wantBeALeaderCheck.Checked = settings.WantBeALeader;
            _teamList.SelectedIndex = (int)settings.Team;
            _strategyList.SelectedIndex = (int)settings.Strategy;
            _serverIpInput.Text = settings.ServerIp;
            _serverPortInput.Value = settings.ServerPort;
            _isLoggingEnabledCheck.Checked = settings.IsLoggingEnabled;
        }

        public override object GetSettings()
        {
            Enum.TryParse<Team>(_teamList.SelectedItem.ToString(), out var team);
            Enum.TryParse<StrategyType>(_strategyList.SelectedItem.ToString(), out var strategy);
            return new AgentSettings
            {
                WantBeALeader = _wantBeALeaderCheck.Checked,
                Team = team,
                Strategy = strategy,
                ServerIp = _serverIpInput.Text,
                ServerPort = _serverPortInput.Value,
                IsLoggingEnabled = _isLoggingEnabledCheck.Checked
            };
        }

        public void SaveSettingsToFile()
        {
            var settings = GetSettings();
            File.WriteAllText(AgentSettings.CustomConfigPath, JsonConvert.SerializeObject(settings, Formatting.Indented));
        }
    }
}