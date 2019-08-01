using NUnit.Framework;
using GameLibrary.Messages;
using GameLibrary.Enum;

namespace TCPTests.SerializationTests.ActionTests.RequestTests
{
    class MoveRequestTests
    {
        #region SerializationTest

        [Test]
        public void Should_ReturnCorrectString_When_Given_MoveUpRequestMessage()
        {
            var message = new MoveRequestMessage
            {
                AgentId = 7,
                Direction = Direction.Up,
                RequestId = 0
            };
            string expected = "{\"moveDirection\":0,\"agentId\":7,\"requestId\":0,\"msgId\":64}";
            TestsBase.SerializeAndCompareCertainMessage(message, expected);
        }

        [Test]
        public void Should_Return_CorrectString_When_Given_MoveDownRequestMessage()
        {
            var message = new MoveRequestMessage
            {
                AgentId = 4,
                Direction = Direction.Down,
                RequestId = 0
            };
            string expected = "{\"moveDirection\":2,\"agentId\":4,\"requestId\":0,\"msgId\":64}";
            TestsBase.SerializeAndCompareCertainMessage(message, expected);
        }

        [Test]
        public void Should_Return_CorrectString_When_Given_MoveRightRequestMessage()
        {
            var message = new MoveRequestMessage
            {
                AgentId = 2,
                Direction = Direction.Right,
                RequestId = 0
            };
            string expected = "{\"moveDirection\":1,\"agentId\":2,\"requestId\":0,\"msgId\":64}";
            TestsBase.SerializeAndCompareCertainMessage(message, expected);
        }

        [Test]
        public void Should_Return_CorrectString_When_Given_MoveLeftRequestMessage()
        {
            var message = new MoveRequestMessage
            {
                AgentId = 3,
                Direction = Direction.Left,
                RequestId = 0
            };
            string expected = "{\"moveDirection\":3,\"agentId\":3,\"requestId\":0,\"msgId\":64}";
            TestsBase.SerializeAndCompareCertainMessage(message, expected);
        }

        #endregion

        #region DeserializationTests

        [Test]
        public void Should_Return_MoveUpRequestMessage_When_Given_String()
        {
            string messageString = "{\"moveDirection\":0,\"agentId\":4,\"requestId\":0,\"msgId\":64}";
            MoveRequestMessage expected = new MoveRequestMessage
            {
                AgentId = 4,
                Direction = Direction.Up,
                RequestId = 0
            };
            TestsBase.DeserializeAndCompareCertainMessage(messageString, expected);
        }

        [Test]
        public void Should_Return_MoveDownRequestMessage_When_Given_String()
        {
            string messageString = "{\"moveDirection\":2,\"agentId\":5,\"requestId\":0,\"msgId\":64}";
            MoveRequestMessage expected = new MoveRequestMessage
            {
                AgentId = 5,
                Direction = Direction.Down,
                RequestId = 0
            };
            TestsBase.DeserializeAndCompareCertainMessage(messageString, expected);
        }

        [Test]
        public void Should_Return_MoveLeftRequestMessage_When_Given_String()
        {
            string messageString = "{\"moveDirection\":3,\"agentId\":3,\"requestId\":0,\"msgId\":64}";
            MoveRequestMessage expected = new MoveRequestMessage
            {
                AgentId = 3,
                Direction = Direction.Left,
                RequestId = 0
            };
            TestsBase.DeserializeAndCompareCertainMessage(messageString, expected);
        }

        [Test]
        public void Should_Return_MoveRightRequestMessage_When_Given_String()
        {
            string messageString = "{\"moveDirection\":1,\"agentId\":2,\"requestId\":0,\"msgId\":64}";
            MoveRequestMessage expected = new MoveRequestMessage
            {
                AgentId = 2,
                Direction = Direction.Right,
                RequestId = 0
            };
            TestsBase.DeserializeAndCompareCertainMessage(messageString, expected);
        }

        #endregion
    }
}
