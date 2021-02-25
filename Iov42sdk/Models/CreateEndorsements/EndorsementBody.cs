using System.Collections.Generic;
using Newtonsoft.Json;

namespace Iov42sdk.Models.CreateEndorsements
{
    public class EndorsementBody 
        : WriteBody
    {
        public EndorsementBody()
            : base(null)
        {
            Endorsements = new Dictionary<string, string>();
        }

        private EndorsementBody(string type)
            : base(type)
        {
            Endorsements = new Dictionary<string, string>();
        }

        public EndorsementBody(string type, string requestId, string subjectId, string endorser, Dictionary<string, string> endorsements)
            : this(type, requestId, null, subjectId, endorser, endorsements)
        {
        }

        public EndorsementBody(string type, string requestId, string subjectTypeId, string subjectId, string endorserId, Dictionary<string, string> endorsements)
            : this(type)
        {
            RequestId = requestId;
            SubjectId = subjectId;
            SubjectTypeId = subjectTypeId;
            EndorserId = endorserId;
            Endorsements = endorsements;
        }

        public string SubjectId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SubjectTypeId { get; set; }
        public string EndorserId { get; set; }
        public Dictionary<string, string> Endorsements { get; set; }
    }
}