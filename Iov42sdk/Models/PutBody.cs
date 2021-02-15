using System;
using Newtonsoft.Json;

namespace Iov42sdk.Models
{
    public abstract class PutBody
    {
        protected PutBody(string type)
        {
            RequestType = type;
            RequestId = Guid.NewGuid().ToString();
        }

        [JsonProperty("_type")]
        public string RequestType { get; set; }

        public string RequestId { get; set; }
    }
}
