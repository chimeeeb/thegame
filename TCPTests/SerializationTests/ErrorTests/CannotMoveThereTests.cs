using NUnit.Framework;
using GameLibrary.Messages;

namespace TCPTests.SerializationTests.ErrorTests
{
    class CannotMoveThereTests
    {
        #region SerializationTests

        [Test]
        public void Should_ReturnCorrectString_When_Given_CannotMoveThereMessage()
        {
            var message = new CannotMoveThereMessage
            {
                AgentId = 3,
                RequestId = 0,
                GameTimeStamp = 10
            };
            string expected = "{\"msgId\":5,\"agentId\":3,\"timestamp\":10,\"requestId\":0}";
            TestsBase.SerializeAndCompareCertainMessage(message, expected);
        }

        #endregion

        #region DeserializationTests

        [Test]
        public void Should_Return_CannotMoveThereMessage_When_Given_String()
        {
            string messageString = "{\"msgId\":5,\"agentId\":3,\"timestamp\":10,\"requestId\":0}";
            CannotMoveThereMessage expected = new CannotMoveThereMessage
            {
                AgentId = 3,
                RequestId = 0,
                GameTimeStamp = 10
            };
            TestsBase.DeserializeAndCompareCertainMessage(messageString, expected);
        }

        #endregion
    }
}