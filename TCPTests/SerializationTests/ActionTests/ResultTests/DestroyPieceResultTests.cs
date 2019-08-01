using NUnit.Framework;
using GameLibrary.Messages;

namespace TCPTests.SerializationTests.ActionTests.ResultTests
{
    class DestroyPieceResultTests
    {
        #region SerializationTests

        [Test]
        public void Should_ReturnCorrectString_When_Given_DestroyPieceResultMessage()
        {
            var message = new DestroyPieceResultMessage
            {
                AgentId = 5,
                RequestId = 10,
                WaitUntilTime = 2000,
                GameTimeStamp = 1000
            };
            string expected = "{\"msgId\":132,\"agentId\":5,\"timestamp\":1000,\"waitUntilTime\":2000,\"requestId\":10}";
            TestsBase.SerializeAndCompareCertainMessage(message, expected);
        }

        #endregion

        #region DeserializationTests

        [Test]
        public void Should_Return_DestroyPieceResultMessage_When_Given_String()
        {
            string messageString = "{\"msgId\":132,\"agentId\":5,\"timestamp\":1000,\"waitUntilTime\":2000,\"requestId\":10}";
            DestroyPieceResultMessage expected = new DestroyPieceResultMessage
            {
                AgentId = 5,
                RequestId = 10,
                WaitUntilTime = 2000,
                GameTimeStamp = 1000
            };
            TestsBase.DeserializeAndCompareCertainMessage(messageString, expected);
        }

        #endregion
    }
}