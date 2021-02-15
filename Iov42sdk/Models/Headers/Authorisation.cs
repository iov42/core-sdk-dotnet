namespace Iov42sdk.Models.Headers
{
    public class Authorisation
    {
        // ReSharper disable once UnusedMember.Global
        public Authorisation()
        {
        }

        public Authorisation(string protocolId, string identityId, string delegateIdentityId, string signature)
        {
            ProtocolId = protocolId;
            IdentityId = identityId;
            DelegateIdentityId = delegateIdentityId;
            Signature = signature;
        }

        public string ProtocolId { get; set; }
        public string DelegateIdentityId { get; set; }
        public string IdentityId { get; set; }
        public string Signature { get; set; }
    }
}