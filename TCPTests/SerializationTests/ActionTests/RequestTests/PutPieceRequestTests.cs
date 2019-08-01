using NUnit.Framework;
using GameLibrary.Messages;

namespace TCPTests.SerializationTests.ActionTests.RequestTests
{
    class PutPieceRequestTests
    {
        #region SerializationTests

        [Test]
        public void Should_ReturnCorrectString_When_Given_PutPieceRequestMessage()
        {
            var message = new PutPieceRequestMessage
            {
                AgentId = 3,
                RequestId = 0
            };
            string expected = "{\"msgId\":69,\"agentId\":3,\"requestId\":0}";
            TestsBase.SerializeAndCompareCertainMessage(message, expected);
        }

        #endregion

        #region DeserializationTests

        [Test]
        public void Should_Return_PutPieceRequestMessage_When_Given_String()
        {
            string messageString = "{\"msgId\":69,\"agentId\":3,\"requestId\":0}";
            PutPieceRequestMessage expected = new PutPieceRequestMessage
            {
                AgentId = 3,
                RequestId = 0
            };
            TestsBase.DeserializeAndCompareCertainMessage(messageString, expected);
        }

        #endregion
    }
}