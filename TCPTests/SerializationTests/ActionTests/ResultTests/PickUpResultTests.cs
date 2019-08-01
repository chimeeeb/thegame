using NUnit.Framework;
using GameLibrary.Messages;

namespace TCPTests.SerializationTests.ActionTests.ResultTests
{
    class PickUpResultTests
    {
        #region SerializationTests

        [Test]
        public void Should_ReturnCorrectString_When_Given_PickUpResultMessage()
        {
            var message = new PickUpResultMessage
            {
                AgentId = 10,
                RequestId = 1,
                GameTimeStamp = 12312,
                WaitUntilTime = 999999
            };
            string expected = "{\"msgId\":130,\"agentId\":10,\"timestamp\":12312,\"waitUntilTime\":999999,\"requestId\":1}";
            TestsBase.SerializeAndCompareCertainMessage(message, expected);
        }

        #endregion

        #region DeserializationTests

        [Test]
        public void Should_Return_PickUpResultMessage_When_Given_String()
        {
            string messageString = "{\"msgId\":130,\"agentId\":10,\"timestamp\":12312,\"waitUntilTime\":999999,\"requestId\":11}";
            PickUpResultMessage expected = new PickUpResultMessage
            {
                AgentId = 10,
                RequestId = 11,
                GameTimeStamp = 12312,
                WaitUntilTime = 999999
            };
            TestsBase.DeserializeAndCompareCertainMessage(messageString, expected);
        }

        #endregion
    }
}