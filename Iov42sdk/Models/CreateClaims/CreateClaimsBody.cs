using Newtonsoft.Json;

namespace Iov42sdk.Models.CreateClaims
{
    public class CreateClaimsBody : WriteBody
    {
        public CreateClaimsBody()
            : base(null)
        {
        }

        private CreateClaimsBody(string type)
            : base(type)
        {
        }


        public CreateClaimsBody(string type, string subjectId, string[] claims)
            : this(type)
        {
            SubjectId = subjectId;
            Claims = claims;
        }

        public CreateClaimsBody(string type, string subjectTypeId, string subjectId, string[] claims)
            : this(type)
        {
            SubjectId = subjectId;
            SubjectTypeId = subjectTypeId;
            Claims = claims;
        }

        public string SubjectId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SubjectTypeId { get; set; }
        public string[] Claims { get; set; }
    }
}
