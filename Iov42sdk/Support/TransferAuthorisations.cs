using Iov42sdk.Models.Headers;

namespace Iov42sdk.Support
{
    public class TransferAuthorisations
    {
        public string RequestId { get; }
        public string BodyText { get; }
        public Authorisation[] Authorisations { get; }

        public TransferAuthorisations(string requestId, string bodyText, params Authorisation[] authorisations)
        {
            RequestId = requestId;
            BodyText = bodyText;
            Authorisations = authorisations;
        }
    }
}