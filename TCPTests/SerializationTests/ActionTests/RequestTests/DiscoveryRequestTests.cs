using GameLibrary.Messages;
using NUnit.Framework;

namespace TCPTests.SerializationTests.ActionTests.RequestTests
{
    class DiscoveryRequestTests
    {
        #region SerializationTests

        [Test]
        public void Should_ReturnCorrectString_When_Given_DiscoveryRequestMessage()
        {
            var message = new DiscoveryRequestMessage
            {
                AgentId = 1,
                RequestId = 0
            };
            string expected = "{\"msgId\":65,\"agentId\":1,\"requestId\":0}";
            TestsBase.SerializeAndCompareCertainMessage(message, expected);
        }

        #endregion

        #region DeserializationTests

        [Test]
        public void Should_Return_DiscoveryRequestMessage_When_Given_String()
        {
            string messageString = "{\"msgId\":65,\"agentId\":10,\"requestId\":0}";
            DiscoveryRequestMessage expected = new DiscoveryRequestMessage
            {
                AgentId = 10,
                RequestId = 0
            };
            TestsBase.DeserializeAndCompareCertainMessage(messageString, expected);
        }

        #endregion
    }
}
