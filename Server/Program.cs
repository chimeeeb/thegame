using System;
using log4net.Config;

namespace Server
{
    class Program
    {
        /// <summary>
        /// Main function to launch communication server console.
        /// </summary>
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            Console.Write("Enter IP: ");
            string ip = Console.ReadLine();

            Console.Write("Enter port: ");
            int port = int.Parse(Console.ReadLine());

            TcpServer server = new TcpServer(ip, port);
            server.Listen();
            while (Console.ReadLine() == null) ;
        }
    }
}
