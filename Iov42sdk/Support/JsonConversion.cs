using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Iov42sdk.Support
{
    public class JsonConversion
    {
        private readonly JsonSerializerSettings _jsonSettings;

        public JsonConversion()
        {
            _jsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() },
                Formatting = Formatting.None
            };
        }

        public T ConvertTo<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }

        public string ConvertFrom<T>(T data, bool reduced = false)
        {
            var serializeObject = JsonConvert.SerializeObject(data, _jsonSettings);
            return reduced ? serializeObject.Replace("\n", " ") : serializeObject;
        }
    }
}