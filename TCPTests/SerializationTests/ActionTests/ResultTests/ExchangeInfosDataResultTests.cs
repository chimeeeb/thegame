using Player;
using System;
using NUnit.Framework;
using GameLibrary.Enum;
using GameLibrary.Messages;
using GameLibrary.Serialization;

namespace TCPTests.SerializationTests.ActionTests.ResultTests
{
    class ExchangeInfosDataResultTests
    {
        #region SerializationTests

        [Test]
        public void Should_ReturnCorrectString_When_Given_AcceptedExchangeInfosDataResultMessage()
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
            var message = new ExchangeInfosDataResultMessage
            {
                AgentId = 7,
                Agreement = true,
                Data = data,
                WithAgentId = 1,
                GameTimeStamp = 990,
                RequestId = 10,
                WaitUntilTime = 2000
            };
            string output = Serializer.Serialize(message);
            ExchangeInfosDataResultMessage msg = (ExchangeInfosDataResultMessage)Serializer.Deserialize(output);
            Assert.AreEqual(msg.Data, data);
            Assert.AreEqual(msg.AgentId, 7);
            Assert.AreEqual(msg.RequestId, 10);
            Assert.AreEqual(msg.WithAgentId, 1);
            Assert.AreEqual(msg.Agreement, true);
            Assert.AreEqual(msg.GameTimeStamp, 990);
            Assert.AreEqual(msg.WaitUntilTime, 2000);
        }

        [Test]
        public void Should_ReturnCorrectString_When_Given_RejectedExchangeInfosDataResultMessage()
        {
            var message = new ExchangeInfosDataResultMessage
            {
                AgentId = 7,
                Agreement = false,
                Data = "",
                WithAgentId = 1,
                GameTimeStamp = 990,
                RequestId = 20,
                WaitUntilTime = 2000
            };
            string expected = "{\"msgId\":134,\"agentId\":7,\"withAgentId\":1,\"agreement\":false,\"timestamp\":990,\"waitUntilTime\":2000,\"data\":\"\",\"requestId\":20}";
            TestsBase.SerializeAndCompareCertainMessage(message, expected);
        }

        #endregion

        #region DeserializationTests

        [Test]
        public void Should_Return_AcceptedExchangeInfosDataResultMessage_When_Given_String()
        {
            Map map = new Map(8, 8, 3);
            Random rand = new Random();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    map[i, j].UpdateTile(DateTime.Now, i + j, (TileType)i + j);
                }
            }
            string data = Map.GetDataStringFromMap(map);
            ExchangeInfosDataResultMessage expected = new ExchangeInfosDataResultMessage
            {
                AgentId = 7,
                Agreement = true,
                Data = data,
                WithAgentId = 1,
                GameTimeStamp = 990,
                RequestId = 12,
                WaitUntilTime = 2000
            };
            string messageString = Serializer.Serialize(expected);
            TestsBase.DeserializeAndCompareCertainMessage(messageString, expected);
        }

        [Test]
        public void Should_Return_RejectedExchangeInfosDataResultMessage_When_Given_String()
        {
            string messageString = "{\"msgId\":134,\"agentId\":7,\"withAgentId\":1,\"agreement\":false,\"timestamp\":990,\"waitUntilTime\":1000,\"data\":\"\",\"requestId\":13}";
            ExchangeInfosDataResultMessage expected = new ExchangeInfosDataResultMessage
            {
                AgentId = 7,
                Agreement = false,
                Data = "",
                WithAgentId = 1,
                GameTimeStamp = 990,
                RequestId = 13,
                WaitUntilTime = 1000
            };
            TestsBase.DeserializeAndCompareCertainMessage(messageString, expected);
        }

        #endregion
    }
}
