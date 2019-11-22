using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WeatherInsurance.Integration.AzureStorage.Blobs
{
    public static class BlobHelper
    {
        public static JToken ReadJsonFromStream(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                using (var jsonReader = new JsonTextReader(reader))
                {
                    return JToken.ReadFrom(jsonReader);
                }
            }
        }

        public static string ReadStringFromStream(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

    }
}
