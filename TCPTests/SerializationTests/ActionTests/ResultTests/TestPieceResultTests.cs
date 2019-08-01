using NUnit.Framework;
using GameLibrary.Messages;

namespace TCPTests.SerializationTests.ActionTests.ResultTests
{
    class TestPieceResultTests
    {
        #region SerializationTests

        [Test]
        public void Should_ReturnCorrectString_When_Given_TestRealPieceResultMessage()
        {
            var message = new TestPieceResultMessage
            {
                AgentId = 7,
                RequestId = 9,
                IsReal = true,
                GameTimeStamp = 500,
                WaitUntilTime = 1000
            };
            string expected = "{\"msgId\":131,\"agentId\":7,\"timestamp\":500,\"waitUntilTime\":1000,\"isCorrect\":true,\"requestId\":9}";
            TestsBase.SerializeAndCompareCertainMessage(message, expected);
        }

        [Test]
        public void Should_ReturnCorrectString_When_Given_TestFakePieceResultMessage()
        {
            var message = new TestPieceResultMessage
            {
                AgentId = 7,
                RequestId = 10,
                IsReal = false,
                GameTimeStamp = 122,
                WaitUntilTime = 10000
            };
            string expected = "{\"msgId\":131,\"agentId\":7,\"timestamp\":122,\"waitUntilTime\":10000,\"isCorrect\":false,\"requestId\":10}";
            TestsBase.SerializeAndCompareCertainMessage(message, expected);
        }

        #endregion

        #region DeserializationTests

        [Test]
        public void Should_Return_TestRealPieceResultMessage_When_Given_String()
        {
            string messageString = "{\"msgId\":131,\"agentId\":1,\"timestamp\":101,\"waitUntilTime\":1102,\"isCorrect\":true,\"requestId\":3}";
            TestPieceResultMessage expected = new TestPieceResultMessage
            {
                AgentId = 1,
                RequestId = 3,
                IsReal = true,
                GameTimeStamp = 101,
                WaitUntilTime = 1102
            };
            TestsBase.DeserializeAndCompareCertainMessage(messageString, expected);
        }

        [Test]
        public void Should_Return_TestFakePieceResultMessage_When_Given_String()
        {
            string messageString = "{\"msgId\":131,\"agentId\":3,\"timestamp\":1000,\"waitUntilTime\":100000,\"isCorrect\":false,\"requestId\":3}";
            TestPieceResultMessage expected = new TestPieceResultMessage
            {
                AgentId = 3,
                RequestId = 3,
                IsReal = false,
                GameTimeStamp = 1000,
                WaitUntilTime = 100000
            };
            TestsBase.DeserializeAndCompareCertainMessage(messageString, expected);
        }

        #endregion
    }
}