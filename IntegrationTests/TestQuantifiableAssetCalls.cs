using System.Threading.Tasks;
using IntegrationTests.Support;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests
{
    [TestClass]
    public class TestQuantifiableAssetCalls
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
        public async Task ShouldCreateANewQuantifiableAssetType()
        {
            var gbpId = _test.CreateUniqueId("GBP");
            var newQuantifiableAssetTypeResponse = await _test.Client.CreateQuantifiableAssetType(gbpId, 2);
            Assert.IsTrue(newQuantifiableAssetTypeResponse.Success);
            Assert.IsNotNull(newQuantifiableAssetTypeResponse.Value.RequestId);
            Assert.IsNotNull(newQuantifiableAssetTypeResponse.Value.Proof);
            Assert.IsNotNull(newQuantifiableAssetTypeResponse.Value.Resources);
            Assert.AreEqual(1, newQuantifiableAssetTypeResponse.Value.Resources.Length);
            Assert.IsNull(newQuantifiableAssetTypeResponse.Value.Errors);
        }

        [TestMethod]
        public async Task ShouldGetAQuantifiableAssetType()
        {
            var gbpId = _test.CreateUniqueId("GBP");
            await _test.Client.CreateQuantifiableAssetType(gbpId, 2);
            var getAssetTypeResponse = await _test.Client.GetQuantifiableAssetType(gbpId);
            Assert.IsTrue(getAssetTypeResponse.Success);
            Assert.AreEqual(gbpId, getAssetTypeResponse.Value.AssetTypeId);
            Assert.AreEqual(2, getAssetTypeResponse.Value.Scale);
            Assert.AreEqual(_test.Identity.Id, getAssetTypeResponse.Value.OwnerId);
            Assert.AreEqual("Quantifiable", getAssetTypeResponse.Value.Type);
            Assert.IsNotNull(getAssetTypeResponse.Value.Proof);
        }

        [TestMethod]
        public async Task ShouldCreateANewQuantifiableAsset()
        {
            var gbpId = _test.CreateUniqueId("GBP");
            var account = _test.CreateUniqueId("AccountGBP");
            await _test.Client.CreateQuantifiableAssetType(gbpId, 2);
            var newQuantifiableAssetResponse = await _test.Client.CreateQuantifiableAccount(account, gbpId, 1000);
            Assert.IsTrue(newQuantifiableAssetResponse.Success);
            Assert.IsNotNull(newQuantifiableAssetResponse.Value.RequestId);
            Assert.IsNotNull(newQuantifiableAssetResponse.Value.Proof);
            Assert.IsNotNull(newQuantifiableAssetResponse.Value.Resources);
            Assert.AreEqual(1, newQuantifiableAssetResponse.Value.Resources.Length);
            Assert.IsNull(newQuantifiableAssetResponse.Value.Errors);
        }

        [TestMethod]
        public async Task ShouldGetANewQuantifiableAsset()
        {
            var gbpId = _test.CreateUniqueId("GBP");
            var account = _test.CreateUniqueId("AccountGBP");
            await _test.Client.CreateQuantifiableAssetType(gbpId, 2);
            await _test.Client.CreateQuantifiableAccount(account, gbpId, 1000);
            var getQuantifiableAssetResponse = await _test.Client.GetQuantifiableAsset(account, gbpId);
            Assert.IsTrue(getQuantifiableAssetResponse.Success);
            Assert.AreEqual(getQuantifiableAssetResponse.Value.Quantity, "1000");
            Assert.AreEqual(getQuantifiableAssetResponse.Value.AssetId, account);
            Assert.AreEqual(getQuantifiableAssetResponse.Value.AssetTypeId, gbpId);
            Assert.AreEqual(getQuantifiableAssetResponse.Value.OwnerId, _test.Identity.Id);
            Assert.IsNotNull(getQuantifiableAssetResponse.Value.Proof);
        }

        [TestMethod]
        public async Task ShouldAddBalance()
        {
            var gbpId = _test.CreateUniqueId("GBP");
            var account = _test.CreateUniqueId("AccountGBP");
            await _test.Client.CreateQuantifiableAssetType(gbpId, 2);
            await _test.Client.CreateQuantifiableAccount(account, gbpId, 1000);
            var result = await _test.Client.AddBalance(account, gbpId, 50);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Value.RequestId);
            Assert.IsNotNull(result.Value.Proof);
            Assert.IsNotNull(result.Value.Resources);
            Assert.IsNull(result.Value.Errors);
            Assert.AreEqual(1, result.Value.Resources.Length);
            var getQuantifiableAssetResponse = await _test.Client.GetQuantifiableAsset(account, gbpId);
            Assert.AreEqual(getQuantifiableAssetResponse.Value.Quantity, "1050");
            Assert.AreEqual(getQuantifiableAssetResponse.Value.AssetId, account);
            Assert.AreEqual(getQuantifiableAssetResponse.Value.AssetTypeId, gbpId);
            Assert.AreEqual(getQuantifiableAssetResponse.Value.OwnerId, _test.Identity.Id);
            Assert.IsNotNull(getQuantifiableAssetResponse.Value.Proof);
        }
    }
}