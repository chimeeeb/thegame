using Newtonsoft.Json;

namespace GameLibrary.Messages
{
    /// <summary>
    /// Base for error messages.
    /// </summary>
    public abstract class ErrorMessage : Message
    {
        /// <summary>
        /// ID of the Agent.
        /// </summary>
        [JsonProperty("agentId")]
        public int AgentId { get; set; }
    }
}
