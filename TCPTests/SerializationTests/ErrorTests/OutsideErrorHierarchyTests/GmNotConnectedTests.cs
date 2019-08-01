using NUnit.Framework;
using GameLibrary.Messages;

namespace TCPTests.SerializationTests.ErrorTests.OutsideErrorHierarchyTests
{
    class GmNotConnectedTests
    {
        #region SerializationTests

        [Test]
        public void Should_ReturnCorrectString_When_Given_GmNotConnectedMessage()
        {
            var message = new GmNotConnectedMessage
            {
                RequestId = 0
            };
            string expected = "{\"msgId\":6,\"requestId\":0}";
            TestsBase.SerializeAndCompareCertainMessage(message, expected);
        }

        #endregion

        #region DeserializationTests

        [Test]
        public void Should_Return_GmNotConnectedMessage_When_Given_String()
        {
            string messageString = "{\"msgId\":6,\"requestId\":0}";
            GmNotConnectedMessage expected = new GmNotConnectedMessage
            {
                RequestId = 0
            };
            TestsBase.DeserializeAndCompareCertainMessage(messageString, expected);
        }

        #endregion
    }
}