using NUnit.Framework;
using GameLibrary.Messages;

namespace TCPTests.SerializationTests.ErrorTests
{
    class RequestDuringPenaltyTests
    {
        #region SerializationTests

        [Test]
        public void Should_ReturnCorrectString_When_Given_RequestDuringPenaltyMessage()
        {
            var message = new RequestDuringPenaltyMessage
            {
                AgentId = 0,
                RequestId = 0,
                GameTimeStamp = 110,
                WaitUntilTime = 100000
            };
            string expected = "{\"msgId\":4,\"agentId\":0,\"timestamp\":110,\"waitUntilTime\":100000,\"requestId\":0}";
            TestsBase.SerializeAndCompareCertainMessage(message, expected);
        }

        #endregion

        #region DeserializationTests

        [Test]
        public void Should_Return_RequestDuringPenaltyMessage_When_Given_String()
        {
            string messageString = "{\"msgId\":4,\"agentId\":0,\"timestamp\":110,\"waitUntilTime\":100000,\"requestId\":0}";
            RequestDuringPenaltyMessage expected = new RequestDuringPenaltyMessage
            {
                AgentId = 0,
                RequestId = 0,
                GameTimeStamp = 110,
                WaitUntilTime = 100000
            };
            TestsBase.DeserializeAndCompareCertainMessage(messageString, expected);
        }

        #endregion
    }
}
