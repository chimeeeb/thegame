using NUnit.Framework;
using GameLibrary.Messages;

namespace TCPTests.SerializationTests.ErrorTests
{
    class InvalidJsonTests
    {
        #region SerializationTests

        [Test]
        public void Should_ReturnCorrectString_When_Given_InvalidJsonMessage()
        {
            var message = new InvalidJsonMessage
            {
                AgentId = 12,
                RequestId = 0,
            };
            string expected = "{\"msgId\":0,\"agentId\":12,\"requestId\":0}";
            TestsBase.SerializeAndCompareCertainMessage(message, expected);
        }

        #endregion

        #region DeserializationTests

        [Test]
        public void Should_Return_InvalidJsonMessage_When_Given_String()
        {
            string messageString = "{\"msgId\":0,\"agentId\":12,\"requestId\":0}";
            InvalidJsonMessage expected = new InvalidJsonMessage
            {
                AgentId = 12,
                RequestId = 0,
            };
            TestsBase.DeserializeAndCompareCertainMessage(messageString, expected);
        }

        #endregion
    }
}
