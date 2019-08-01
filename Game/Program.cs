using System;
using System.Windows.Forms;
using GameLibrary.Configuration;
using Game.GUI;
using Game.IoC;
using Ninject;
using log4net.Config;

namespace Game
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var kernel = new StandardKernel(new GameModule(GameSettings.DefaultConfigPath, GameSettings.CustomConfigPath));

            XmlConfigurator.Configure();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(kernel.Get<Client>());
        }
    }
}
