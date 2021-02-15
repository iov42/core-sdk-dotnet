using System.Diagnostics;
using Iov42sdk.Crypto;

namespace Iov42sdk.Identity
{
    /// <summary>
    /// Wrapper for the identity id and crypto instance (which included the keys)
    /// </summary>
    [DebuggerDisplay("{" + nameof(Id) + "}")]
    public class IdentityDetails
    {
        public string Id { get; }
        public ICrypto Crypto { get; }

        internal IdentityDetails(string id, ICrypto crypto)
        {
            Id = id;
            Crypto = crypto;
        }
    }
}
