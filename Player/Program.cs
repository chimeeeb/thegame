using System;
using System.Windows.Forms;
using GameLibrary.Configuration;
using Player.GUI;
using log4net.Config;
using Ninject;
using Player.IoC;

namespace Player
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var kernel = new StandardKernel(new PlayerModule(AgentSettings.DefaultConfigPath, AgentSettings.CustomConfigPath));

            XmlConfigurator.Configure();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(kernel.Get<Client>());
        }
    }
}
