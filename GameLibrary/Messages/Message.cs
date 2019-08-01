using Newtonsoft.Json;
using System;

namespace GameLibrary.Messages
{
    /// <summary>
    /// A class that represents a message between Agents and Game Master
    /// </summary>
    public abstract class Message
    {
        /// <summary>
        /// If current meesage is response to a request, RequestId is set to request's id. Otherwise this id is 0.
        /// </summary>
        [JsonProperty("requestId")]
        public int RequestId { get; set; }

        /// <summary>
        /// Creates a new Message
        /// </summary>
        protected Message()
        {
            RequestId = 0;
        }
    }
}
