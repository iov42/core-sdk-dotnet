using System.Threading.Tasks;
using BouncyCastleCrypto;
using IntegrationTests.Support;
using Iov42sdk.Connection;
using Iov42sdk.Models;
using Iov42sdk.Models.IssueIdentity;
using Iov42sdk.Support;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests
{
    [TestClass]
    public class TestIdentityCalls
    {
        private static IntegrationTestCreation _test;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            // Run one set of tests with RSA encryption to check it works
            _test = new IntegrationTestCreation(() => new RsaCryptoEngine());
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            _test?.Dispose();
        }

        [TestMethod]
        public async Task ShouldCreateANewIdentity()
        {
            var newId = _test.IdentityBuilder.Create();
            var issueIdentityResponse = await _test.Client.CreateIdentity(newId);

            Assert.IsTrue(issueIdentityResponse.Success);
            Assert.IsNotNull(issueIdentityResponse.Value.RequestId);
            Assert.IsNotNull(issueIdentityResponse.Value.Proof);
            Assert.IsFalse(issueIdentityResponse.Value.RequestIdReusable);
            Assert.IsNull(issueIdentityResponse.Value.Errors);
        }

        [TestMethod]
        public async Task ShouldGetIdentity()
        {
            var getIdentityResponse = await _test.Client.GetIdentity(_test.Identity.Id);
            
            Assert.IsTrue(getIdentityResponse.Success);
            Assert.IsNotNull(getIdentityResponse.Value.IdentityId);
            Assert.IsNotNull(getIdentityResponse.Value.Proof);
            Assert.AreEqual(_test.Identity.Crypto.Pair.PublicKeyBase64String, getIdentityResponse.Value.PublicCredentials[0].Key);
        }

        [TestMethod]
        public async Task ShouldGetIdentityPublicKey()
        {
            var getPublicKeyResponse = await _test.Client.GetIdentityPublicKey(_test.Identity.Id);
            
            Assert.IsTrue(getPublicKeyResponse.Success);
            Assert.IsNotNull(getPublicKeyResponse.Value.Key);
            Assert.IsNotNull(getPublicKeyResponse.Value.ProtocolId);
            Assert.AreEqual(_test.Identity.Crypto.Pair.PublicKeyBase64String, getPublicKeyResponse.Value.Key);
        }

        [TestMethod]
        public async Task ShouldGetIdentityPublicKeyByAnotherIdentity()
        {
            var other = new IntegrationTestCreation();
            var getPublicKeyResponse = await other.Client.GetIdentityPublicKey(_test.Identity.Id);
            
            Assert.IsTrue(getPublicKeyResponse.Success);
            Assert.IsNotNull(getPublicKeyResponse.Value.Key);
            Assert.IsNotNull(getPublicKeyResponse.Value.ProtocolId);
            Assert.AreEqual(_test.Identity.Crypto.Pair.PublicKeyBase64String, getPublicKeyResponse.Value.Key);
        }

        [TestMethod]
        public async Task ShouldReuseAnIdentity()
        {
            var newId = _test.IdentityBuilder.Create();
            
            var _ = await _test.Client.CreateIdentity(newId);
            var newClient = await ClientBuilder.CreateWithNewIdentity(TestEnvironment.DefaultClientSettings, newId);
            var horse = _test.CreateUniqueId("Horse")+newId.Id;
            var __ = await newClient.CreateUniqueAssetType(horse);
            
            // Rebuild the id from the underlying keypair strings and the id
            var serialized = newId.Crypto.Pair.Serialize();
            var privateKey = serialized.PrivateKey;
            var publicKey = serialized.PublicKey;
            var newKeyPair = new SerializedKeys(privateKey, publicKey);
            var pair = new BouncyKeyPair(newKeyPair);
            var reuse = _test.IdentityBuilder.Create(newId.Id, pair);
            
            Assert.AreEqual(newId.Id, reuse.Id);
            Assert.AreEqual(newId.Crypto.ProtocolId, reuse.Crypto.ProtocolId);
            Assert.AreEqual(newId.Crypto.Pair.PrivateKeyBase64String, reuse.Crypto.Pair.PrivateKeyBase64String);
            Assert.AreEqual(newId.Crypto.Pair.PublicKeyBase64String, reuse.Crypto.Pair.PublicKeyBase64String);

            // Create a client using the reused identity
            var reuseClient = await ClientBuilder.CreateWithExistingIdentity(TestEnvironment.DefaultClientSettings, reuse);
            
            // Test an authenticated call using the reused identity
            var response = await reuseClient.CreateUniqueAsset(_test.CreateUniqueId("Trevor"), horse);
            Assert.IsTrue(response.Success);
        }

        [TestMethod]
        public async Task ShouldFailReuseANonExistentIdentity()
        {
            var newId = _test.IdentityBuilder.Create();
            var _ = await _test.Client.CreateIdentity(newId);

            var horse = _test.CreateUniqueId("Horse") + newId.Id;
            var __ = await _test.Client.CreateUniqueAssetType(horse);

            // This will generate a new key pair which won't match the original
            var reuse = _test.IdentityBuilder.Create();
            
            // Create a client using the reused identity
            var reuseClient = await ClientBuilder.CreateWithExistingIdentity(TestEnvironment.DefaultClientSettings, reuse);
            
            // Test an authenticated call using the reused identity
            var response = await reuseClient.CreateUniqueAsset(_test.CreateUniqueId("Trevor"), horse);
            
            Assert.IsFalse(response.Success);
        }

        [TestMethod]
        public async Task ShouldCreateANewIdentityUsingRequest()
        {
            var newId = _test.IdentityBuilder.Create();
            var body = new IssueIdentityBody(newId.Id, new Credentials(newId.Crypto.Pair.PublicKeyBase64String, newId.Crypto.ProtocolId));
            var request = _test.Client.BuildRequest(body, new[] {newId}, newId);
            var issueIdentityResponse = await _test.Client.Write(request);
            issueIdentityResponse.VerifyWriteResult();
        }

        [TestMethod]
        public async Task ShouldCreateANewIdentityUsingRawRequest()
        {
            var newId = _test.IdentityBuilder.Create();
            var body = new IssueIdentityBody(newId.Id, new Credentials(newId.Crypto.Pair.PublicKeyBase64String, newId.Crypto.ProtocolId));
            var bodyText = body.Serialize();
            var authorisations = new[] { _test.Client.GenerateAuthorisation(bodyText, newId) };
            var authentication = _test.Client.GenerateAuthentication(authorisations, newId);
            var request = new PlatformWriteRequest(body.RequestId, bodyText, authorisations, authentication);
            var issueIdentityResponse = await _test.Client.Write(request);
            issueIdentityResponse.VerifyWriteResult();
        }

        // TODO: Add permission tests
    }
}