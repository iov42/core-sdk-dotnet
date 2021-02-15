using Newtonsoft.Json;

namespace Iov42sdk.Models.GetProof
{
    public class ProofNodeId
    {
        [JsonProperty("_Type")]
        public string Type { get; set; }
        public string Id { get; set; }
    }
}