using Newtonsoft.Json;

namespace GameLibrary.Messages
{

    /// <summary>
    /// Game master, after both agents have agreed for communication sends data they wanted to pass
    /// </summary>
    class ExchangeInfosDataResult : ResultMessage
    {
        /// <summary>
        /// Agent's id with whom you want to communicate 
        /// </summary>
        [JsonProperty("withAgentId")]
        public int WithAgentId { get; set; }

        /// <summary>
        /// Tells if agent wants to communicate
        /// </summary>
        [JsonProperty("agreement")]
        public bool Agreement { get; set; }


        /// <summary>
        /// Data you want to pass to other agent, empty if agreement is false
        /// </summary>
        [JsonProperty("data")]
        public string Data { get; set; }


        public ExchangeInfosDataResult(int agentId, int timestamp, int waitUntilTime, int withAgentId,bool agreement, string data)
            : base(agentId, timestamp, waitUntilTime)
        {
            WithAgentId = withAgentId;
            Agreement = agreement;
            Data = data;
            MsgId = "134";
        }
    }
}
