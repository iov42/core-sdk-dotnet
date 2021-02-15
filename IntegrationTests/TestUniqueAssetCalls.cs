using System.Threading.Tasks;
using IntegrationTests.Support;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests
{
    [TestClass]
    public class TestUniqueAssetCalls
    {
        private static IntegrationTestCreation _test;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            _test = new IntegrationTestCreation();
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            _test?.Dispose();
        }

        [TestMethod]
        public async Task ShouldCreateANewUniqueAssetType()
        {
            var horseId = _test.CreateUniqueId("horse");
            var newUniqueAssetTypeResponse = await _test.Client.CreateUniqueAssetType(horseId);
            Assert.IsTrue(newUniqueAssetTypeResponse.Success);
            Assert.IsNotNull(newUniqueAssetTypeResponse.Value.RequestId);
            Assert.IsNotNull(newUniqueAssetTypeResponse.Value.Proof);
            Assert.IsNotNull(newUniqueAssetTypeResponse.Value.Resources);
            Assert.AreEqual(1, newUniqueAssetTypeResponse.Value.Resources.Length);
            Assert.IsNull(newUniqueAssetTypeResponse.Value.Errors);
        }

        [TestMethod]
        public async Task ShouldGetAUniqueAssetType()
        {
            var horseId = _test.CreateUniqueId("horse");
            await _test.Client.CreateUniqueAssetType(horseId);
            var getAssetTypeResponse = await _test.Client.GetUniqueAssetType(horseId);
            Assert.IsTrue(getAssetTypeResponse.Success);
            Assert.AreEqual(horseId, getAssetTypeResponse.Value.AssetTypeId);
            Assert.AreEqual(_test.Identity.Id, getAssetTypeResponse.Value.OwnerId);
            Assert.IsNotNull(getAssetTypeResponse.Value.Proof);
            Assert.AreEqual("Unique", getAssetTypeResponse.Value.Type);
        }

        [TestMethod]
        public async Task ShouldCreateANewUniqueAsset()
        {
            var horseId = _test.CreateUniqueId("horse");
            await _test.Client.CreateUniqueAssetType(horseId);
            var trevorId = _test.CreateUniqueId("trevor");
            var newUniqueAssetResponse = await _test.Client.CreateUniqueAsset(trevorId, horseId);
            Assert.IsTrue(newUniqueAssetResponse.Success);
            Assert.IsNotNull(newUniqueAssetResponse.Value.RequestId);
            Assert.IsNotNull(newUniqueAssetResponse.Value.Proof);
            Assert.IsNotNull(newUniqueAssetResponse.Value.Resources);
            Assert.AreEqual(1, newUniqueAssetResponse.Value.Resources.Length);
            Assert.IsNull(newUniqueAssetResponse.Value.Errors);
        }

        [TestMethod]
        public async Task ShouldGetANewUniqueAsset()
        {
            var horseId = _test.CreateUniqueId("horse");
            await _test.Client.CreateUniqueAssetType(horseId);
            var trevorId = _test.CreateUniqueId("trevor");
            await _test.Client.CreateUniqueAsset(trevorId, horseId);
            var getUniqueAssetResponse = await _test.Client.GetUniqueAsset(trevorId, horseId);
            Assert.IsTrue(getUniqueAssetResponse.Success);
            Assert.AreEqual(getUniqueAssetResponse.Value.AssetId, trevorId);
            Assert.AreEqual(getUniqueAssetResponse.Value.AssetTypeId, horseId);
            Assert.AreEqual(getUniqueAssetResponse.Value.OwnerId, _test.Identity.Id);
            Assert.IsNotNull(getUniqueAssetResponse.Value.Proof);
        }
    }
} 