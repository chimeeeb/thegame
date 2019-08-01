using NUnit.Framework;
using GameLibrary.Messages;

namespace TCPTests.SerializationTests.SetupTests
{
    class AcceptedToGameTests
    {
        #region SerializationTests

        [Test]
        public void Should_ReturnCorrectString_When_Given_Connected_AcceptedToGameMessage()
        {
            var message = new AcceptedToGameMessage
            {
                AgentId = 2,
                RequestId = 0,
                IsConnected = true
            };
            string expected = "{\"msgId\":49,\"agentId\":2,\"isConnected\":true,\"requestId\":0}";
            TestsBase.SerializeAndCompareCertainMessage(message, expected);
        }

        [Test]
        public void Should_ReturnCorrectString_When_Given_Unconnected_AcceptedToGameMessage()
        {
            var message = new AcceptedToGameMessage
            {
                AgentId = 8,
                RequestId = 0,
                IsConnected = false
            };
            string expected = "{\"msgId\":49,\"agentId\":8,\"isConnected\":false,\"requestId\":0}";
            TestsBase.SerializeAndCompareCertainMessage(message, expected);
        }

        #endregion

        #region DeserializationTests

        [Test]
        public void Should_Return_Connected_AcceptedToGameMessage_When_Given_String()
        {
            string messageString = "{\"msgId\":49,\"agentId\":2,\"isConnected\":true,\"requestId\":0}";
            AcceptedToGameMessage expected = new AcceptedToGameMessage
            {
                AgentId = 2,
                RequestId = 0,
                IsConnected = true
            };
            TestsBase.DeserializeAndCompareCertainMessage(messageString, expected);
        }

        [Test]
        public void Should_Return_Unconnected_AcceptedToGameMessage_When_Given_String()
        {
            string messageString = "{\"msgId\":49,\"agentId\":8,\"isConnected\":false,\"requestId\":0}";
            AcceptedToGameMessage expected = new AcceptedToGameMessage
            {
                AgentId = 8,
                RequestId = 0,
                IsConnected = false
            };
            TestsBase.DeserializeAndCompareCertainMessage(messageString, expected);
        }

        #endregion
    }
}
