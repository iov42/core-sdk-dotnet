namespace Iov42sdk.Models.Headers
{
    public class AuthenticationHeader
    {
        // ReSharper disable once UnusedMember.Global
        public AuthenticationHeader()
        {
        }

        public AuthenticationHeader(string protocolId, string identityId, string signature, string delegateIdentityId = null)
        {
            IdentityId = identityId;
            ProtocolId = protocolId;
            Signature = signature;
            DelegateIdentityId = delegateIdentityId;
        }

        public string IdentityId { get; set; }
        public string ProtocolId { get; set; }
        public string Signature { get; set; }
        public string DelegateIdentityId { get; set; }
    }
}