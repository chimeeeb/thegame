using NUnit.Framework;
using GameLibrary.Messages;
using System.Collections.Generic;

namespace TCPTests.SerializationTests.ActionTests.ResultTests
{
    class DiscoveryResultTests
    {
        #region SerializationTests

        [Test]
        public void Should_ReturnCorrectString_When_Given_DiscoveryResultMessage()
        {
            List<JMapTile> closestPieces = new List<JMapTile>
            {
                new JMapTile() { X = 3, Y = 3, Distance = 2 },
                new JMapTile() { X = 3, Y = 3, Distance = 3 },
                new JMapTile() { X = 3, Y = 3, Distance = 4 },
                new JMapTile() { X = 3, Y = 3, Distance = 3 },
                new JMapTile() { X = 3, Y = 3, Distance = 4 },
                new JMapTile() { X = 3, Y = 3, Distance = 5 },
                new JMapTile() { X = 3, Y = 3, Distance = 4 },
                new JMapTile() { X = 3, Y = 3, Distance = 5 },
                new JMapTile() { X = 3, Y = 3, Distance = 6 },
            };
            var message = new DiscoveryResultMessage
            {
                AgentId = 3,
                RequestId = 10,
                GameTimeStamp = 1000,
                WaitUntilTime = 5000,
                DiscoveryResults = closestPieces
            };
            string expected = "{\"msgId\":129,\"agentId\":3,\"timestamp\":1000,\"waitUntilTime\":5000," +
                "\"closestPieces\":[" +
                "{\"x\":3,\"y\":3,\"dist\":2}," +
                "{\"x\":3,\"y\":3,\"dist\":3}," +
                "{\"x\":3,\"y\":3,\"dist\":4}," +
                "{\"x\":3,\"y\":3,\"dist\":3}," +
                "{\"x\":3,\"y\":3,\"dist\":4}," +
                "{\"x\":3,\"y\":3,\"dist\":5}," +
                "{\"x\":3,\"y\":3,\"dist\":4}," +
                "{\"x\":3,\"y\":3,\"dist\":5}," +
                "{\"x\":3,\"y\":3,\"dist\":6}],\"requestId\":10}";
            TestsBase.SerializeAndCompareCertainMessage(message, expected);
        }

        #endregion

        #region DeserializationTests

        [Test]
        public void Should_Return_DiscoveryResultMessage_When_Given_String()
        {
            string messageString = "{\"msgId\":129,\"agentId\":3,\"timestamp\":1000,\"waitUntilTime\":5000," +
                "\"closestPieces\":[" +
                "{\"x\":3,\"y\":3,\"dist\":2}," +
                "{\"x\":3,\"y\":3,\"dist\":3}," +
                "{\"x\":3,\"y\":3,\"dist\":4}," +
                "{\"x\":3,\"y\":3,\"dist\":3}," +
                "{\"x\":3,\"y\":3,\"dist\":4}," +
                "{\"x\":3,\"y\":3,\"dist\":5}," +
                "{\"x\":3,\"y\":3,\"dist\":4}," +
                "{\"x\":3,\"y\":3,\"dist\":5}," +
                "{\"x\":3,\"y\":3,\"dist\":6}],\"requestId\":10}";
            List<JMapTile> closestPieces = new List<JMapTile>
            {
                new JMapTile() { X = 3, Y = 3, Distance = 2 },
                new JMapTile() { X = 3, Y = 3, Distance = 3 },
                new JMapTile() { X = 3, Y = 3, Distance = 4 },
                new JMapTile() { X = 3, Y = 3, Distance = 3 },
                new JMapTile() { X = 3, Y = 3, Distance = 4 },
                new JMapTile() { X = 3, Y = 3, Distance = 5 },
                new JMapTile() { X = 3, Y = 3, Distance = 4 },
                new JMapTile() { X = 3, Y = 3, Distance = 5 },
                new JMapTile() { X = 3, Y = 3, Distance = 6 },
            };
            DiscoveryResultMessage expected = new DiscoveryResultMessage
            {
                AgentId = 3,
                RequestId = 10,
                GameTimeStamp = 1000,
                WaitUntilTime = 5000,
                DiscoveryResults = closestPieces
            };
            TestsBase.DeserializeAndCompareCertainMessage(messageString, expected);
        }

        #endregion
    }
}
