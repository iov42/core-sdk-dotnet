namespace Iov42sdk.Models.Headers
{
    public class AuthenticationHeader
    {
        // ReSharper disable once UnusedMember.Global
        public AuthenticationHeader()
        {
        }

        public AuthenticationHeader(string protocolId, string identityId, string signature)
        {
            IdentityId = identityId;
            ProtocolId = protocolId;
            Signature = signature;
        }

        public string IdentityId { get; set; }
        public string ProtocolId { get; set; }
        public string Signature { get; set; }
    }
}