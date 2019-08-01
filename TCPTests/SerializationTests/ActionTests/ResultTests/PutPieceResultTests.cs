using NUnit.Framework;
using GameLibrary.Messages;
using GameLibrary.Enum;

namespace TCPTests.SerializationTests.ActionTests.ResultTests
{
    class PutPieceResultTests
    {
        #region SerializationTests

        [Test]
        public void Should_ReturnCorrectString_When_Given_PiecePutInTaskArea_PutPieceResultMessage()
        {
            var message = new PutPieceResultMessage
            {
                AgentId = 9,
                Effect = Result.PiecePutInTaskArea,
                RequestId = 14,
                GameTimeStamp = 1212,
                WaitUntilTime = 1212312
            };
            string expected = "{\"msgId\":133,\"agentId\":9,\"timestamp\":1212,\"waitUntilTime\":1212312,\"result\":0,\"requestId\":14}";
            TestsBase.SerializeAndCompareCertainMessage(message, expected);
        }

        [Test]
        public void Should_ReturnCorrectString_When_Given_GoalCompleted_PutPieceResultMessage()
        {
            var message = new PutPieceResultMessage
            {
                AgentId = 10,
                Effect = Result.GoalCompleted,
                RequestId = 19,
                GameTimeStamp = 1212,
                WaitUntilTime = 1212312
            };
            string expected = "{\"msgId\":133,\"agentId\":10,\"timestamp\":1212,\"waitUntilTime\":1212312,\"result\":1,\"requestId\":19}";
            TestsBase.SerializeAndCompareCertainMessage(message, expected);
        }

        [Test]
        public void Should_ReturnCorrectString_When_Given_NonGoalDiscovered_PutPieceResultMessage()
        {
            var message = new PutPieceResultMessage
            {
                AgentId = 9,
                Effect = Result.NonGoalDiscovered,
                RequestId = 14,
                GameTimeStamp = 100,
                WaitUntilTime = 1212312
            };
            string expected = "{\"msgId\":133,\"agentId\":9,\"timestamp\":100,\"waitUntilTime\":1212312,\"result\":2,\"requestId\":14}";
            TestsBase.SerializeAndCompareCertainMessage(message, expected);
        }

        [Test]
        public void Should_ReturnCorrectString_When_Given_FakePieceInGoalArea_PutPieceResultMessage()
        {
            var message = new PutPieceResultMessage
            {
                AgentId = 9,
                Effect = Result.FakePieceInGoalArea,
                RequestId = 7,
                GameTimeStamp = 1000,
                WaitUntilTime = 1212312
            };
            string expected = "{\"msgId\":133,\"agentId\":9,\"timestamp\":1000,\"waitUntilTime\":1212312,\"result\":3,\"requestId\":7}";
            TestsBase.SerializeAndCompareCertainMessage(message, expected);
        }

        #endregion

        #region DeserializationTests

        [Test]
        public void Should_Return_PiecePutInTaskArea_PutPieceResultMessage_When_Given_String()
        {
            string messageString = "{\"msgId\":133,\"agentId\":9,\"timestamp\":1000,\"waitUntilTime\":1212312,\"result\":0,\"requestId\":14}";
            PutPieceResultMessage expected = new PutPieceResultMessage
            {
                AgentId = 9,
                Effect = Result.PiecePutInTaskArea,
                RequestId = 14,
                GameTimeStamp = 1000,
                WaitUntilTime = 1212312
            };
            TestsBase.DeserializeAndCompareCertainMessage(messageString, expected);
        }

        [Test]
        public void Should_Return_GoalCompleted_PutPieceResultMessage_When_Given_String()
        {
            string messageString = "{\"msgId\":133,\"agentId\":10,\"timestamp\":121,\"waitUntilTime\":1000,\"result\":1,\"requestId\":4}";
            PutPieceResultMessage expected = new PutPieceResultMessage
            {
                AgentId = 10,
                Effect = Result.GoalCompleted,
                RequestId = 4,
                GameTimeStamp = 121,
                WaitUntilTime = 1000
            };
            TestsBase.DeserializeAndCompareCertainMessage(messageString, expected);
        }

        [Test]
        public void Should_Return_NonGoalDiscovered_PutPieceResultMessage_When_Given_String()
        {
            string messageString = "{\"msgId\":133,\"agentId\":9,\"timestamp\":1000,\"waitUntilTime\":1212312,\"result\":2,\"requestId\":22}";
            PutPieceResultMessage expected = new PutPieceResultMessage
            {
                AgentId = 9,
                Effect = Result.NonGoalDiscovered,
                RequestId = 22,
                GameTimeStamp = 1000,
                WaitUntilTime = 1212312
            };
            TestsBase.DeserializeAndCompareCertainMessage(messageString, expected);
        }

        [Test]
        public void Should_Return_FakePieceInGoalArea_PutPieceResultMessage_When_Given_String()
        {
            string messageString = "{\"msgId\":133,\"agentId\":9,\"timestamp\":1212,\"waitUntilTime\":1212312,\"result\":3,\"requestId\":11}";
            PutPieceResultMessage expected = new PutPieceResultMessage
            {
                AgentId = 9,
                Effect = Result.FakePieceInGoalArea,
                RequestId = 11,
                GameTimeStamp = 1212,
                WaitUntilTime = 1212312
            };
            TestsBase.DeserializeAndCompareCertainMessage(messageString, expected);
        }

        #endregion
    }
}