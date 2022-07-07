using System.Threading.Tasks;
using IntegrationTests.Support;
using Iov42sdk.Connection;
using Iov42sdk.Models;
using Iov42sdk.Support.Permissions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests.Permissions
{
    [TestClass]
    public class TestIdentityPermissionsCalls
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
        public async Task ShouldAllowCreateANewIdentity()
        {
            var alice = _test.IdentityBuilder.Create();
            var permissions = new CreateIdentityPermissionBuilder()
                .WithCreateIdentity(true)
                .Build();

            var createIdentityResponse = await _test.Client.CreateIdentity(alice, permissions);
            Assert.IsTrue(createIdentityResponse.Success);

            var aliceClient = await ClientBuilder.CreateWithExistingIdentity(TestEnvironment.DefaultClientSettings, alice);

            // Try and create a new identity
            var bob = _test.IdentityBuilder.Create();
            var response = await aliceClient.CreateIdentity(bob);
            Assert.IsTrue(response.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowCreateANewIdentity()
        {
            var alice = _test.IdentityBuilder.Create();
            var permissions = new CreateIdentityPermissionBuilder()
                .WithCreateIdentity(false)
                .Build();

            var createIdentityResponse = await _test.Client.CreateIdentity(alice, permissions);
            Assert.IsTrue(createIdentityResponse.Success);

            var aliceClient = await ClientBuilder.CreateWithExistingIdentity(TestEnvironment.DefaultClientSettings, alice);

            // Try and create a new identity
            var bob = _test.IdentityBuilder.Create();
            var response = await aliceClient.CreateIdentity(bob);
            Assert.IsFalse(response.Success);
        }

        [TestMethod]
        public async Task ShouldAllowCreateClaim()
        {
            var alice = _test.IdentityBuilder.Create();
            var permissions = new CreateIdentityPermissionBuilder()
                .WithCreateClaimForEveryone(true)
                .Build();

            var createIdentityResponse = await _test.Client.CreateIdentity(alice, permissions);
            Assert.IsTrue(createIdentityResponse.Success);

            var response = await _test.Client.CreateIdentityClaimsOnIdentity(alice.Id, "Test");
            Assert.IsTrue(response.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowCreateClaim()
        {
            var alice = _test.IdentityBuilder.Create();
            var permissions = new CreateIdentityPermissionBuilder()
                .WithCreateClaimForEveryone(false)
                .Build();

            var createIdentityResponse = await _test.Client.CreateIdentity(alice, permissions);
            Assert.IsTrue(createIdentityResponse.Success);

            var response = await _test.Client.CreateIdentityClaimsOnIdentity(alice.Id, "Test");
            Assert.IsFalse(response.Success);
        }

        [TestMethod]
        public async Task ShouldAllowReadClaim()
        {
            var alice = _test.IdentityBuilder.Create();
            var permissions = new CreateIdentityPermissionBuilder()
                .WithCreateClaimForEveryone(true)
                .WithReadClaimForIdentity(_test.Identity.Id, true)
                .Build();

            var createIdentityResponse = await _test.Client.CreateIdentity(alice, permissions);
            Assert.IsTrue(createIdentityResponse.Success);

            var response = await _test.Client.CreateIdentityClaimsOnIdentity(alice.Id, "Test");
            Assert.IsTrue(response.Success);

            var result = await TestHelper.CallAndRetry(() => _test.Client.GetIdentityClaim(alice.Id, "Test"));
            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowReadClaim()
        {
            var bob = _test.IdentityBuilder.Create();
            var bobResponse = await _test.Client.CreateIdentity(bob);
            Assert.IsTrue(bobResponse.Success);

            var alice = _test.IdentityBuilder.Create();
            var permissions = new CreateIdentityPermissionBuilder()
                .WithCreateClaimForEveryone(true)
                .WithReadClaimForIdentity(bob.Id, true) // Set Bob with permissions
                .Build();

            var createIdentityResponse = await _test.Client.CreateIdentity(alice, permissions);
            Assert.IsTrue(createIdentityResponse.Success);

            var response = await _test.Client.CreateIdentityClaimsOnIdentity(alice.Id, "Test");
            Assert.IsTrue(response.Success);

            // Read with not Bob
            var result = await TestHelper.CallAndRetry(() => _test.Client.GetIdentityClaim(alice.Id, "Test"));
            Assert.IsFalse(result.Success);
        }

        [TestMethod]
        public async Task ShouldAllowCreateEndorsement()
        {
            var alice = _test.IdentityBuilder.Create();
            var permissions = new CreateIdentityPermissionBuilder()
                .WithCreateClaimForEveryone(true)
                .WithEndorseClaimForEveryone(true)
                .Build();

            var createIdentityResponse = await _test.Client.CreateIdentity(alice, permissions);
            Assert.IsTrue(createIdentityResponse.Success);

            var response = await _test.Client.CreateIdentityClaimsOnIdentity(alice.Id, "Test");
            Assert.IsTrue(response.Success);

            var endorsements = _test.Client.CreateIdentityEndorsements(alice.Id)
                .AddEndorsement("Test");
            var body = endorsements.GenerateIdentityEndorsementBody().Serialize();
            var header = _test.Client.GenerateAuthorisation(body);
            var result = await _test.Client.CreateIdentityClaimsEndorsements(endorsements, endorsements.RequestId, body, header);
            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowCreateEndorsement()
        {
            var alice = _test.IdentityBuilder.Create();
            var permissions = new CreateIdentityPermissionBuilder()
                .WithCreateClaimForEveryone(true)
                .WithEndorseClaimForIdentity(alice.Id, true)
                .Build();

            var createIdentityResponse = await _test.Client.CreateIdentity(alice, permissions);
            Assert.IsTrue(createIdentityResponse.Success);

            var response = await _test.Client.CreateIdentityClaimsOnIdentity(alice.Id, "Test");
            Assert.IsTrue(response.Success);

            var endorsements = _test.Client.CreateIdentityEndorsements(alice.Id)
                .AddEndorsement("Test");
            var body = endorsements.GenerateIdentityEndorsementBody().Serialize();
            var header = _test.Client.GenerateAuthorisation(body);
            var result = await _test.Client.CreateIdentityClaimsEndorsements(endorsements, endorsements.RequestId, body, header);
            Assert.IsFalse(result.Success);
        }

        [TestMethod]
        public async Task ShouldAllowReadEndorsement()
        {
            var alice = _test.IdentityBuilder.Create();
            var permissions = new CreateIdentityPermissionBuilder()
                .WithCreateClaimForEveryone(true)
                .WithEndorseClaimForEveryone(true)
                .WithReadEndorsementForEveryone(true)
                .Build();

            var createIdentityResponse = await _test.Client.CreateIdentity(alice, permissions);
            Assert.IsTrue(createIdentityResponse.Success);

            var response = await _test.Client.CreateIdentityClaimsOnIdentity(alice.Id, "Test");
            Assert.IsTrue(response.Success);

            var endorsements = _test.Client.CreateIdentityEndorsements(alice.Id)
                .AddEndorsement("Test");
            var body = endorsements.GenerateIdentityEndorsementBody().Serialize();
            var header = _test.Client.GenerateAuthorisation(body);
            var endorse = await _test.Client.CreateIdentityClaimsEndorsements(endorsements, endorsements.RequestId, body, header);
            Assert.IsTrue(endorse.Success);
            var result = await TestHelper.CallAndRetry(() => _test.Client.GetIdentityEndorsement(alice.Id, "Test", _test.Identity.Id));
            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowReadEndorsement()
        {
            var alice = _test.IdentityBuilder.Create();
            var permissions = new CreateIdentityPermissionBuilder()
                .WithCreateClaimForEveryone(true)
                .WithEndorseClaimForEveryone(true)
                .WithReadEndorsementForIdentity(_test.Identity.Id, false)
                .Build();

            var createIdentityResponse = await _test.Client.CreateIdentity(alice, permissions);
            Assert.IsTrue(createIdentityResponse.Success);

            var response = await _test.Client.CreateIdentityClaimsOnIdentity(alice.Id, "Test");
            Assert.IsTrue(response.Success);

            var endorsements = _test.Client.CreateIdentityEndorsements(alice.Id)
                .AddEndorsement("Test");
            var body = endorsements.GenerateIdentityEndorsementBody().Serialize();
            var header = _test.Client.GenerateAuthorisation(body);
            var endorse = await _test.Client.CreateIdentityClaimsEndorsements(endorsements, endorsements.RequestId, body, header);
            Assert.IsTrue(endorse.Success);
            var result = await TestHelper.CallAndRetry(() => _test.Client.GetIdentityEndorsement(alice.Id, "Test", _test.Identity.Id));
            Assert.IsFalse(result.Success);
        }
    }
}