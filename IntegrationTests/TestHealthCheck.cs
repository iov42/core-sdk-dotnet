using System.Threading.Tasks;
using IntegrationTests.Support;
using Iov42sdk.Models.Health;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests
{
    [TestClass]
    public class TestHealthCheck
    {
        [TestMethod]
        public async Task ShouldFetchStatus()
        {
            using var test = new IntegrationTestCreation();
            var healthStatus = await test.Client.GetHealthStatus();

            Assert.IsNotNull(healthStatus);
            Assert.IsTrue(healthStatus.Success);
            var status = healthStatus.Value;
            CheckBuildInfo(status.BuildInfo);
            CheckBrokerStatus(status.Broker);
            CheckStoreStatus(status.AssetStore);
            CheckStoreStatus(status.ClaimStore);
            CheckStoreStatus(status.EndorsementStore);
            CheckStoreStatus(status.PermissionStore);
            CheckStoreStatus(status.TransactionStore);
            CheckStoreStatus(status.TransferStore);
            CheckStoreStatus(status.TransferByIdentityStore);
            Assert.IsTrue(status.Hsm.HasKeys);
        }

        private static void CheckStoreStatus(StoreHealthStatus status)
        {
            Assert.IsNotNull(status);
            Assert.IsTrue(status.CanRead);
            Assert.IsTrue(status.CanWrite);
        }

        private static void CheckBrokerStatus(BrokerHealthStatus status)
        {
            Assert.IsNotNull(status);
            Assert.IsTrue(status.CanWrite);
        }

        private static void CheckBuildInfo(BuildInfo buildInfo)
        {
            Assert.IsNotNull(buildInfo);
            Assert.IsNotNull(buildInfo.Name);
            Assert.IsNotNull(buildInfo.Version);
            Assert.IsNotNull(buildInfo.ScalaVersion);
            Assert.IsNotNull(buildInfo.SbtVersion);
        }
    }
}
