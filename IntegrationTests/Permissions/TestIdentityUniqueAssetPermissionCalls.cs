using System.Threading.Tasks;
using IntegrationTests.Support;
using Iov42sdk.Connection;
using Iov42sdk.Models;
using Iov42sdk.Support;
using Iov42sdk.Support.Permissions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests.Permissions
{
    [TestClass]
    public class TestIdentityUniqueAssetPermissionCalls
    {
        private static IntegrationTestCreation _test;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            _test = new IntegrationTestCreation(TestEnvironment.DefaultClientSettings);
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            _test?.Dispose();
        }

        [TestMethod]
        public async Task ShouldAllowCreateClaimByOwnerOnBlank()
        {
            var horseId = _test.CreateUniqueId("horse");
            var permissions = new CreateUniqueAssetTypePermissionBuilder()
                .Build();
            var newUniqueAssetTypeResponse = await _test.Client.CreateUniqueAssetType(horseId, permissions);
            Assert.IsTrue(newUniqueAssetTypeResponse.Success);

            var response = await _test.Client.CreateAssetTypeClaims(horseId, "Test");
            Assert.IsTrue(response.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowCreateClaimByAnyoneOnBlank()
        {
            var horseId = _test.CreateUniqueId("horse");
            var permissions = new CreateUniqueAssetTypePermissionBuilder()
                .Build();
            var newUniqueAssetTypeResponse = await _test.Client.CreateUniqueAssetType(horseId, permissions);
            Assert.IsTrue(newUniqueAssetTypeResponse.Success);

            var alice = _test.IdentityBuilder.Create();
            var createIdentityResponse = await _test.Client.CreateIdentity(alice);
            Assert.IsTrue(createIdentityResponse.Success);
            var aliceClient = await ClientBuilder.CreateWithExistingIdentity(TestEnvironment.DefaultClientSettings, alice);

            var response = await aliceClient.CreateAssetTypeClaims(horseId, "Test");
            Assert.IsFalse(response.Success);
        }

        [TestMethod]
        public async Task ShouldAllowCreateClaim()
        {
            var horseId = _test.CreateUniqueId("horse");
            var permissions = new CreateUniqueAssetTypePermissionBuilder()
                .WithCreateClaimForEveryone(true)
                .Build();
            var newUniqueAssetTypeResponse = await _test.Client.CreateUniqueAssetType(horseId, permissions);
            Assert.IsTrue(newUniqueAssetTypeResponse.Success);

            var alice = _test.IdentityBuilder.Create();
            var createIdentityResponse = await _test.Client.CreateIdentity(alice);
            Assert.IsTrue(createIdentityResponse.Success);
            var aliceClient = await ClientBuilder.CreateWithExistingIdentity(TestEnvironment.DefaultClientSettings, alice);

            var response = await aliceClient.CreateAssetTypeClaims(horseId, "Test");
            Assert.IsTrue(response.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowCreateClaim()
        {
            var alice = _test.IdentityBuilder.Create();
            var horseId = _test.CreateUniqueId("horse");
            var permissions = new CreateUniqueAssetTypePermissionBuilder()
                .WithCreateClaimForIdentity(alice.Id, false)
                .Build();
            var newUniqueAssetTypeResponse = await _test.Client.CreateUniqueAssetType(horseId, permissions);
            Assert.IsTrue(newUniqueAssetTypeResponse.Success);

            var createIdentityResponse = await _test.Client.CreateIdentity(alice);
            Assert.IsTrue(createIdentityResponse.Success);
            var aliceClient = await ClientBuilder.CreateWithExistingIdentity(TestEnvironment.DefaultClientSettings, alice);

            var response = await aliceClient.CreateAssetTypeClaims(horseId, "Test");
            Assert.IsFalse(response.Success);
        }

        [TestMethod]
        public async Task ShouldAllowInstanceOwnerCreateClaim()
        {
            var horseId = _test.CreateUniqueId("horse");
            var permissions = new CreateUniqueAssetTypePermissionBuilder()
                .WithCreateClaimForInstanceOwner(true)
                .Build();
            var newUniqueAssetTypeResponse = await _test.Client.CreateUniqueAssetType(horseId, permissions);
            Assert.IsTrue(newUniqueAssetTypeResponse.Success);
            var response = await _test.Client.CreateAssetTypeClaims(horseId, "Test");
            Assert.IsTrue(response.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowInstanceOwnerCreateClaim()
        {
            var horseId = _test.CreateUniqueId("horse");
            var permissions = new CreateUniqueAssetTypePermissionBuilder()
                .WithCreateClaimForInstanceOwner(false)
                .Build();
            var newUniqueAssetTypeResponse = await _test.Client.CreateUniqueAssetType(horseId, permissions);
            Assert.IsTrue(newUniqueAssetTypeResponse.Success);
            var response = await _test.Client.CreateAssetTypeClaims(horseId, "Test");
            Assert.IsFalse(response.Success);
        }

        [TestMethod]
        public async Task ShouldAllowReadClaim()
        {
            var horseId = _test.CreateUniqueId("horse");
            var permissions = new CreateUniqueAssetTypePermissionBuilder()
                .WithCreateClaimForEveryone(true)
                .WithReadClaimForEveryone(true)
                .Build();
            var newUniqueAssetTypeResponse = await _test.Client.CreateUniqueAssetType(horseId, permissions);
            Assert.IsTrue(newUniqueAssetTypeResponse.Success);

            var alice = _test.IdentityBuilder.Create();
            var createIdentityResponse = await _test.Client.CreateIdentity(alice);
            Assert.IsTrue(createIdentityResponse.Success);
            var aliceClient = await ClientBuilder.CreateWithExistingIdentity(TestEnvironment.DefaultClientSettings, alice);

            var response = await _test.Client.CreateAssetTypeClaims(horseId, "Test");
            Assert.IsTrue(response.Success);

            var result = await TestHelper.CallAndRetry(() => aliceClient.GetAssetTypeClaim(horseId, "Test"));
            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowReadClaim()
        {
            var alice = _test.IdentityBuilder.Create();
            var horseId = _test.CreateUniqueId("horse");
            var permissions = new CreateUniqueAssetTypePermissionBuilder()
                .WithCreateClaimForEveryone(true)
                .WithReadClaimForIdentity(alice.Id, false)
                .Build();
            var newUniqueAssetTypeResponse = await _test.Client.CreateUniqueAssetType(horseId, permissions);
            Assert.IsTrue(newUniqueAssetTypeResponse.Success);

            var createIdentityResponse = await _test.Client.CreateIdentity(alice);
            Assert.IsTrue(createIdentityResponse.Success);
            var aliceClient = await ClientBuilder.CreateWithExistingIdentity(TestEnvironment.DefaultClientSettings, alice);

            var response = await _test.Client.CreateAssetTypeClaims(horseId, "Test");
            Assert.IsTrue(response.Success);

            var result = await TestHelper.CallAndRetry(() => aliceClient.GetAssetTypeClaim(horseId, "Test"));
            Assert.IsFalse(result.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowReadClaimForInstanceOwner()
        {
            var horseId = _test.CreateUniqueId("horse");
            var permissions = new CreateUniqueAssetTypePermissionBuilder()
                .WithCreateClaimForEveryone(true)
                .WithReadClaimForInstanceOwner(false)
                .Build();
            var newUniqueAssetTypeResponse = await _test.Client.CreateUniqueAssetType(horseId, permissions);
            Assert.IsTrue(newUniqueAssetTypeResponse.Success);

            var response = await _test.Client.CreateAssetTypeClaims(horseId, "Test");
            Assert.IsTrue(response.Success);

            var result = await TestHelper.CallAndRetry(() => _test.Client.GetAssetTypeClaim(horseId, "Test"));
            Assert.IsFalse(result.Success);
        }

        [TestMethod]
        public async Task ShouldAllowEndorseClaim()
        {
            var horseId = _test.CreateUniqueId("horse");
            var permissions = new CreateUniqueAssetTypePermissionBuilder()
                .WithCreateClaimForEveryone(true)
                .WithEndorseClaimForEveryone(true)
                .Build();
            var newUniqueAssetTypeResponse = await _test.Client.CreateUniqueAssetType(horseId, permissions);
            Assert.IsTrue(newUniqueAssetTypeResponse.Success);

            var alice = _test.IdentityBuilder.Create();
            var createIdentityResponse = await _test.Client.CreateIdentity(alice);
            Assert.IsTrue(createIdentityResponse.Success);
            var aliceClient = await ClientBuilder.CreateWithExistingIdentity(TestEnvironment.DefaultClientSettings, alice);

            var response = await _test.Client.CreateAssetTypeClaims(horseId, "Test");
            Assert.IsTrue(response.Success);

            var endorsements = aliceClient.CreateAssetTypeEndorsements(horseId)
                .AddEndorsement("Test");
            var body = endorsements.GenerateAssetTypeEndorsementBody().Serialize();
            var testHeader = aliceClient.GenerateAuthorisation(body);
            var endorse = await aliceClient.CreateAssetTypeClaimsEndorsements(endorsements, endorsements.RequestId, body, testHeader);
            Assert.IsTrue(endorse.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowEndorseClaim()
        {
            var alice = _test.IdentityBuilder.Create();
            var horseId = _test.CreateUniqueId("horse");
            var permissions = new CreateUniqueAssetTypePermissionBuilder()
                .WithCreateClaimForEveryone(true)
                .WithEndorseClaimForIdentity(alice.Id, false)
                .Build();
            var newUniqueAssetTypeResponse = await _test.Client.CreateUniqueAssetType(horseId, permissions);
            Assert.IsTrue(newUniqueAssetTypeResponse.Success);

            var createIdentityResponse = await _test.Client.CreateIdentity(alice);
            Assert.IsTrue(createIdentityResponse.Success);
            var aliceClient = await ClientBuilder.CreateWithExistingIdentity(TestEnvironment.DefaultClientSettings, alice);

            var response = await _test.Client.CreateAssetTypeClaims(horseId, "Test");
            Assert.IsTrue(response.Success);

            var endorsements = aliceClient.CreateAssetTypeEndorsements(horseId)
                .AddEndorsement("Test");
            var body = endorsements.GenerateAssetTypeEndorsementBody().Serialize();
            var testHeader = aliceClient.GenerateAuthorisation(body);
            var endorse = await aliceClient.CreateAssetTypeClaimsEndorsements(endorsements, endorsements.RequestId, body, testHeader);
            Assert.IsFalse(endorse.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowEndorseClaimForInstanceOwner()
        {
            var horseId = _test.CreateUniqueId("horse");
            var permissions = new CreateUniqueAssetTypePermissionBuilder()
                .WithCreateClaimForEveryone(true)
                .WithEndorseClaimForInstanceOwner(false)
                .Build();
            var newUniqueAssetTypeResponse = await _test.Client.CreateUniqueAssetType(horseId, permissions);
            Assert.IsTrue(newUniqueAssetTypeResponse.Success);

            var response = await _test.Client.CreateAssetTypeClaims(horseId, "Test");
            Assert.IsTrue(response.Success);

            var endorsements = _test.Client.CreateAssetTypeEndorsements(horseId)
                .AddEndorsement("Test");
            var body = endorsements.GenerateAssetTypeEndorsementBody().Serialize();
            var testHeader = _test.Client.GenerateAuthorisation(body);
            var endorse = await _test.Client.CreateAssetTypeClaimsEndorsements(endorsements, endorsements.RequestId, body, testHeader);
            Assert.IsFalse(endorse.Success);
        }

        // Instances

        [TestMethod]
        public async Task ShouldAllowInstanceCreateClaim()
        {
            var horseId = _test.CreateUniqueId("horse");
            var permissions = new CreateUniqueAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateUniqueAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceCreateClaimForEveryone(true)
                        .BuildInstances())
                .Build();
            var newUniqueAssetTypeResponse = await _test.Client.CreateUniqueAssetType(horseId, permissions);
            Assert.IsTrue(newUniqueAssetTypeResponse.Success);

            var trevorId = _test.CreateUniqueId("trevor");
            var trevorResponse = await _test.Client.CreateUniqueAsset(trevorId, horseId);
            Assert.IsTrue(trevorResponse.Success);

            var response = await _test.Client.CreateAssetClaims(horseId, trevorId, "Test");
            Assert.IsTrue(response.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowInstanceCreateClaim()
        {
            var alice = _test.IdentityBuilder.Create();
            var horseId = _test.CreateUniqueId("horse");
            var permissions = new CreateUniqueAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateUniqueAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceCreateClaimForIdentity(alice.Id, false)
                        .BuildInstances())
                .Build();

            var createIdentityResponse = await _test.Client.CreateIdentity(alice);
            Assert.IsTrue(createIdentityResponse.Success);

            var aliceClient = await ClientBuilder.CreateWithExistingIdentity(TestEnvironment.DefaultClientSettings, alice);

            var newUniqueAssetTypeResponse = await _test.Client.CreateUniqueAssetType(horseId, permissions);
            Assert.IsTrue(newUniqueAssetTypeResponse.Success);

            var trevorId = _test.CreateUniqueId("trevor");
            var trevorResponse = await _test.Client.CreateUniqueAsset(trevorId, horseId);
            Assert.IsTrue(trevorResponse.Success);

            var response = await aliceClient.CreateAssetClaims(horseId, trevorId, "Test");
            Assert.IsFalse(response.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowInstanceInstanceOwnerCreateClaim()
        {
            var horseId = _test.CreateUniqueId("horse");
            var permissions = new CreateUniqueAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateUniqueAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceCreateClaimForInstanceOwner(false)
                        .BuildInstances())
                .Build();

            var newUniqueAssetTypeResponse = await _test.Client.CreateUniqueAssetType(horseId, permissions);
            Assert.IsTrue(newUniqueAssetTypeResponse.Success);

            var trevorId = _test.CreateUniqueId("trevor");
            var trevorResponse = await _test.Client.CreateUniqueAsset(trevorId, horseId);
            Assert.IsTrue(trevorResponse.Success);

            var response = await _test.Client.CreateAssetClaims(horseId, trevorId, "Test");
            Assert.IsFalse(response.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowInstanceTypeOwnerCreateClaim()
        {
            var horseId = _test.CreateUniqueId("horse");
            var permissions = new CreateUniqueAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateUniqueAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceCreateClaimForTypeOwner(false)
                        .BuildInstances())
                .Build();

            var newUniqueAssetTypeResponse = await _test.Client.CreateUniqueAssetType(horseId, permissions);
            Assert.IsTrue(newUniqueAssetTypeResponse.Success);

            var trevorId = _test.CreateUniqueId("trevor");
            var trevorResponse = await _test.Client.CreateUniqueAsset(trevorId, horseId);
            Assert.IsTrue(trevorResponse.Success);

            var response = await _test.Client.CreateAssetClaims(horseId, trevorId, "Test");
            Assert.IsFalse(response.Success);
        }

        [TestMethod]
        public async Task ShouldAllowInstanceReadClaim()
        {
            var alice = _test.IdentityBuilder.Create();
            var horseId = _test.CreateUniqueId("horse");
            var permissions = new CreateUniqueAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateUniqueAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceCreateClaimForEveryone(true)
                        .WithInstanceReadClaimForEveryone(true)
                        .BuildInstances())
                .Build();
            var newUniqueAssetTypeResponse = await _test.Client.CreateUniqueAssetType(horseId, permissions);
            Assert.IsTrue(newUniqueAssetTypeResponse.Success);

            var trevorId = _test.CreateUniqueId("trevor");
            var trevorResponse = await _test.Client.CreateUniqueAsset(trevorId, horseId);
            Assert.IsTrue(trevorResponse.Success);

            var response = await _test.Client.CreateAssetClaims(horseId, trevorId, "Test");
            Assert.IsTrue(response.Success);

            var createIdentityResponse = await _test.Client.CreateIdentity(alice);
            Assert.IsTrue(createIdentityResponse.Success);
            var aliceClient = await ClientBuilder.CreateWithExistingIdentity(TestEnvironment.DefaultClientSettings, alice);

            var result = await TestHelper.CallAndRetry(() => aliceClient.GetAssetClaim(horseId, trevorId, "Test"));
            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowInstanceReadClaim()
        {
            var alice = _test.IdentityBuilder.Create();
            var horseId = _test.CreateUniqueId("horse");
            var permissions = new CreateUniqueAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateUniqueAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceCreateClaimForEveryone(true)
                        .WithInstanceReadClaimForIdentity(alice.Id, false)
                        .BuildInstances())
                .Build();
            var newUniqueAssetTypeResponse = await _test.Client.CreateUniqueAssetType(horseId, permissions);
            Assert.IsTrue(newUniqueAssetTypeResponse.Success);

            var trevorId = _test.CreateUniqueId("trevor");
            var trevorResponse = await _test.Client.CreateUniqueAsset(trevorId, horseId);
            Assert.IsTrue(trevorResponse.Success);

            var response = await _test.Client.CreateAssetClaims(horseId, trevorId, "Test");
            Assert.IsTrue(response.Success);

            var createIdentityResponse = await _test.Client.CreateIdentity(alice);
            Assert.IsTrue(createIdentityResponse.Success);
            var aliceClient = await ClientBuilder.CreateWithExistingIdentity(TestEnvironment.DefaultClientSettings, alice);

            var result = await TestHelper.CallAndRetry(() => aliceClient.GetAssetClaim(horseId, trevorId, "Test"));
            Assert.IsFalse(result.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowInstanceTypeOwnerReadClaim()
        {
            var horseId = _test.CreateUniqueId("horse");
            var permissions = new CreateUniqueAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateUniqueAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceCreateClaimForEveryone(true)
                        .WithInstanceReadClaimForTypeOwner(false)
                        .BuildInstances())
                .Build();
            var newUniqueAssetTypeResponse = await _test.Client.CreateUniqueAssetType(horseId, permissions);
            Assert.IsTrue(newUniqueAssetTypeResponse.Success);

            var trevorId = _test.CreateUniqueId("trevor");
            var trevorResponse = await _test.Client.CreateUniqueAsset(trevorId, horseId);
            Assert.IsTrue(trevorResponse.Success);

            var response = await _test.Client.CreateAssetClaims(horseId, trevorId, "Test");
            Assert.IsTrue(response.Success);

            var result = await TestHelper.CallAndRetry(() => _test.Client.GetAssetClaim(horseId, trevorId, "Test"));
            Assert.IsFalse(result.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowInstanceInstanceOwnerReadClaim()
        {
            var horseId = _test.CreateUniqueId("horse");
            var permissions = new CreateUniqueAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateUniqueAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceCreateClaimForEveryone(true)
                        .WithInstanceReadClaimForInstanceOwner(false)
                        .BuildInstances())
                .Build();
            var newUniqueAssetTypeResponse = await _test.Client.CreateUniqueAssetType(horseId, permissions);
            Assert.IsTrue(newUniqueAssetTypeResponse.Success);

            var trevorId = _test.CreateUniqueId("trevor");
            var trevorResponse = await _test.Client.CreateUniqueAsset(trevorId, horseId);
            Assert.IsTrue(trevorResponse.Success);

            var response = await _test.Client.CreateAssetClaims(horseId, trevorId, "Test");
            Assert.IsTrue(response.Success);

            var result = await TestHelper.CallAndRetry(() => _test.Client.GetAssetClaim(horseId, trevorId, "Test"));
            Assert.IsFalse(result.Success);
        }

        [TestMethod]
        public async Task ShouldAllowInstanceCreateEndorsement()
        {
            var alice = _test.IdentityBuilder.Create();
            var horseId = _test.CreateUniqueId("horse");
            var permissions = new CreateUniqueAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateUniqueAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceCreateClaimForEveryone(true)
                        .WithInstanceEndorseClaimForEveryone(true)
                        .BuildInstances())
                .Build();
            var createIdentityResponse = await _test.Client.CreateIdentity(alice);
            Assert.IsTrue(createIdentityResponse.Success);
            var aliceClient = await ClientBuilder.CreateWithExistingIdentity(TestEnvironment.DefaultClientSettings, alice);

            var newUniqueAssetTypeResponse = await _test.Client.CreateUniqueAssetType(horseId, permissions);
            Assert.IsTrue(newUniqueAssetTypeResponse.Success);

            var trevorId = _test.CreateUniqueId("trevor");
            var trevorResponse = await _test.Client.CreateUniqueAsset(trevorId, horseId);
            Assert.IsTrue(trevorResponse.Success);

            var claimResponse = await _test.Client.CreateAssetClaims(horseId, trevorId, "Test");
            Assert.IsTrue(claimResponse.Success);

            var endorsements = aliceClient.CreateAssetEndorsements(horseId, trevorId)
                .AddEndorsement("Test");
            var body = endorsements.GenerateAssetEndorsementBody(horseId).Serialize();
            var header = aliceClient.GenerateAuthorisation(body);
            var endorse = await aliceClient.CreateAssetClaimsEndorsements(endorsements, endorsements.RequestId, body, header);
            Assert.IsTrue(endorse.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowInstanceCreateEndorsement()
        {
            var alice = _test.IdentityBuilder.Create();
            var horseId = _test.CreateUniqueId("horse");
            var permissions = new CreateUniqueAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateUniqueAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceCreateClaimForEveryone(true)
                        .WithInstanceEndorseClaimForIdentity(alice.Id, false)
                        .BuildInstances())
                .Build();
            var createIdentityResponse = await _test.Client.CreateIdentity(alice);
            Assert.IsTrue(createIdentityResponse.Success);
            var aliceClient = await ClientBuilder.CreateWithExistingIdentity(TestEnvironment.DefaultClientSettings, alice);

            var newUniqueAssetTypeResponse = await _test.Client.CreateUniqueAssetType(horseId, permissions);
            Assert.IsTrue(newUniqueAssetTypeResponse.Success);

            var trevorId = _test.CreateUniqueId("trevor");
            var trevorResponse = await _test.Client.CreateUniqueAsset(trevorId, horseId);
            Assert.IsTrue(trevorResponse.Success);

            var claimResponse = await _test.Client.CreateAssetClaims(horseId, trevorId, "Test");
            Assert.IsTrue(claimResponse.Success);

            var endorsements = aliceClient.CreateAssetEndorsements(horseId, trevorId)
                .AddEndorsement("Test");
            var body = endorsements.GenerateAssetEndorsementBody(horseId).Serialize();
            var header = aliceClient.GenerateAuthorisation(body);
            var endorse = await aliceClient.CreateAssetClaimsEndorsements(endorsements, endorsements.RequestId, body, header);
            Assert.IsFalse(endorse.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowInstanceTypeOwnerCreateEndorsement()
        {
            var horseId = _test.CreateUniqueId("horse");
            var permissions = new CreateUniqueAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateUniqueAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceCreateClaimForEveryone(true)
                        .WithInstanceEndorseClaimForTypeOwner(false)
                        .BuildInstances())
                .Build();

            var newUniqueAssetTypeResponse = await _test.Client.CreateUniqueAssetType(horseId, permissions);
            Assert.IsTrue(newUniqueAssetTypeResponse.Success);

            var trevorId = _test.CreateUniqueId("trevor");
            var trevorResponse = await _test.Client.CreateUniqueAsset(trevorId, horseId);
            Assert.IsTrue(trevorResponse.Success);

            var claimResponse = await _test.Client.CreateAssetClaims(horseId, trevorId, "Test");
            Assert.IsTrue(claimResponse.Success);

            var endorsements = _test.Client.CreateAssetEndorsements(horseId, trevorId)
                .AddEndorsement("Test");
            var body = endorsements.GenerateAssetEndorsementBody(horseId).Serialize();
            var header = _test.Client.GenerateAuthorisation(body);
            var endorse = await _test.Client.CreateAssetClaimsEndorsements(endorsements, endorsements.RequestId, body, header);
            Assert.IsFalse(endorse.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowInstanceInstanceOwnerCreateEndorsement()
        {
            var horseId = _test.CreateUniqueId("horse");
            var permissions = new CreateUniqueAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateUniqueAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceCreateClaimForEveryone(true)
                        .WithInstanceEndorseClaimForInstanceOwner(false)
                        .BuildInstances())
                .Build();

            var newUniqueAssetTypeResponse = await _test.Client.CreateUniqueAssetType(horseId, permissions);
            Assert.IsTrue(newUniqueAssetTypeResponse.Success);

            var trevorId = _test.CreateUniqueId("trevor");
            var trevorResponse = await _test.Client.CreateUniqueAsset(trevorId, horseId);
            Assert.IsTrue(trevorResponse.Success);

            var claimResponse = await _test.Client.CreateAssetClaims(horseId, trevorId, "Test");
            Assert.IsTrue(claimResponse.Success);

            var endorsements = _test.Client.CreateAssetEndorsements(horseId, trevorId)
                .AddEndorsement("Test");
            var body = endorsements.GenerateAssetEndorsementBody(horseId).Serialize();
            var header = _test.Client.GenerateAuthorisation(body);
            var endorse = await _test.Client.CreateAssetClaimsEndorsements(endorsements, endorsements.RequestId, body, header);
            Assert.IsFalse(endorse.Success);
        }

        [TestMethod]
        public async Task ShouldAllowInstanceReadEndorsement()
        {
            var alice = _test.IdentityBuilder.Create();
            var horseId = _test.CreateUniqueId("horse");
            var permissions = new CreateUniqueAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateUniqueAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceCreateClaimForEveryone(true)
                        .WithInstanceEndorseClaimForEveryone(true)
                        .WithInstanceReadEndorsementForEveryone(true)
                        .BuildInstances())
                .Build();
            var createIdentityResponse = await _test.Client.CreateIdentity(alice);
            Assert.IsTrue(createIdentityResponse.Success);
            var aliceClient = await ClientBuilder.CreateWithExistingIdentity(TestEnvironment.DefaultClientSettings, alice);

            var newUniqueAssetTypeResponse = await _test.Client.CreateUniqueAssetType(horseId, permissions);
            Assert.IsTrue(newUniqueAssetTypeResponse.Success);

            var trevorId = _test.CreateUniqueId("trevor");
            var trevorResponse = await _test.Client.CreateUniqueAsset(trevorId, horseId);
            Assert.IsTrue(trevorResponse.Success);

            var claimResponse = await _test.Client.CreateAssetClaims(horseId, trevorId, "Test");
            Assert.IsTrue(claimResponse.Success);

            var endorsements = _test.Client.CreateAssetEndorsements(horseId, trevorId)
                .AddEndorsement("Test");
            var body = endorsements.GenerateAssetEndorsementBody(horseId).Serialize();
            var header = _test.Client.GenerateAuthorisation(body);
            var endorse = await _test.Client.CreateAssetClaimsEndorsements(endorsements, endorsements.RequestId, body, header);
            Assert.IsTrue(endorse.Success);

            var result = await TestHelper.CallAndRetry(() => aliceClient.GetAssetEndorsement(horseId, trevorId, "Test", _test.Identity.Id));
            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowInstanceReadEndorsement()
        {
            var alice = _test.IdentityBuilder.Create();
            var horseId = _test.CreateUniqueId("horse");
            var permissions = new CreateUniqueAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateUniqueAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceCreateClaimForEveryone(true)
                        .WithInstanceEndorseClaimForEveryone(true)
                        .WithInstanceReadEndorsementForIdentity(alice.Id, false)
                        .BuildInstances())
                .Build();
            var createIdentityResponse = await _test.Client.CreateIdentity(alice);
            Assert.IsTrue(createIdentityResponse.Success);
            var aliceClient = await ClientBuilder.CreateWithExistingIdentity(TestEnvironment.DefaultClientSettings, alice);

            var newUniqueAssetTypeResponse = await _test.Client.CreateUniqueAssetType(horseId, permissions);
            Assert.IsTrue(newUniqueAssetTypeResponse.Success);

            var trevorId = _test.CreateUniqueId("trevor");
            var trevorResponse = await _test.Client.CreateUniqueAsset(trevorId, horseId);
            Assert.IsTrue(trevorResponse.Success);

            var claimResponse = await _test.Client.CreateAssetClaims(horseId, trevorId, "Test");
            Assert.IsTrue(claimResponse.Success);

            var endorsements = _test.Client.CreateAssetEndorsements(horseId, trevorId)
                .AddEndorsement("Test");
            var body = endorsements.GenerateAssetEndorsementBody(horseId).Serialize();
            var header = _test.Client.GenerateAuthorisation(body);
            var endorse = await _test.Client.CreateAssetClaimsEndorsements(endorsements, endorsements.RequestId, body, header);
            Assert.IsTrue(endorse.Success);

            var result = await TestHelper.CallAndRetry(() => aliceClient.GetAssetEndorsement(horseId, trevorId, "Test", _test.Identity.Id));
            Assert.IsFalse(result.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowInstanceTypeOwnerReadEndorsement()
        {
            var horseId = _test.CreateUniqueId("horse");
            var permissions = new CreateUniqueAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateUniqueAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceCreateClaimForEveryone(true)
                        .WithInstanceEndorseClaimForEveryone(true)
                        .WithInstanceReadEndorsementForTypeOwner(false)
                        .BuildInstances())
                .Build();
            var newUniqueAssetTypeResponse = await _test.Client.CreateUniqueAssetType(horseId, permissions);
            Assert.IsTrue(newUniqueAssetTypeResponse.Success);

            var trevorId = _test.CreateUniqueId("trevor");
            var trevorResponse = await _test.Client.CreateUniqueAsset(trevorId, horseId);
            Assert.IsTrue(trevorResponse.Success);

            var claimResponse = await _test.Client.CreateAssetClaims(horseId, trevorId, "Test");
            Assert.IsTrue(claimResponse.Success);

            var endorsements = _test.Client.CreateAssetEndorsements(horseId, trevorId)
                .AddEndorsement("Test");
            var body = endorsements.GenerateAssetEndorsementBody(horseId).Serialize();
            var header = _test.Client.GenerateAuthorisation(body);
            var endorse = await _test.Client.CreateAssetClaimsEndorsements(endorsements, endorsements.RequestId, body, header);
            Assert.IsTrue(endorse.Success);

            var result = await TestHelper.CallAndRetry(() => _test.Client.GetAssetEndorsement(horseId, trevorId, "Test", _test.Identity.Id));
            Assert.IsFalse(result.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowInstanceInstanceOwnerReadEndorsement()
        {
            var horseId = _test.CreateUniqueId("horse");
            var permissions = new CreateUniqueAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateUniqueAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceCreateClaimForEveryone(true)
                        .WithInstanceEndorseClaimForEveryone(true)
                        .WithInstanceReadEndorsementForInstanceOwner(false)
                        .BuildInstances())
                .Build();
            var newUniqueAssetTypeResponse = await _test.Client.CreateUniqueAssetType(horseId, permissions);
            Assert.IsTrue(newUniqueAssetTypeResponse.Success);

            var trevorId = _test.CreateUniqueId("trevor");
            var trevorResponse = await _test.Client.CreateUniqueAsset(trevorId, horseId);
            Assert.IsTrue(trevorResponse.Success);

            var claimResponse = await _test.Client.CreateAssetClaims(horseId, trevorId, "Test");
            Assert.IsTrue(claimResponse.Success);

            var endorsements = _test.Client.CreateAssetEndorsements(horseId, trevorId)
                .AddEndorsement("Test");
            var body = endorsements.GenerateAssetEndorsementBody(horseId).Serialize();
            var header = _test.Client.GenerateAuthorisation(body);
            var endorse = await _test.Client.CreateAssetClaimsEndorsements(endorsements, endorsements.RequestId, body, header);
            Assert.IsTrue(endorse.Success);

            var result = await TestHelper.CallAndRetry(() => _test.Client.GetAssetEndorsement(horseId, trevorId, "Test", _test.Identity.Id));
            Assert.IsFalse(result.Success);
        }

        [TestMethod]
        public async Task ShouldAllowInstanceTransfer()
        {
            var alice = _test.IdentityBuilder.Create();
            var bob = _test.IdentityBuilder.Create();
            var horseId = _test.CreateUniqueId("horse");
            var permissions = new CreateUniqueAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateUniqueAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceTransferForEveryone(true)
                        .BuildInstances())
                .Build();
            var createIdentityResponse = await _test.Client.CreateIdentity(alice);
            Assert.IsTrue(createIdentityResponse.Success);
            var aliceClient = await ClientBuilder.CreateWithExistingIdentity(TestEnvironment.DefaultClientSettings, alice);

            var bobResponse = await _test.Client.CreateIdentity(bob);
            Assert.IsTrue(bobResponse.Success);

            var newUniqueAssetTypeResponse = await _test.Client.CreateUniqueAssetType(horseId, permissions);
            Assert.IsTrue(newUniqueAssetTypeResponse.Success);

            var trevorId = _test.CreateUniqueId("trevor");
            var trevorResponse = await _test.Client.CreateUniqueAsset(trevorId, horseId);
            Assert.IsTrue(trevorResponse.Success);

            var request = new TransferBuilder(aliceClient)
                .AddOwnershipTransfer(trevorId, horseId, _test.Identity.Id, bob.Id)
                .Build();
            var response = await aliceClient.Write(request);
            Assert.IsTrue(response.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowInstanceTransfer()
        {
            var alice = _test.IdentityBuilder.Create();
            var bob = _test.IdentityBuilder.Create();
            var horseId = _test.CreateUniqueId("horse");
            var permissions = new CreateUniqueAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateUniqueAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceTransferForIdentity(alice.Id, false)
                        .BuildInstances())
                .Build();
            var createIdentityResponse = await _test.Client.CreateIdentity(alice);
            Assert.IsTrue(createIdentityResponse.Success);
            var aliceClient = await ClientBuilder.CreateWithExistingIdentity(TestEnvironment.DefaultClientSettings, alice);

            var bobResponse = await _test.Client.CreateIdentity(bob);
            Assert.IsTrue(bobResponse.Success);
            
            var newUniqueAssetTypeResponse = await _test.Client.CreateUniqueAssetType(horseId, permissions);
            Assert.IsTrue(newUniqueAssetTypeResponse.Success);

            var trevorId = _test.CreateUniqueId("trevor");
            var trevorResponse = await _test.Client.CreateUniqueAsset(trevorId, horseId);
            Assert.IsTrue(trevorResponse.Success);

            var request = new TransferBuilder(aliceClient)
                .AddOwnershipTransfer(trevorId, horseId, _test.Identity.Id, bob.Id)
                .Build();
            var response = await aliceClient.Write(request);
            Assert.IsFalse(response.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowInstanceTypeOwnerTransfer()
        {
            var alice = _test.IdentityBuilder.Create();
            var horseId = _test.CreateUniqueId("horse");
            var permissions = new CreateUniqueAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateUniqueAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceTransferForTypeOwner(false)
                        .BuildInstances())
                .Build();
            var createIdentityResponse = await _test.Client.CreateIdentity(alice);
            Assert.IsTrue(createIdentityResponse.Success);

            var newUniqueAssetTypeResponse = await _test.Client.CreateUniqueAssetType(horseId, permissions);
            Assert.IsTrue(newUniqueAssetTypeResponse.Success);

            var trevorId = _test.CreateUniqueId("trevor");
            var trevorResponse = await _test.Client.CreateUniqueAsset(trevorId, horseId);
            Assert.IsTrue(trevorResponse.Success);

            var request = new TransferBuilder(_test.Client)
                .AddOwnershipTransfer(trevorId, horseId, _test.Identity.Id, alice.Id)
                .Build();
            var response = await _test.Client.Write(request);
            Assert.IsFalse(response.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowInstanceInstanceOwnerTransfer()
        {
            var alice = _test.IdentityBuilder.Create();
            var horseId = _test.CreateUniqueId("horse");
            var permissions = new CreateUniqueAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateUniqueAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceTransferForInstanceOwner(false)
                        .BuildInstances())
                .Build();
            var createIdentityResponse = await _test.Client.CreateIdentity(alice);
            Assert.IsTrue(createIdentityResponse.Success);

            var newUniqueAssetTypeResponse = await _test.Client.CreateUniqueAssetType(horseId, permissions);
            Assert.IsTrue(newUniqueAssetTypeResponse.Success);

            var trevorId = _test.CreateUniqueId("trevor");
            var trevorResponse = await _test.Client.CreateUniqueAsset(trevorId, horseId);
            Assert.IsTrue(trevorResponse.Success);

            var request = new TransferBuilder(_test.Client)
                .AddOwnershipTransfer(trevorId, horseId, _test.Identity.Id, alice.Id)
                .Build();
            var response = await _test.Client.Write(request);
            Assert.IsFalse(response.Success);
        }
    }
}