using Iov42sdk.Support;
using Newtonsoft.Json;

namespace Iov42sdk.Models
{
    public abstract class WriteBody
    {
        protected WriteBody(string type)
        {
            RequestType = type;
            RequestId = IovClient.CreateUniqueId();
        }

        [JsonProperty("_type")]
        public string RequestType { get; set; }

        public string RequestId { get; set; }
    }

    public static class PutBodyExtension
    {
        public static string Serialize(this WriteBody body)
        {
            var json = new JsonConversion();
            return json.ConvertFrom(body, true);
        }
    }
}
