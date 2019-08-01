using NUnit.Framework;
using GameLibrary.Messages;

namespace TCPTests.SerializationTests.SetupTests
{
    class AcceptedGmTests
    {
        #region SerializationTests

        [Test]
        public void Should_ReturnCorrectString_When_Given_Connected_AcceptedGmMessage()
        {
            var message = new AcceptedGmMessage
            {
                RequestId = 0,
                IsConnected = true
            };
            string expected = "{\"msgId\":51,\"isConnected\":true,\"requestId\":0}";
            TestsBase.SerializeAndCompareCertainMessage(message, expected);
        }

        [Test]
        public void Should_ReturnCorrectString_When_Given_Unconnected_AcceptedGmMessage()
        {
            var message = new AcceptedGmMessage
            {
                RequestId = 0,
                IsConnected = false
            };
            string expected = "{\"msgId\":51,\"isConnected\":false,\"requestId\":0}";
            TestsBase.SerializeAndCompareCertainMessage(message, expected);
        }

        #endregion

        #region DeserializationTests

        [Test]
        public void Should_Return_Connected_AcceptedGmMessage_When_Given_String()
        {
            string messageString = "{\"msgId\":51,\"isConnected\":true,\"requestId\":0}";
            AcceptedGmMessage expected = new AcceptedGmMessage
            {
                RequestId = 0,
                IsConnected = true
            };
            TestsBase.DeserializeAndCompareCertainMessage(messageString, expected);
        }

        [Test]
        public void Should_Return_Unconnected_AcceptedGmMessage_When_Given_String()
        {
            string messageString = "{\"msgId\":51,\"isConnected\":false,\"requestId\":0}";
            AcceptedGmMessage expected = new AcceptedGmMessage
            {
                RequestId = 0,
                IsConnected = false
            };
            TestsBase.DeserializeAndCompareCertainMessage(messageString, expected);
        }

        #endregion
    }
}
