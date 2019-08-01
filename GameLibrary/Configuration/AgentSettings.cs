using System.IO;
using GameLibrary.Enum;
using Newtonsoft.Json;

namespace GameLibrary.Configuration
{
    public class AgentSettings
    {
        [JsonIgnore]
        public static readonly string DefaultConfigPath = Path.Combine("Configuration", "default_agent_settings.json");
        [JsonIgnore]
        public static readonly string CustomConfigPath = Path.Combine("Configuration", "user_agent_settings.json");

        [JsonProperty("WantBeALeader")]
        public bool WantBeALeader { get; set; }
        [JsonProperty("Team")]
        public Team Team { get; set; }
        [JsonProperty("Strategy")]
        public StrategyType Strategy { get; set; }
        [JsonProperty("ServerIp")]
        public string ServerIp { get; set; }
        [JsonProperty("ServerPort")]
        public int ServerPort { get; set; }
        [JsonProperty("IsLoggingEnabled")]
        public bool IsLoggingEnabled { get; set; }

        public static AgentSettings GetDefault()
        {
            using (StreamReader reader = new StreamReader(DefaultConfigPath))
            {
                string json = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<AgentSettings>(json);
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is AgentSettings)) return false;
            AgentSettings settings = obj as AgentSettings;
            return settings.ServerIp == ServerIp &&
                settings.ServerPort == ServerPort &&
                settings.Strategy == Strategy &&
                settings.Team == Team &&
                settings.WantBeALeader == WantBeALeader &&
                settings.IsLoggingEnabled == IsLoggingEnabled;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}