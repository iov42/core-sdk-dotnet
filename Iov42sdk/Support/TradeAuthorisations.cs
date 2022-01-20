using Iov42sdk.Models.Headers;

namespace Iov42sdk.Support
{
    public class TradeAuthorisations
    {
        public string RequestId { get; }
        public string BodyText { get; }
        public Authorisation[] Authorisations { get; }

        public TradeAuthorisations(string requestId, string bodyText, params Authorisation[] authorisations)
        {
            RequestId = requestId;
            BodyText = bodyText;
            Authorisations = authorisations;
        }
    }
}