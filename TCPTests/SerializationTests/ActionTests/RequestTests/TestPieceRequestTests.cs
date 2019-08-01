using NUnit.Framework;
using GameLibrary.Messages;

namespace TCPTests.SerializationTests.ActionTests.RequestTests
{
    class TestPieceRequestTests
    {
        #region SerializationTests

        [Test]
        public void Should_ReturnCorrectString_When_Given_TestPieceRequestMessage()
        {
            var message = new TestPieceRequestMessage
            {
                AgentId = 9,
                RequestId = 0
            };
            string expected = "{\"msgId\":67,\"agentId\":9,\"requestId\":0}";
            TestsBase.SerializeAndCompareCertainMessage(message, expected);
        }

        #endregion

        #region DeserializationTests

        [Test]
        public void Should_Return_TestPieceRequestMessage_When_Given_String()
        {
            string messageString = "{\"msgId\":67,\"agentId\":9,\"requestId\":0}";
            TestPieceRequestMessage expected = new TestPieceRequestMessage
            {
                AgentId = 9,
                RequestId = 0
            };
            TestsBase.DeserializeAndCompareCertainMessage(messageString, expected);
        }

        #endregion
    }
}