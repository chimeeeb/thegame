using NUnit.Framework;
using GameLibrary.Messages;
using System;
using Player;
using GameLibrary.Enum;
using GameLibrary.Serialization;

namespace TCPTests.SerializationTests.ActionTests.RequestTests
{
    class ExchangeInfosRequestTests
    {
        #region SerializationTests

        [Test]
        public void Should_ReturnCorrectString_When_Given_ExchangeInfosRequestMessage()
        {
            Map map = new Map(4, 4, 2);
            Random rand = new Random();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    map[i, j].UpdateTile(DateTime.Now, i + j, (TileType)i + j);
                }
            }
            string data = Map.GetDataStringFromMap(map);
            var message = new ExchangeInfosRequestMessage
            {
                AgentId = 8,
                WithAgentId = 6,
                RequestId = 0,
                Data = data
            };
            string output = Serializer.Serialize(message);
            ExchangeInfosRequestMessage msg = (ExchangeInfosRequestMessage)Serializer.Deserialize(output);
            Assert.AreEqual(msg.Data, data);
            Assert.AreEqual(msg.AgentId, 8);
            Assert.AreEqual(msg.RequestId, 0);
            Assert.AreEqual(msg.WithAgentId, 6);
        }

        #endregion

        #region DeserializationTests

        [Test]
        public void Should_Return_ExchangeInfosRequestMessage_When_Given_String()
        {
            Map map = new Map(4, 4, 2);
            Random rand = new Random();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    map[i, j].UpdateTile(DateTime.Now, i + j, (TileType)i + j);
                }
            }
            string data = Map.GetDataStringFromMap(map);
            ExchangeInfosRequestMessage expected = new ExchangeInfosRequestMessage
            {
                AgentId = 8,
                WithAgentId = 6,
                RequestId = 0,
                Data = data
            };
            string messageString = Serializer.Serialize(expected);
            TestsBase.DeserializeAndCompareCertainMessage(messageString, expected);
        }

        #endregion
    }
}
