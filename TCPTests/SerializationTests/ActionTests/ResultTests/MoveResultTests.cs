using NUnit.Framework;
using GameLibrary.Messages;

namespace TCPTests.SerializationTests.ActionTests.ResultTests
{
    class MoveResultTests
    {
        #region SerializationTests

        [Test]
        public void Should_ReturnCorrectString_When_Given_MoveResultMessage()
        {
            var message = new MoveResultMessage
            {
                AgentId = 6,
                RequestId = 12,
                DistanceToPiece = 29,
                GameTimeStamp = 11121,
                WaitUntilTime = 111110
            };
            string expected = "{\"msgId\":128,\"agentId\":6,\"timestamp\":11121,\"waitUntilTime\":111110,\"closestPiece\":29,\"requestId\":12}";
            TestsBase.SerializeAndCompareCertainMessage(message, expected);
        }

        #endregion

        #region DeserializationTests

        [Test]
        public void Should_Return_MoveResultMessage_When_Given_String()
        {
            string messageString = "{\"msgId\":128,\"agentId\":6,\"timestamp\":11121,\"waitUntilTime\":111110,\"closestPiece\":29,\"requestId\":11}";
            MoveResultMessage expected = new MoveResultMessage
            {
                AgentId = 6,
                RequestId = 11,
                DistanceToPiece = 29,
                GameTimeStamp = 11121,
                WaitUntilTime = 111110
            };
            TestsBase.DeserializeAndCompareCertainMessage(messageString, expected);
        }

        #endregion
    }
}
