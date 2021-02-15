namespace Iov42sdk.Models.IssueIdentity
{
    public class IssueIdentityBody : PutBody
    {
        public IssueIdentityBody()
            : base(NodeConstants.IssueIdentityRequestType)
        {
        }

        public IssueIdentityBody(string identityId, Credentials publicCredentials)
            : this()
        {
            IdentityId = identityId;
            PublicCredentials = publicCredentials;
        }

        public string IdentityId { get; set; }
        public Credentials PublicCredentials { get; set; }
    }
}
