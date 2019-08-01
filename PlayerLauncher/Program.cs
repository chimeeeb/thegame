using System;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using log4net.Config;
using Ninject;
using Player;
using Player.GUI;
using Player.IoC;
using GameLibrary.Configuration;
using GameLibrary.Enum;

namespace PlayerLauncher
{
    class Program
    {
        /// <summary>
        /// Main function to launch a player according to arguments given on the command line.
        /// </summary>
        /// <param name="args">
        /// args[0] - is GUI enabled
        /// args[1] - team (0 is Red, 1 is Blue)
        /// args[2] - strategy
        /// args[3] - is logging enabled
        /// args[4] - server ip (optional)
        /// args[5] - server port (optional)
        /// </param>
        static void Main(string[] args)
        {
            if (args.Length < 4 || args.Length > 6)
            {
                Console.WriteLine("Invalid arguments number!");
                Console.WriteLine("USAGE: .\\PlayerLauncher.exe isGUIEnabled team strategy isLoggingEnabled [server IP] [server port]");
                return;
            }

            bool isGUIEnabled = bool.Parse(args[0]);

            AgentSettings agentSettings = AgentSettings.GetDefault();
            agentSettings.Team = (Team)Enum.Parse(typeof(Team), args[1]);
            agentSettings.Strategy = (StrategyType)Enum.Parse(typeof(StrategyType), args[2]);
            agentSettings.IsLoggingEnabled = bool.Parse(args[3]);
            if(args.Length > 4)
            {
                agentSettings.ServerIp = args[4];
            }
            if(args.Length > 5)
            {
                agentSettings.ServerPort = int.Parse(args[5]);
            }

            XmlConfigurator.Configure();

            if (isGUIEnabled)
            {
                File.WriteAllText(AgentSettings.CustomConfigPath, JsonConvert.SerializeObject(agentSettings, Formatting.Indented));

                var kernel = new StandardKernel(new PlayerModule(AgentSettings.DefaultConfigPath, AgentSettings.CustomConfigPath));

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(kernel.Get<Client>());
            }
            else
            {
                Agent agent = new Agent(agentSettings);
                if(!agent.ConnectToServer())
                {
                    Console.WriteLine("Could not connect to server, exiting...");
                    return;
                }
                agent.ConnectToGame();
                Console.WriteLine("Agent connected");

                Console.ReadKey();
            }
        }
    }
}
