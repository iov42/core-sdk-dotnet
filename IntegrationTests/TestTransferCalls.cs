using System.Threading.Tasks;
using IntegrationTests.Support;
using Iov42sdk.Support;
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
            
            var request = new TransferBuilder(_test.Client)
                .AddOwnershipTransfer(trevorId, horseId, _test.Identity.Id, bruce.Id)
                .Build();
            var response = await _test.Client.Write(request);
            
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
            await _test.Client.CreateQuantifiableAssetType(gbpId, 2);

            var account = _test.CreateUniqueId("AccountGBP");
            await _test.Client.CreateQuantifiableAccount(account, gbpId, 1000);

            var _ = await _test.Client.AddBalance(account, gbpId, 50);
            
            var bruceClient = new IntegrationTestCreation();
            var bruceAccount = bruceClient.CreateUniqueId("AccountGBP");
            await bruceClient.Client.CreateQuantifiableAccount(bruceAccount, gbpId, 1000);
            

            var request = new TransferBuilder(_test.Client)
                .AddQuantityTransfer(account, bruceAccount, gbpId, 10)
                .Build();
            var response = await _test.Client.Write(request);

            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Value.RequestId);
            Assert.IsNotNull(response.Value.Proof);
            Assert.IsNotNull(response.Value.Resources);
            Assert.AreEqual(2, response.Value.Resources.Length);
            Assert.IsNull(response.Value.Errors);
        }

        [TestMethod]
        public async Task ShouldTransferACombinationOfAssets()
        {
            var horseId = _test.CreateUniqueId("horse");
            await _test.Client.CreateUniqueAssetType(horseId);

            var trevorId = _test.CreateUniqueId("trevor");
            var _ = await _test.Client.CreateUniqueAsset(trevorId, horseId);
            
            var gbpId = _test.CreateUniqueId("GBP");
            await _test.Client.CreateQuantifiableAssetType(gbpId, 2);
            
            var account = _test.CreateUniqueId("AccountGBP");
            await _test.Client.CreateQuantifiableAccount(account, gbpId, 1000);
            
            var __ = await _test.Client.AddBalance(account, gbpId, 50);
            
            var bruceClient = new IntegrationTestCreation();
            var bruceAccount = bruceClient.CreateUniqueId("AccountGBP");
            await bruceClient.Client.CreateQuantifiableAccount(bruceAccount, gbpId, 1000);
            

            var authorisations = new TransferBuilder(_test.Client)
                .AddOwnershipTransfer(trevorId, horseId, _test.Identity.Id, bruceClient.Identity.Id)
                .AddQuantityTransfer(bruceAccount, account, gbpId, 10)
                .GenerateAuthorisations();
            // Pass it to Bruce to sign
            var request = new TransferBuilder(bruceClient.Client, authorisations)
                .Build();
            var response = await bruceClient.Client.Write(request);
            response.VerifyWriteResult(3);
        }
    }
}