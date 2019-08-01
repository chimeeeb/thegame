using NUnit.Framework;
using GameLibrary.Messages;

namespace TCPTests.SerializationTests.SetupTests
{
    class ConnectGmTests
    {
        #region SerializationTests

        [Test]
        public void Should_ReturnCorrectString_When_Given_ConnectGmMessage()
        {
            var message = new ConnectGmMessage
            {
                RequestId = 0
            };
            string expected = "{\"msgId\":50,\"requestId\":0}";
            TestsBase.SerializeAndCompareCertainMessage(message, expected);
        }

        #endregion

        #region DeserializationTests

        [Test]
        public void Should_Return_ConnectGmMessage_When_Given_String()
        {
            string messageString = "{\"msgId\":50,\"requestId\":0}";
            ConnectGmMessage expected = new ConnectGmMessage
            {
                RequestId = 0
            };
            TestsBase.DeserializeAndCompareCertainMessage(messageString, expected);
        }

        #endregion
    }
}
