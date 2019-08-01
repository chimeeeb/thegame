using NUnit.Framework;
using GameLibrary.Messages;

namespace TCPTests.SerializationTests.ActionTests.RequestTests
{
    class ExchangeInfosAskingTests
    {
        #region SerializationTests

        [Test]
        public void Should_ReturnCorrectString_When_Given_ExchangeInfosAskingMessage()
        {
            var message = new ExchangeInfosAskingMessage
            {
                AgentId = 5,
                WithAgentId = 1,
                RequestId = 0,
                GameTimeStamp = 100
            };
            string expected = "{\"msgId\":71,\"agentId\":5,\"withAgentId\":1,\"timestamp\":100,\"requestId\":0}";
            TestsBase.SerializeAndCompareCertainMessage(message, expected);
        }

        #endregion

        #region DeserializationTests

        [Test]
        public void Should_Return_ExchangeInfosAskingMessage_When_Given_String()
        {
            string messageString = "{\"msgId\":71,\"agentId\":5,\"withAgentId\":1,\"timestamp\":100,\"requestId\":0}";
            ExchangeInfosAskingMessage expected = new ExchangeInfosAskingMessage
            {
                AgentId = 5,
                WithAgentId = 1,
                RequestId = 0,
                GameTimeStamp = 100
            };
            TestsBase.DeserializeAndCompareCertainMessage(messageString, expected);
        }

        #endregion
    }
}
