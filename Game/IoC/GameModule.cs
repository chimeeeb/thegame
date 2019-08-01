using System.IO;
using GameLibrary.Configuration;
using Game.GUI;
using GameLibrary.GUI;
using Newtonsoft.Json;
using Ninject.Modules;

namespace Game.IoC
{
    public class GameModule : NinjectModule
    {
        private readonly string _defaultConfigFile;
        private readonly string _customConfigFile;

        public GameModule(string defaultConfigFile, string customConfigFile)
        {
            _defaultConfigFile = defaultConfigFile;
            _customConfigFile = customConfigFile;
        }

        public override void Load()
        {
            Configure<GameSettings>(File.Exists(_customConfigFile) ? _customConfigFile : _defaultConfigFile);
            Bind<ClientBase>().To<Client>().InSingletonScope();
            Bind<GameMaster>().ToSelf().InSingletonScope();
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