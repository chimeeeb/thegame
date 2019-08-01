using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using GameLibrary.Messages;
using Newtonsoft.Json.Linq;

namespace GameLibrary.Serialization
{
    /// <summary>
    /// Specialized converter to convert JSON strings and different Message types.
    /// </summary>
    public class MessageJsonConverter : JsonConverter
    {
        private static readonly Dictionary<int, Type> IdToType;
        private static readonly Dictionary<Type, int> TypeToId;
        private static readonly JsonSerializer DefaultSerializer = JsonSerializer.CreateDefault();
        static MessageJsonConverter()
        {
            IdToType = new Dictionary<int, Type>
            {
                { 0, typeof(InvalidJsonMessage) },
                { 1, typeof(GmNotRespondingMessage) },
                { 2, typeof(AgentNotRespondingMessage) },
                { 3, typeof(WybranoCalyPrzedzialDlaPsaMessage) },
                { 4, typeof(RequestDuringPenaltyMessage) },
                { 5, typeof(CannotMoveThereMessage) },
                { 6, typeof(GmNotConnectedMessage) },
                { 7, typeof(InvalidActionMessage) },
                { 32, typeof(GameInfoMessage) },
                { 33, typeof(GameEndedMessage) },
                { 48, typeof(ConnectToGameMessage) },
                { 49, typeof(AcceptedToGameMessage) },
                { 50, typeof(ConnectGmMessage) },
                { 51, typeof(AcceptedGmMessage) },
                { 64, typeof(MoveRequestMessage) },
                { 65, typeof(DiscoveryRequestMessage)},
                { 66, typeof(PickUpRequestMessage) },
                { 67, typeof(TestPieceRequestMessage) },
                { 68, typeof(DestroyPieceRequestMessage) },
                { 69, typeof(PutPieceRequestMessage) },
                { 70, typeof(ExchangeInfosRequestMessage) },
                { 71, typeof(ExchangeInfosAskingMessage) },
                { 72, typeof(ExchangeInfosResponseMessage) },
                { 128, typeof(MoveResultMessage) },
                { 129, typeof(DiscoveryResultMessage)},
                { 130, typeof(PickUpResultMessage) },
                { 131, typeof(TestPieceResultMessage) },
                { 132, typeof(DestroyPieceResultMessage) },
                { 133, typeof(PutPieceResultMessage) },
                { 134, typeof(ExchangeInfosDataResultMessage) }
            };
            TypeToId = IdToType.ToDictionary(pair => pair.Value, pair => pair.Key);
        }

        public override bool CanRead => true;
        public override bool CanWrite => true;
        public override bool CanConvert(Type objectType) => typeof(Message).IsAssignableFrom(objectType);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JObject jo = JObject.FromObject(value);
            jo.Add("msgId", TypeToId[value.GetType()]);
            writer.WriteRaw(jo.ToString(Formatting.None));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken jObject = JToken.ReadFrom(reader);
            var msgIdToken = jObject["msgId"];
            if (msgIdToken == null)
                throw new JsonException("Unknown message");
            return DefaultSerializer.Deserialize(
                jObject.CreateReader(),
                IdToType[msgIdToken.Value<int>()]);
        }
    }
}