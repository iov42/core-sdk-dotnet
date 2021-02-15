using System.Threading.Tasks;
using IntegrationTests.Support;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests
{
    [TestClass]
    public class TestRequestStatus
    {
        [TestMethod]
        public async Task ShouldGetRequestStatus()
        {
            using var test = new IntegrationTestCreation();
            var newId = test.IdentityBuilder.Create();
            var issueIdentityResponse = await test.Client.CreateIdentity(newId);
            var status = await test.Client.GetRequestStatus(issueIdentityResponse.Value.RequestId);
            Assert.IsTrue(status.Success);
            Assert.IsNotNull(status);
            Assert.AreEqual(issueIdentityResponse.Value.RequestId, status.Value.RequestId);
            Assert.IsNotNull(status.Value.Proof);
            Assert.IsNotNull(status.Value.Resources);
            Assert.AreEqual(1, status.Value.Resources.Length);
            Assert.IsNotNull(status.Value.Resources[0]);
        }
    }
}
