using GameLibrary.Serialization;
using NUnit.Framework;

namespace TCPTests.SerializationTests.ErrorCases
{
    class ErrorCases
    {
        [Test]
        public void Should_Throw_AnException_When_Given_CompletelyWrong_String()
        {
            string messageString = "{Wrong json is being tested in here}";
            Assert.Throws<Newtonsoft.Json.JsonReaderException>(() => Serializer.Deserialize(messageString));
        }

        [Test]
        public void Should_Throw_AnException_When_Given_PropertyMissed_String()
        {
            string messageString = "{\"agentId\":10,\"requestId\":0}";
            Assert.Throws<Newtonsoft.Json.JsonException>(() => Serializer.Deserialize(messageString));
        }
    }
}