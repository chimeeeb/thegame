using NUnit.Framework;
using GameLibrary.Messages;

namespace TCPTests.SerializationTests.ErrorTests
{
    class GmNotRespondingTests
    {
        #region SerializationTests

        [Test]
        public void Should_ReturnCorrectString_When_Given_GmNotRespondingMessage()
        {
            var message = new GmNotRespondingMessage
            {
                AgentId = 17,
                RequestId = 0,
            };
            string expected = "{\"msgId\":1,\"agentId\":17,\"requestId\":0}";
            TestsBase.SerializeAndCompareCertainMessage(message, expected);
        }

        #endregion

        #region DeserializationTests

        [Test]
        public void Should_Return_GmNotRespondingMessage_When_Given_String()
        {
            string messageString = "{\"msgId\":1,\"agentId\":17,\"requestId\":0}";
            GmNotRespondingMessage expected = new GmNotRespondingMessage
            {
                AgentId = 17,
                RequestId = 0,
            };
            TestsBase.DeserializeAndCompareCertainMessage(messageString, expected);
        }

        #endregion
    }
}
