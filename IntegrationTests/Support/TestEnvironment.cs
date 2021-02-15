namespace IntegrationTests.Support
{
    public class TestEnvironment
    {
        static TestEnvironment()
        {
            Environment = System.Environment.GetEnvironmentVariable("IOV42_ENVIRONMENT");
        }

        public static string Environment { get; }
    }
}
