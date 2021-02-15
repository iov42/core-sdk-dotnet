using System;
using Iov42sdk.Crypto;

namespace Iov42sdk.Identity
{
    /// <summary>
    /// Helper class for creating identities - means you don't have to pass the crypto creation function
    /// each time
    /// </summary>
    public class IdentityBuilder
    {
        private readonly Func<IKeyPair, ICrypto> _createCrypto;

        public IdentityBuilder(Func<IKeyPair, ICrypto> createCrypto)
        {
            _createCrypto = createCrypto;
        }

        /// <summary>
        /// Create an identity using the optional id and keys
        /// </summary>
        /// <param name="id">The optional identity id to use. If none passed then it will create one</param>
        /// <param name="keys">The crypto keys to user and if none passed it will create some</param>
        /// <returns></returns>
        public IdentityDetails Create(string id = null, IKeyPair keys = null)
        {
            return new IdentityDetails(id ?? Guid.NewGuid().ToString(), _createCrypto(keys));
        }
    }
}