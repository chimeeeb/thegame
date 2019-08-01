using System.Drawing;
using System.Windows.Forms;

namespace GameLibrary.GUI
{
    public abstract class SettingsBase : Form
    {
        protected Label ReconnectGameInfo;

        public bool ShowReconnectGameInfo
        {
            get => ReconnectGameInfo.Text != string.Empty;
            set
            {
                if (value)
                    ReconnectGameInfo.Text = "Reconnect game to apply changes";
                else
                    ReconnectGameInfo.Text = string.Empty;
                ReconnectGameInfo.Refresh();
            }
        }

        protected SettingsBase()
        {
            AutoScaleMode = AutoScaleMode.Font;
            Text = "Settings";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            InitializeComponents();
            
            ReconnectGameInfo = new Label()
            {
                Size = new Size(Width - 10, 15),
                Location = new Point(5, 5),
                ForeColor = Color.Red,
                Visible = true,
                TextAlign = ContentAlignment.MiddleCenter
            };
            Controls.Add(ReconnectGameInfo);
        }
        protected abstract void InitializeComponents();
        public abstract object GetSettings();
    }
}