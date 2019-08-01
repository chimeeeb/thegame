using NUnit.Framework;
using GameLibrary.Messages;
using GameLibrary.Enum;

namespace TCPTests.SerializationTests.SetupTests
{
    class ConnectToGameTests
    {
        #region SerializationTests

        [Test]
        public void Should_ReturnCorrectString_When_Given_ConnectToGameMessage_ForLeader()
        {
            var message = new ConnectToGameMessage
            {
                AgentId = 5,
                RequestId = 0,
                TeamId = Team.Blue,
                WantToBeLeader = true
            };
            string expected = "{\"msgId\":48,\"teamId\":1,\"agentId\":5,\"wantToBeLeader\":true,\"requestId\":0}";
            TestsBase.SerializeAndCompareCertainMessage(message, expected);
        }

        [Test]
        public void Should_ReturnCorrectString_When_Given_ConnectToGameMessage_ForAgent()
        {
            var message = new ConnectToGameMessage
            {
                AgentId = 1,
                RequestId = 0,
                TeamId = Team.Red,
                WantToBeLeader = false
            };
            string expected = "{\"msgId\":48,\"teamId\":0,\"agentId\":1,\"wantToBeLeader\":false,\"requestId\":0}";
            TestsBase.SerializeAndCompareCertainMessage(message, expected);
        }

        #endregion

        #region DeserializationTests

        [Test]
        public void Should_Return_ConnectToGameMessage_ForLeader_When_Given_String()
        {
            string messageString = "{\"msgId\":48,\"teamId\":1,\"agentId\":5,\"wantToBeLeader\":true,\"requestId\":0}";
            ConnectToGameMessage expected = new ConnectToGameMessage
            {
                AgentId = 5,
                RequestId = 0,
                TeamId = Team.Blue,
                WantToBeLeader = true
            };
            TestsBase.DeserializeAndCompareCertainMessage(messageString, expected);
        }

        [Test]
        public void Should_Return_ConnectToGameMessage_ForAgent_When_Given_String()
        {
            string messageString = "{\"msgId\":48,\"teamId\":0,\"agentId\":1,\"wantToBeLeader\":false,\"requestId\":0}";
            ConnectToGameMessage expected = new ConnectToGameMessage
            {
                AgentId = 1,
                RequestId = 0,
                TeamId = Team.Red,
                WantToBeLeader = false
            };
            TestsBase.DeserializeAndCompareCertainMessage(messageString, expected);
        }

        #endregion
    }
}
