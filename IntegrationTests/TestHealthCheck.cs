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
            CheckWriteStatus(status.Broker);
            CheckReadStatus(status.RequestStore);
            CheckReadStatus(status.AssetStore);
            CheckReadStatus(status.ClaimStore);
            CheckReadStatus(status.EndorsementStore);
            CheckReadStatus(status.ProofStore);
            Assert.IsTrue(status.Hsm.HasKeys);
        }

        private static void CheckReadStatus(ReadServiceStatus status, bool expectedRead = true, bool expectedWrite = true)
        {
            Assert.IsNotNull(status);
            Assert.AreEqual(expectedRead, status.CanRead);
            Assert.AreEqual(expectedWrite, status.CanWrite);
        }

        private static void CheckWriteStatus(WriteServiceStatus status, bool expectedWrite = true)
        {
            Assert.IsNotNull(status);
            Assert.AreEqual(expectedWrite, status.CanWrite);
        }
    }
}
