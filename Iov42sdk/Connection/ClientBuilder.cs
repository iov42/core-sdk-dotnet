using System.Threading.Tasks;
using Iov42sdk.Identity;
using Iov42sdk.Support;

namespace Iov42sdk.Connection
{
    public class ClientBuilder
    {
        /// <summary>
        /// Create an instance of the client for the given identity.
        /// </summary>
        /// <param name="clientSettings">The client settings to use</param>
        /// <param name="identity">The identity to use for the connection.</param>
        /// <returns>The node connection to use for subsequent calls</returns>
        public static async Task<IPlatformClient> CreateWithExistingIdentity(ClientSettings clientSettings, IdentityDetails identity)
        {
            return await Create(clientSettings, identity, false);
        }

        /// <summary>
        /// Create an instance of the client for the new identity. As it is a new identity then it will create the new identity on the platform ready for use.
        /// </summary>
        /// <param name="clientSettings">The client settings to use</param>
        /// <param name="identity">The identity to use for the connection.</param>
        /// <returns>The node connection to use for subsequent calls</returns>
        public static async Task<IPlatformClient> CreateWithNewIdentity(ClientSettings clientSettings, IdentityDetails identity)
        {
            return await Create(clientSettings, identity, true);
        }

        private static async Task<IPlatformClient> Create(ClientSettings clientSettings, IdentityDetails identity, bool isNewIdentity)
        {
            var client = new PlatformClient(clientSettings);
            await client.Init(identity, isNewIdentity);
            return client;
        }
    }
}