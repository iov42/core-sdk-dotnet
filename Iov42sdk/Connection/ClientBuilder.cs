using System.Threading.Tasks;
using Iov42sdk.Identity;

namespace Iov42sdk.Connection
{
    public class ClientBuilder
    {
        /// <summary>
        /// Create an instance of the client for the given identity.
        /// </summary>
        /// <param name="baseUrl">The base url of the node to connect to (for example https://someservername/api/v1/) </param>
        /// <param name="identity">The identity to use for the connection.</param>
        /// <returns>The node connection to use for subsequent calls</returns>
        public static async Task<IPlatformClient> CreateWithExistingIdentity(string baseUrl, IdentityDetails identity)
        {
            return await Create(baseUrl, identity, false);
        }

        /// <summary>
        /// Create an instance of the client for the new identity. As it is a new identity then it will create the new identity on the platform ready for use.
        /// </summary>
        /// <param name="baseUrl">The base url of the node to connect to (for example https://someservername/api/v1/) </param>
        /// <param name="identity">The identity to use for the connection.</param>
        /// <returns>The node connection to use for subsequent calls</returns>
        public static async Task<IPlatformClient> CreateWithNewIdentity(string baseUrl, IdentityDetails identity)
        {
            return await Create(baseUrl, identity, true);
        }

        private static async Task<IPlatformClient> Create(string baseUrl, IdentityDetails identity, bool isNewIdentity)
        {
            var client = new PlatformClient(baseUrl);
            await client.Init(identity, isNewIdentity);
            return client;
        }
    }
}