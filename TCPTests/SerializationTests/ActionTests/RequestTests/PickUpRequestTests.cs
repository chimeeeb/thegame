using NUnit.Framework;
using GameLibrary.Messages;

namespace TCPTests.SerializationTests.ActionTests.RequestTests
{
    class PickUpRequestTests
    {
        #region SerializationTests

        [Test]
        public void Should_ReturnCorrectString_When_Given_PickUpRequestMessage()
        {
            var message = new PickUpRequestMessage
            {
                AgentId = 0,
                RequestId = 0
            };
            string expected = "{\"msgId\":66,\"agentId\":0,\"requestId\":0}";
            TestsBase.SerializeAndCompareCertainMessage(message, expected);
        }

        #endregion

        #region DeserializationTests

        [Test]
        public void Should_Return_PickUpRequestMessage_When_Given_String()
        {
            string messageString = "{\"msgId\":66,\"agentId\":0,\"requestId\":0}";
            PickUpRequestMessage expected = new PickUpRequestMessage
            {
                AgentId = 0,
                RequestId = 0
            };
            TestsBase.DeserializeAndCompareCertainMessage(messageString, expected);
        }

        #endregion
    }
}
