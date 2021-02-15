using System.Collections.Generic;

namespace Iov42sdk.Models.GetIdentity
{
    public class IdentityResult
    {
        public string Proof { get; set; }
        public string IdentityId { get; set; }
        public List<Credentials> PublicCredentials { get; set; }
    }
}

