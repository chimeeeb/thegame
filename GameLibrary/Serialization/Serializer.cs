using System.Collections.Generic;
using GameLibrary.Messages;
using Newtonsoft.Json;

namespace GameLibrary.Serialization
{
    /// <summary>
    /// Static class that enables serialization with no additional packages needed.
    /// </summary>
    public static class Serializer
    {
        /// <summary>
        /// JSON settings used.
        /// </summary>
        private static JsonSerializerSettings settings;

        static Serializer()
        {
            settings = new JsonSerializerSettings()
            {
                Converters = new List<JsonConverter>() { new MessageJsonConverter() }
            };
        }

        /// <summary>
        /// Static method to wrap serialization.
        /// </summary>
        /// <param name="message">Message to be serialized.</param>
        /// <returns>JSON string representing the message.</returns>
        public static string Serialize(Message message)
        {
            return JsonConvert.SerializeObject(message, message.GetType(), settings) + '\n';
        }
        
        /// <summary>
        /// Static method to wrap deserialization.
        /// </summary>
        /// <param name="messageString">JSON string to be processed.</param>
        /// <returns>Object of type Message with deserialized data.</returns>
        public static Message Deserialize(string messageString)
        {
            return JsonConvert.DeserializeObject<Message>(messageString, settings);
        }
    }
}