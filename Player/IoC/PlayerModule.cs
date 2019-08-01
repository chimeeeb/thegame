using System.IO;
using GameLibrary.Configuration;
using GameLibrary.GUI;
using Newtonsoft.Json;
using Ninject.Modules;
using Player.GUI;

namespace Player.IoC
{
    public class PlayerModule : NinjectModule
    {
        private readonly string _defaultConfigFile;
        private readonly string _customConfigFile;

        public PlayerModule(string defaultConfigFile, string customConfigFile)
        {
            _defaultConfigFile = defaultConfigFile;
            _customConfigFile = customConfigFile;
        }

        public override void Load()
        {
            Configure<AgentSettings>(File.Exists(_customConfigFile) ? _customConfigFile : _defaultConfigFile);
            Bind<ClientBase>().To<Client>().InSingletonScope();
            Bind<Agent>().ToSelf().InSingletonScope();
        }

        public void Configure<T>(string configFileName)
        {
            using (StreamReader reader = new StreamReader(configFileName))
            {
                string json = reader.ReadToEnd();
                Bind<T>().ToConstant(JsonConvert.DeserializeObject<T>(json)).InSingletonScope();
            }
        }
    }
}