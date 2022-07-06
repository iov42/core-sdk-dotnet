using Iov42sdk.Models.Permissions;

namespace Iov42sdk.Models.CreateIdentity
{
    public class CreateIdentityBody : WriteBody
    {
        public CreateIdentityBody()
            : base(NodeConstants.CreateIdentityRequestType)
        {
        }

        public CreateIdentityBody(string identityId, Credentials publicCredentials, IdentityPermissions permissions = null)
            : this()
        {
            IdentityId = identityId;
            PublicCredentials = publicCredentials;
            Permissions = permissions;
        }

        public string IdentityId { get; set; }
        public Credentials PublicCredentials { get; set; }
        public IdentityPermissions Permissions { get; set; }
    }
}
