using System.Collections.Generic;
using Iov42sdk.Models.Headers;

namespace Iov42sdk.Support
{
    public class PlatformWriteRequest
    {
        public PlatformWriteRequest(string requestId, string body, Authorisation[] authorisations = null, AuthenticationHeader authentication = null)
        {
            RequestId = requestId;
            Body = body;
            Authorisations = authorisations;
            Authentication = authentication;
        }

        public PlatformWriteRequest WithAdditionalHeaders(Dictionary<string, string> additionalHeaders)
        {
            AdditionalHeaders = additionalHeaders;
            return this;
        }

        public Dictionary<string, string> AdditionalHeaders { get; private set; }
        public AuthenticationHeader Authentication { get; }
        public Authorisation[] Authorisations { get; }
        public string RequestId { get; }
        public string Body { get; }
    }
}