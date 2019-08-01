using NUnit.Framework;
using GameLibrary.Messages;

namespace TCPTests.SerializationTests.ActionTests.RequestTests
{
    class DestroyPieceRequestTests
    {
        #region SerializationTests

        [Test]
        public void Should_ReturnCorrectString_When_Given_DestroyPieceRequestMessage()
        {
            var message = new DestroyPieceRequestMessage
            {
                AgentId = 8,
                RequestId = 0
            };
            string expected = "{\"msgId\":68,\"agentId\":8,\"requestId\":0}";
            TestsBase.SerializeAndCompareCertainMessage(message, expected);
        }

        #endregion

        #region DeserializationTests

        [Test]
        public void Should_Return_DestroyPieceRequestMessage_When_Given_String()
        {
            string messageString = "{\"msgId\":68,\"agentId\":8,\"requestId\":0}";
            DestroyPieceRequestMessage expected = new DestroyPieceRequestMessage
            {
                AgentId = 8,
                RequestId = 0
            };
            TestsBase.DeserializeAndCompareCertainMessage(messageString, expected);
        }

        #endregion
    }
}
