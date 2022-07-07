using System;

namespace Iov42sdk.Support
{
    public class ClientSettings
    {
        public Uri BaseAddress { get; }
        public int DelayForConsistency { get; private set; }
        public int RedirectDelay { get; set; } = 1000;
        public int ConsistencyDelay { get; } = 0;

        /// <summary>
        /// Create client settings
        /// </summary>
        /// <param name="baseAddress">The url of the environment to connect to, for example "https://some.environment.com"</param>
        /// <param name="apiLocation">The path to the api version (default is "/api/v1/")</param>
        public ClientSettings(string baseAddress, string apiLocation = "/api/v1/")
        {
            BaseAddress = new Uri(new Uri(baseAddress), apiLocation);
        }

        /// <summary>
        /// Add in a delay to any read operation after a write operation to allow for
        /// eventual consistency - default is off
        /// </summary>
        /// <param name="delay">The delay in ms</param>
        /// <returns></returns>
        public ClientSettings WithConsistencyDelay(int delay)
        {
            DelayForConsistency = delay;
            return this;
        }

        /// <summary>
        /// Change the delay before retrying when the platform returns a redirect
        /// </summary>
        /// <param name="delay">Millisecond delay (default is 1000)</param>
        /// <returns></returns>
        public ClientSettings WithRedirectDelay(int delay)
        {
            RedirectDelay = delay;
            return this;
        }
    }
}
