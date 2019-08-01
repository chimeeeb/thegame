using GameLibrary.Messages;
using GameLibrary.Serialization;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace TCPTests
{
    public static class TestsBase
    {
        public static void SerializeAndCompareCertainMessage(Message message, string expected)
        {
            string output = Serializer.Serialize(message);
            Assert.NotNull(output);
            JObject jo_expected = JObject.Parse(expected);
            JObject jo_output = JObject.Parse(output);
            Assert.True(JToken.DeepEquals(jo_expected, jo_output));
        }

        public static void DeserializeAndCompareCertainMessage(string messageString, Message expected)
        {
            var output = Serializer.Deserialize(messageString);
            Assert.NotNull(output);
            string output_string = Serializer.Serialize(output);
            string expected_string = Serializer.Serialize(expected);
            Assert.AreEqual(expected_string, output_string);
        }
    }
}

