using System;
using System.Text;
using Newtonsoft.Json;

namespace WatsonTcp
{
    public static class Common
    {
        /// <summary>
        /// Serialize an object to JSON.
        /// </summary>
        /// <param name="obj">Object to serialize.</param>
        /// <returns>JSON string.</returns>
        public static string SerializeJson(object obj)
        {
            if (obj == null) return null;
            string json = JsonConvert.SerializeObject(
                obj,
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                });

            return json;
        }

        /// <summary>
        /// Deserialize JSON string to an object using Newtonsoft JSON.NET.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="json">JSON string.</param>
        /// <returns>An object of the specified type.</returns>
        public static T DeserializeJson<T>(string json)
        {
            if (String.IsNullOrEmpty(json)) throw new ArgumentNullException(nameof(json));

            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception e)
            {
                Console.WriteLine("");
                Console.WriteLine("Exception while deserializing:");
                Console.WriteLine(json);
                Console.WriteLine("");
                throw e;
            }
        }

        /// <summary>
        /// Deserialize JSON string to an object using Newtonsoft JSON.NET.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="data">Byte array containing the JSON string.</param>
        /// <returns>An object of the specified type.</returns>
        public static T DeserializeJson<T>(byte[] data)
        {
            if (data == null || data.Length < 1) throw new ArgumentNullException(nameof(data));
            return DeserializeJson<T>(Encoding.UTF8.GetString(data));
        }
    }
}
