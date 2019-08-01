using NUnit.Framework;
using GameLibrary.Messages;

namespace TCPTests.SerializationTests.ErrorTests
{
    class InvalidActionTests
    {
        #region SerializationTests

        [Test]
        public void Should_ReturnCorrectString_When_Given_InvalidActionMessage()
        {
            var message = new InvalidActionMessage
            {
                AgentId = 10,
                RequestId = 0,
            };
            string expected = "{\"msgId\":7,\"agentId\":10,\"requestId\":0}";
            TestsBase.SerializeAndCompareCertainMessage(message, expected);
        }

        #endregion

        #region DeserializationTests

        [Test]
        public void Should_Return_InvalidActionMessage_When_Given_String()
        {
            string messageString = "{\"msgId\":7,\"agentId\":10,\"requestId\":0}";
            InvalidActionMessage expected = new InvalidActionMessage
            {
                AgentId = 10,
                RequestId = 0,
            };
            TestsBase.DeserializeAndCompareCertainMessage(messageString, expected);
        }

        #endregion
    }
}
