using Newtonsoft.Json;
using GameLibrary.Configuration;

namespace GameLibrary.Messages
{
    /// <summary>
    /// Represents message sent by GM to Agents that informs Agents about Game configuration and starting position.
    /// </summary>
    public class GameInfoMessage : Message
    {
        /// <summary>
        /// ID of the Agent in a game
        /// </summary>
        [JsonProperty("agentId")]
        public int AgentId { get; set; }
        /// <summary>
        /// ID of the team's TeamLeader
        /// </summary>
        [JsonProperty("teamLeaderId")]
        public int TeamLeaderId { get; set; }
        /// <summary>
        /// IDs of all agents from current agent's team
        /// </summary>
        [JsonProperty("agentIdsFromTeam")]
        public int[] AgentIdsFromTeam { get; set; }
        /// <summary>
        /// When the message was created, as measured by game time
        /// </summary>
        [JsonProperty("timestamp")]
        public int GameTime { get; set; }
        /// <summary>
        /// Width of the game board
        /// </summary>
        [JsonProperty("boardSizeX")]
        public int AgentMapWidth { get; set; }
        /// <summary>
        /// Height of the game board
        /// </summary>
        [JsonProperty("boardSizeY")]
        public int TaskAreaHeight { get; set; }
        /// <summary>
        /// Height of the goal area height
        /// </summary>
        [JsonProperty("goalAreaHeight")]
        public int GoalAreaHeight { get; set; }
        /// <summary>
        /// Player initial x coordinate
        /// </summary>
        [JsonProperty("initialXPosition")]
        public int InitialXPosition { get; set; }
        /// <summary>
        /// Player initial y coordinate
        /// </summary>
        [JsonProperty("initialYPosition")]
        public int InitialYPosition { get; set; }
        /// <summary>
        /// Time penalty for sending any message (???)
        /// </summary>
        [JsonProperty("baseTimePenalty")]
        public int BaseTimePenalty { get; set; }
        /// <summary>
        /// Time restriction after making move action
        /// </summary>
        [JsonProperty("tpm_move")]
        public int TpmMove { get; set; }
        /// <summary>
        /// Time restriction after making discovery action
        /// </summary>
        [JsonProperty("tpm_discoverPieces")]
        public int TpmDiscoverPieces { get; set; }
        /// <summary>
        /// Time restriction after making pickPiece action
        /// </summary>
        [JsonProperty("tpm_pickPiece")]
        public int TpmPickPiece { get; set; }
        /// <summary>
        /// Time restriction after making checkPiece action
        /// </summary>
        [JsonProperty("tpm_checkPiece")]
        public int TpmCheckPiece { get; set; }
        /// <summary>
        /// Time restriction after making destroyPiece action
        /// </summary>
        [JsonProperty("tpm_destroyPiece")]
        public int TpmDestroyPiece { get; set; }
        /// <summary>
        /// Time restriction after making putPiece action
        /// </summary>
        [JsonProperty("tpm_putPiece")]
        public int TpmPutPiece { get; set; }
        /// <summary>
        /// Time restriction after making infoExchange action
        /// </summary>
        [JsonProperty("tpm_infoExchange")]
        public int TpmInfoExchange { get; set; }
        /// <summary>
        /// Number of goals on the GM map for one team to score
        /// </summary>
        [JsonProperty("numberOfGoals")]
        public int NumberOfGoals { get; set; }
        /// <summary>
        /// Number of players in game
        /// </summary>
        [JsonProperty("numberOfPlayers")]
        public int NumberOfPlayers { get; set; }
        /// <summary>
        /// Time gap between consecutive piece spawns
        /// </summary>
        [JsonProperty("pieceSpawnDelay")]
        public int PieceSpawnDelay { get; set; }
        /// <summary>
        /// Maximum number of pieces on board
        /// </summary>
        [JsonProperty("maxNumberOfPiecesOnBoard")]
        public int MaxNumberOfPiecesOnBoard { get; set; }
        /// <summary>
        /// Probabilty for a piece to be a sham
        /// </summary>
        [JsonProperty("probabilityOfBadPiece")]
        public double ProbabilityOfBadPiece { get; set; }

        public GameInfoMessage(GameSettings settings)
        {
            AgentMapWidth = settings.MapWidth;
            TaskAreaHeight = settings.MapHeight - 2*settings.GoalAreaHeight;
            GoalAreaHeight = settings.GoalAreaHeight;
            BaseTimePenalty = settings.WaitBase;
            TpmMove = settings.WaitMove;
            TpmDiscoverPieces = settings.WaitDiscovery;
            TpmPickPiece = settings.WaitPickPiece;
            TpmCheckPiece = settings.WaitTestPiece;
            TpmPutPiece = settings.WaitPutPiece;
            TpmDestroyPiece = settings.WaitDestroyPiece;
            TpmInfoExchange = settings.WaitInfoExchange;
            NumberOfGoals = settings.NumberOfGoalsPerTeam;
            NumberOfPlayers = settings.NumberOfPlayers;
            PieceSpawnDelay = settings.PieceGenerationInterval;
            MaxNumberOfPiecesOnBoard = settings.NumberOfPieces;
            ProbabilityOfBadPiece = settings.ProbabilityOfBadPiece;
        }

        public GameInfoMessage() { }
    }
}
