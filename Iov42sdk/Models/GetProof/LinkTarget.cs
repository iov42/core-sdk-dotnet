using Newtonsoft.Json;

namespace Iov42sdk.Models.GetProof
{
    public class LinkTarget
    {
        public string Id { get; set; }
        [JsonProperty("_Type")]
        public string Type { get; set; }
    }
}