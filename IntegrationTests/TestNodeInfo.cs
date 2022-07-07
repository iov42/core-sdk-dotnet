using System.Threading.Tasks;
using IntegrationTests.Support;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests
{
    [TestClass]
    public class TestNodeInfo
    {
        [TestMethod]
        public async Task ShouldFetchNodeInfo()
        {
            using var test = new IntegrationTestCreation();
            var info = await TestHelper.CallAndRetry(() =>test.Client.GetNodeInfo());

            Assert.IsNotNull(info);
            Assert.IsTrue(info.Success);
            var nodeInfo = info.Value;
            Assert.IsNotNull(nodeInfo);
            Assert.IsNotNull(nodeInfo.NodeId);
            Assert.IsNotNull(nodeInfo.PublicCredentials);
            Assert.IsNotNull(nodeInfo.PublicCredentials.Key);
            Assert.IsNotNull(nodeInfo.PublicCredentials.ProtocolId);
        }
    }
}