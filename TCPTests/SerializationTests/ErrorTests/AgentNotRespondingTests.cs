using NUnit.Framework;
using GameLibrary.Messages;

namespace TCPTests.SerializationTests.ErrorTests
{
    class AgentNotRespondingTests
    {
        #region SerializationTests

        [Test]
        public void Should_ReturnCorrectString_When_Given_AgentNotRespondingMessage()
        {
            var message = new AgentNotRespondingMessage
            {
                AgentId = 5,
                RequestId = 0
            };
            string expected = "{\"msgId\":2,\"agentId\":5,\"requestId\":0}";
            TestsBase.SerializeAndCompareCertainMessage(message, expected);
        }

        #endregion

        #region DeserializationTests

        [Test]
        public void Should_Return_AgentNotRespondingMessage_When_Given_String()
        {
            string messageString = "{\"msgId\":2,\"agentId\":5,\"requestId\":0}";
            AgentNotRespondingMessage expected = new AgentNotRespondingMessage
            {
                AgentId = 5,
                RequestId = 0
            };
            TestsBase.DeserializeAndCompareCertainMessage(messageString, expected);
        }

        #endregion
    }
}