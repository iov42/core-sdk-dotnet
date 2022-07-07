using System.Threading.Tasks;
using IntegrationTests.Support;
using Iov42sdk.Models.CreateAsset;
using Iov42sdk.Models.CreateAssetType;
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
            newUniqueAssetTypeResponse.VerifyWriteResult();
        }

        [TestMethod]
        public async Task ShouldGetAUniqueAssetType()
        {
            var horseId = _test.CreateUniqueId("horse");
            await _test.Client.CreateUniqueAssetType(horseId);

            var getAssetTypeResponse = await TestHelper.CallAndRetry(() =>_test.Client.GetUniqueAssetType(horseId));

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
            newUniqueAssetResponse.VerifyWriteResult();
        }

        [TestMethod]
        public async Task ShouldGetANewUniqueAsset()
        {
            var horseId = _test.CreateUniqueId("horse");
            await _test.Client.CreateUniqueAssetType(horseId);
            
            var trevorId = _test.CreateUniqueId("trevor");
            await _test.Client.CreateUniqueAsset(trevorId, horseId);
            
            var getUniqueAssetResponse = await TestHelper.CallAndRetry(() =>_test.Client.GetUniqueAsset(trevorId, horseId));
            
            Assert.IsTrue(getUniqueAssetResponse.Success);
            Assert.AreEqual(getUniqueAssetResponse.Value.AssetId, trevorId);
            Assert.AreEqual(getUniqueAssetResponse.Value.AssetTypeId, horseId);
            Assert.AreEqual(getUniqueAssetResponse.Value.OwnerId, _test.Identity.Id);
            Assert.IsNotNull(getUniqueAssetResponse.Value.Proof);
        }

        [TestMethod]
        public async Task ShouldCreateANewUniqueAssetTypeUsingRequest()
        {
            var horseId = _test.CreateUniqueId("horse");
            var body = new CreateUniqueAssetTypeBody(horseId);
            var request = _test.Client.BuildRequest(body);
            var newUniqueAssetTypeResponse = await _test.Client.Write(request);
            newUniqueAssetTypeResponse.VerifyWriteResult();
        }

        [TestMethod]
        public async Task ShouldCreateANewUniqueAssetUsingRequest()
        {
            var horseId = _test.CreateUniqueId("horse");
            await _test.Client.CreateUniqueAssetType(horseId);

            var trevorId = _test.CreateUniqueId("trevor");
            var body = new CreateUniqueAssetBody(trevorId, horseId);
            var request = _test.Client.BuildRequest(body);
            var newUniqueAssetResponse = await _test.Client.Write(request);
            newUniqueAssetResponse.VerifyWriteResult();
        }

        // TODO: Add permission tests
    }
} 