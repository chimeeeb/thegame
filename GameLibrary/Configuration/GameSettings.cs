using System.IO;
using Newtonsoft.Json;

namespace GameLibrary.Configuration
{
    public class GameSettings
    {
        [JsonIgnore]
        public static readonly string DefaultConfigPath = Path.Combine("Configuration", "default_game_settings.json");
        [JsonIgnore]
        public static readonly string CustomConfigPath = Path.Combine("Configuration", "user_game_settings.json");

        [JsonProperty("NumberOfPlayers")]
        public int NumberOfPlayers { get; set; }
        [JsonProperty("NumberOfGoalsPerTeam")]
        public int NumberOfGoalsPerTeam { get; set; }
        [JsonProperty("PieceGenerationInterval")]
        public int PieceGenerationInterval { get; set; }
        [JsonProperty("ProbabilityOfBadPiece")]
        public double ProbabilityOfBadPiece { get; set; }
        [JsonProperty("WaitBase")]
        public int WaitBase { get; set; }
        [JsonProperty("WaitMove")]
        public int WaitMove { get; set; }
        [JsonProperty("WaitPickPiece")]
        public int WaitPickPiece { get; set; }
        [JsonProperty("WaitTestPiece")]
        public int WaitTestPiece { get; set; }
        [JsonProperty("WaitPutPiece")]
        public int WaitPutPiece { get; set; }
        [JsonProperty("WaitDestroyPiece")]
        public int WaitDestroyPiece { get; set; }
        [JsonProperty("WaitDiscovery")]
        public int WaitDiscovery { get; set; }
        [JsonProperty("WaitInfoExchange")]
        public int WaitInfoExchange { get; set; }
        [JsonProperty("NumberOfPieces")]
        public int NumberOfPieces { get; set; }
        [JsonProperty("MapWidth")]
        public int MapWidth { get; set; }
        [JsonProperty("MapHeight")]
        public int MapHeight { get; set; }
        [JsonProperty("GoalAreaHeight")]
        public int GoalAreaHeight { get; set; }
        [JsonProperty("ServerIp")]
        public string ServerIp { get; set; }
        [JsonProperty("ServerPort")]
        public int ServerPort { get; set; }
        [JsonProperty("IsLoggingEnabled")]
        public bool IsLoggingEnabled { get; set; }

        public static GameSettings GetDefault()
        {
            using (StreamReader reader = new StreamReader(DefaultConfigPath))
            {
                string json = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<GameSettings>(json);
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is GameSettings)) return false;
            GameSettings settings = obj as GameSettings;
            return settings.GoalAreaHeight == GoalAreaHeight &&
                settings.MapHeight == MapHeight &&
                settings.MapWidth == MapWidth &&
                settings.NumberOfGoalsPerTeam == NumberOfGoalsPerTeam &&
                settings.NumberOfPieces == NumberOfPieces &&
                settings.NumberOfPlayers == NumberOfPlayers &&
                settings.PieceGenerationInterval == PieceGenerationInterval &&
                settings.ProbabilityOfBadPiece == ProbabilityOfBadPiece &&
                settings.ServerIp == ServerIp &&
                settings.ServerPort == ServerPort &&
                settings.WaitDestroyPiece == WaitDestroyPiece &&
                settings.WaitDiscovery == WaitDiscovery &&
                settings.WaitInfoExchange == WaitInfoExchange &&
                settings.WaitMove == WaitMove &&
                settings.WaitPickPiece == WaitPickPiece &&
                settings.WaitPutPiece == WaitPutPiece &&
                settings.WaitTestPiece == WaitTestPiece &&
                settings.IsLoggingEnabled == IsLoggingEnabled;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}