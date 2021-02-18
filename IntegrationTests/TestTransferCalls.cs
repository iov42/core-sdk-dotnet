using System.Linq;
using System.Threading.Tasks;
using IntegrationTests.Support;
using Iov42sdk.Models.Transfers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests
{
    [TestClass]
    public class TestTransferCalls
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
        public async Task ShouldTransferAUniqueAsset()
        {
            var horseId = _test.CreateUniqueId("horse");
            await _test.Client.CreateUniqueAssetType(horseId);
            var trevorId = _test.CreateUniqueId("trevor");
            var _ = await _test.Client.CreateUniqueAsset(trevorId, horseId);
            var bruce = _test.IdentityBuilder.Create();
            var __ = await _test.Client.CreateIdentity(bruce);
            var transfer = _test.Client.CreateOwnershipTransfer(trevorId, horseId, _test.Identity.Id, bruce.Id);
            var response = await _test.Client.TransferAssets(transfer);
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Value.RequestId);
            Assert.IsNotNull(response.Value.Proof);
            Assert.IsNotNull(response.Value.Resources);
            Assert.AreEqual(1, response.Value.Resources.Length);
            Assert.IsNull(response.Value.Errors);
        }

        [TestMethod]
        public async Task ShouldTransferAQuantifiableAsset()
        {
            var gbpId = _test.CreateUniqueId("GBP");
            var account = _test.CreateUniqueId("AccountGBP");
            await _test.Client.CreateQuantifiableAssetType(gbpId, 2);
            await _test.Client.CreateQuantifiableAccount(account, gbpId, 1000);
            var _ = await _test.Client.AddBalance(account, gbpId, 50);
            var bruceClient = new IntegrationTestCreation();
            var bruceAccount = bruceClient.CreateUniqueId("AccountGBP");
            await bruceClient.Client.CreateQuantifiableAccount(bruceAccount, gbpId, 1000);
            var transfer = _test.Client.CreateQuantityTransfer(account, bruceAccount, gbpId, 10);
            var response = await _test.Client.TransferAssets(transfer);
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Value.RequestId);
            Assert.IsNotNull(response.Value.Proof);
            Assert.IsNotNull(response.Value.Resources);
            Assert.AreEqual(2, response.Value.Resources.Length);
            Assert.IsNull(response.Value.Errors);
        }

        [TestMethod]
        public async Task ShouldTransferACombinationOfAsset()
        {
            var horseId = _test.CreateUniqueId("horse");
            await _test.Client.CreateUniqueAssetType(horseId);
            var trevorId = _test.CreateUniqueId("trevor");
            var _ = await _test.Client.CreateUniqueAsset(trevorId, horseId);
            var gbpId = _test.CreateUniqueId("GBP");
            var account = _test.CreateUniqueId("AccountGBP");
            await _test.Client.CreateQuantifiableAssetType(gbpId, 2);
            await _test.Client.CreateQuantifiableAccount(account, gbpId, 1000);
            var __ = await _test.Client.AddBalance(account, gbpId, 50);
            var bruceClient = new IntegrationTestCreation();
            var bruceAccount = bruceClient.CreateUniqueId("AccountGBP");
            await bruceClient.Client.CreateQuantifiableAccount(bruceAccount, gbpId, 1000);
            var uniqueTransfer = _test.Client.CreateOwnershipTransfer(trevorId, horseId, _test.Identity.Id, bruceClient.Identity.Id);
            var quantityTransfer = _test.Client.CreateQuantityTransfer(account, bruceAccount, gbpId, 10);
            var testTransferRequest = new TransferRequest(quantityTransfer, uniqueTransfer)
                .AddAuthorisation(_test.Client.GenerateAuthorisation);
            var body = testTransferRequest.Body;
            // Pass the body to Bruce to sign
            var bruceAuthorisations = new TransferRequest(body)
                .AddAuthorisation(bruceClient.Client.GenerateAuthorisation)
                .Authorisations.ToArray();
            // Bruce now returns his authotisations
            testTransferRequest.AddAuthorisations(bruceAuthorisations);
            var response = await _test.Client.TransferAssets(testTransferRequest);
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Value.RequestId);
            Assert.IsNotNull(response.Value.Proof);
            Assert.IsNotNull(response.Value.Resources);
            Assert.AreEqual(3, response.Value.Resources.Length);
            Assert.IsNull(response.Value.Errors);
        }
    }
}