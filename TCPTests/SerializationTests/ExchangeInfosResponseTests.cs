using Player;
using System;
using NUnit.Framework;
using GameLibrary.Enum;
using GameLibrary.Messages;
using GameLibrary.Serialization;

namespace TCPTests.SerializationTests
{
    class ExchangeInfosResponseTests
    {
        #region SerializationTests

        [Test]
        public void Should_ReturnCorrectString_When_Given_Accepted_ExchangeInfosResponseMessage()
        {
            Map map = new Map(6, 6, 2);
            Random rand = new Random();
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    map[i, j].UpdateTile(DateTime.Now, i + j, (TileType)i + j);
                }
            }
            string data = Map.GetDataStringFromMap(map);
            var message = new ExchangeInfosResponseMessage
            {
                AgentId = 1,
                Agreement = true,
                RequestId = 3,
                WithAgentId = 4,
                Data = data
            };
            string output = Serializer.Serialize(message);
            ExchangeInfosResponseMessage msg = (ExchangeInfosResponseMessage)Serializer.Deserialize(output);
            Assert.AreEqual(msg.Data, data);
            Assert.AreEqual(msg.AgentId, 1);
            Assert.AreEqual(msg.RequestId, 3);
            Assert.AreEqual(msg.WithAgentId, 4);
            Assert.AreEqual(msg.Agreement, true);
        }

        [Test]
        public void Should_ReturnCorrectString_When_Given_Unaccepted_ExchangeInfosResponseMessage()
        {
            var message = new ExchangeInfosResponseMessage
            {
                AgentId = 4,
                Agreement = false,
                RequestId = 3,
                WithAgentId = 1,
                Data = ""
            };
            string expected = "{\"msgId\":72,\"agentId\":4,\"agreement\":false,\"withAgentId\":1,\"data\":\"\",\"requestId\":3}";
            TestsBase.SerializeAndCompareCertainMessage(message, expected);
        }

        #endregion

        #region DeserializationTests

        [Test]
        public void Should_Return_Accepted_ExchangeInfosResponseMessage_When_Given_String()
        {
            Map map = new Map(6, 6, 2);
            Random rand = new Random();
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    map[i, j].UpdateTile(DateTime.Now, i + j, (TileType)i + j);
                }
            }
            string data = Map.GetDataStringFromMap(map);
            ExchangeInfosResponseMessage expected = new ExchangeInfosResponseMessage
            {
                AgentId = 1,
                Agreement = true,
                RequestId = 4,
                WithAgentId = 4,
                Data = data
            };
            string messageString = Serializer.Serialize(expected);
            TestsBase.DeserializeAndCompareCertainMessage(messageString, expected);
        }

        [Test]
        public void Should_Return_Unaccepted_ExchangeInfosResponseMessage_When_Given_String()
        {
            string messageString = "{\"msgId\":72,\"agentId\":4,\"agreement\":false,\"withAgentId\":1,\"data\":\"\",\"requestId\":4}";
            ExchangeInfosResponseMessage expected = new ExchangeInfosResponseMessage
            {
                AgentId = 4,
                Agreement = false,
                RequestId = 4,
                WithAgentId = 1,
                Data = ""
            };
            TestsBase.DeserializeAndCompareCertainMessage(messageString, expected);
        }

        #endregion
    }
}
