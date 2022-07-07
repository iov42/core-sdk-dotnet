using System.Linq;
using System.Threading.Tasks;
using IntegrationTests.Support;
using Iov42sdk.Connection;
using Iov42sdk.Models.AddDelegate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests
{
    [TestClass]
    public class TestDelegates
    {
        [TestMethod]
        public async Task ShouldCreateANewDelegate()
        {
            using var test = new IntegrationTestCreation();
            var delegateId = test.IdentityBuilder.Create();
            var _ = await test.Client.CreateIdentity(delegateId);

            var response = await test.Client.AddDelegate(delegateId);
            
            Assert.IsTrue(response.Success);
            Assert.IsFalse(response.Value.RequestIdReusable);
            Assert.IsNotNull(response.Value.RequestId);
            Assert.IsNotNull(response.Value.Proof);
            Assert.IsNull(response.Value.Errors);
        }

        [TestMethod]
        public async Task ShouldGetDelegates()
        {
            using var test = new IntegrationTestCreation();
            var delegateId = test.IdentityBuilder.Create();
            
            var _ = await test.Client.CreateIdentity(delegateId);
            
            var __ = await test.Client.AddDelegate(delegateId);
            
            var response = await TestHelper.CallAndRetry(() => test.Client.GetIdentityDelegates(test.Identity.Id));
            
            Assert.IsTrue(response.Success);
            Assert.AreEqual(1, response.Value.Delegates.Length);
            Assert.AreEqual(delegateId.Id, response.Value.Delegates[0].DelegateIdentityId);
            Assert.IsNotNull(response.Value.Delegates[0].Proof);
        }

        [TestMethod]
        public async Task ShouldCreateANewAssetTypeAsADelegate()
        {
            using var test = new IntegrationTestCreation();
            var delegateId = test.IdentityBuilder.Create();
            
            var _ = await test.Client.CreateIdentity(delegateId);
            
            var response = await test.Client.AddDelegate(delegateId);
            Assert.IsTrue(response.Success);
            
            using var delegateClient = await ClientBuilder.CreateWithNewIdentity(TestEnvironment.DefaultClientSettings, delegateId);
            delegateClient.UseDelegator(test.Identity.Id);
            var horseId = test.CreateUniqueId("horse");
            var newUniqueAssetTypeResponse = await delegateClient.CreateUniqueAssetType(horseId);
            
            Assert.IsTrue(newUniqueAssetTypeResponse.Success);
            
            var proofResponse = await TestHelper.CallAndRetry(() => delegateClient.GetProof(newUniqueAssetTypeResponse.Value.RequestId));
            
            Assert.IsTrue(proofResponse.Success);
            var match = proofResponse.Value.Signatories.FirstOrDefault(x => x.DelegateIdentity != null);
            Assert.IsNotNull(match);
            Assert.AreEqual(test.Identity.Id, match.Identity);
            Assert.AreEqual(delegateId.Id, match.DelegateIdentity);
        }

        [TestMethod]
        public async Task ShouldNotCreateANewAssetTypeAsNotADelegate()
        {
            using var test = new IntegrationTestCreation();
            var delegateId = test.IdentityBuilder.Create();
            var _ = await test.Client.CreateIdentity(delegateId);
            using var delegateClient = await ClientBuilder.CreateWithNewIdentity(TestEnvironment.DefaultClientSettings, delegateId);

            // Flag as using a delegate but we have not set it up on the platform as we
            // haven't called AddDelegate
            delegateClient.UseDelegator(test.Identity.Id);
            var horseId = test.CreateUniqueId("horse");
            var newUniqueAssetTypeResponse = await delegateClient.CreateUniqueAssetType(horseId);
            
            Assert.IsFalse(newUniqueAssetTypeResponse.Success);
        }

        [TestMethod]
        public async Task ShouldStopBeingADelegate()
        {
            using var test = new IntegrationTestCreation();
            var delegateId = test.IdentityBuilder.Create();
            var _ = await test.Client.CreateIdentity(delegateId);

            var response = await test.Client.AddDelegate(delegateId);
            
            Assert.IsTrue(response.Success);
            
            using var delegateClient = await ClientBuilder.CreateWithNewIdentity(TestEnvironment.DefaultClientSettings, delegateId);
            delegateClient.UseDelegator(test.Identity.Id);
            
            var horseId = test.CreateUniqueId("horse");
            var newUniqueAssetTypeResponse = await delegateClient.CreateUniqueAssetType(horseId);
            
            Assert.IsTrue(newUniqueAssetTypeResponse.Success);
            
            var proofResponse = await TestHelper.CallAndRetry(() => delegateClient.GetProof(newUniqueAssetTypeResponse.Value.RequestId));
            
            Assert.IsTrue(proofResponse.Success);
            
            var match = proofResponse.Value.Signatories.FirstOrDefault(x => x.DelegateIdentity != null);
            Assert.IsNotNull(match);
            Assert.AreEqual(test.Identity.Id, match.Identity);
            Assert.AreEqual(delegateId.Id, match.DelegateIdentity);
            
            delegateClient.StopUsingDelegator();

            var cowId = test.CreateUniqueId("cow");
            var newUniqueAssetTypeCowResponse = await delegateClient.CreateUniqueAssetType(cowId);
            
            Assert.IsTrue(newUniqueAssetTypeCowResponse.Success);
            var proofResponseNonDelegate = await TestHelper.CallAndRetry(() => delegateClient.GetProof(newUniqueAssetTypeCowResponse.Value.RequestId));
            
            Assert.IsTrue(proofResponseNonDelegate.Success);
            var noMatch = proofResponseNonDelegate.Value.Signatories.FirstOrDefault(x => x.DelegateIdentity != null);
            Assert.IsNull(noMatch); // No entries where delegation is used
        }

        [TestMethod]
        public async Task ShouldCreateANewDelegateUsingRequest()
        {
            using var test = new IntegrationTestCreation();
            var delegateId = test.IdentityBuilder.Create();
            var _ = await test.Client.CreateIdentity(delegateId);
            var body = new AddDelegateBody(delegateId.Id, test.Identity.Id);
            var request = test.Client.BuildRequest(body, new[] { test.Identity, delegateId });
            var response = await test.Client.Write(request);
            response.VerifyWriteResult();
        }
    }
}
