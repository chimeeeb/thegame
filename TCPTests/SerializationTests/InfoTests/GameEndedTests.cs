using NUnit.Framework;
using GameLibrary.Messages;
using GameLibrary.Enum;

namespace TCPTests.SerializationTests.InfoTests
{
    class GameEndedTests
    {
        #region SerializationTests

        [Test]
        public void Should_ReturnCorrectString_When_Given_GameEndedMessage()
        {
            var message = new GameEndedMessage
            {
                AgentId = 5,
                RequestId = 0,
                GameTimeStamp = 1000,
                WinningTeam = Team.Red
            };
            string expected = "{\"msgId\":33,\"agentId\":5,\"timestamp\":1000,\"winningTeam\":0,\"requestId\":0}";
            TestsBase.SerializeAndCompareCertainMessage(message, expected);
        }

        #endregion

        #region DeserializationTests

        [Test]
        public void Should_Return_GameEndedMessage_When_Given_String()
        {
            string messageString = "{\"msgId\":33,\"agentId\":5,\"timestamp\":1000,\"winningTeam\":1,\"requestId\":0}";
            GameEndedMessage expected = new GameEndedMessage
            {
                AgentId = 5,
                RequestId = 0,
                GameTimeStamp = 1000,
                WinningTeam = Team.Blue
            };
            TestsBase.DeserializeAndCompareCertainMessage(messageString, expected);
        }

        #endregion
    }
}
