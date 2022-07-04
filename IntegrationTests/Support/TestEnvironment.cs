using Iov42sdk.Support;

namespace IntegrationTests.Support
{
    public class TestEnvironment
    {
        static TestEnvironment()
        {
            Environment = System.Environment.GetEnvironmentVariable("IOV42_ENVIRONMENT");
            DefaultClientSettings = new ClientSettings(Environment)
                .WithReadDelayForConsistency()
                .WithRedirectDelay(500);
        }

        public static ClientSettings DefaultClientSettings { get; }

        public static string Environment { get; }
    }
}
