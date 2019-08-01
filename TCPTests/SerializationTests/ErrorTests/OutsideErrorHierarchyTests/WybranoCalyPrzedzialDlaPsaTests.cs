using NUnit.Framework;
using GameLibrary.Messages;

namespace TCPTests.SerializationTests.ErrorTests.OutsideErrorHierarchyTests
{
    class WybranoCalyPrzedzialDlaPsaTests
    {
        #region SerializationTests

        [Test]
        public void Should_ReturnCorrectString_When_Given_WybranoCalyPrzedzialDlaPsaMessage()
        {
            var message = new WybranoCalyPrzedzialDlaPsaMessage
            {
                RequestId = 0
            };
            string expected = "{\"msgId\":3,\"requestId\":0}";
            TestsBase.SerializeAndCompareCertainMessage(message, expected);
        }

        #endregion

        #region DeserializationTests

        [Test]
        public void Should_Return_WybranoCalyPrzedzialDlaPsaMessage_When_Given_String()
        {
            string messageString = "{\"msgId\":3,\"requestId\":0}";
            WybranoCalyPrzedzialDlaPsaMessage expected = new WybranoCalyPrzedzialDlaPsaMessage
            {
                RequestId = 0
            };
            TestsBase.DeserializeAndCompareCertainMessage(messageString, expected);
        }

        #endregion
    }
}
