namespace Iov42sdk.Models.CreateIdentity
{
    public class CreateIdentityBody : WriteBody
    {
        public CreateIdentityBody()
            : base(NodeConstants.CreateIdentityRequestType)
        {
        }

        public CreateIdentityBody(string identityId, Credentials publicCredentials)
            : this()
        {
            IdentityId = identityId;
            PublicCredentials = publicCredentials;
        }

        public string IdentityId { get; set; }
        public Credentials PublicCredentials { get; set; }
    }
}
