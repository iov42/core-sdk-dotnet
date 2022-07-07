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
    public class TestIdentityQuantifiableAssetPermissionCalls
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
            var gbpId = _test.CreateUniqueId("gbp");
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .Build();
            var newQuantifiableAssetTypeResponse = await _test.Client.CreateQuantifiableAssetType(gbpId, 1, permissions);
            Assert.IsTrue(newQuantifiableAssetTypeResponse.Success);

            var response = await _test.Client.CreateAssetTypeClaims(gbpId, "Test");
            Assert.IsTrue(response.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowCreateClaimByAnyoneOnBlank()
        {
            var gbpId = _test.CreateUniqueId("gbp");
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .Build();
            var newQuantifiableAssetTypeResponse = await _test.Client.CreateQuantifiableAssetType(gbpId, 1, permissions);
            Assert.IsTrue(newQuantifiableAssetTypeResponse.Success);

            var alice = _test.IdentityBuilder.Create();
            var createIdentityResponse = await _test.Client.CreateIdentity(alice);
            Assert.IsTrue(createIdentityResponse.Success);
            var aliceClient = await ClientBuilder.CreateWithExistingIdentity(TestEnvironment.DefaultClientSettings, alice);

            var response = await aliceClient.CreateAssetTypeClaims(gbpId, "Test");
            Assert.IsFalse(response.Success);
        }

        [TestMethod]
        public async Task ShouldAllowCreateClaim()
        {
            var gbpId = _test.CreateUniqueId("gbp");
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .WithCreateClaimForEveryone(true)
                .Build();
            var newQuantifiableAssetTypeResponse = await _test.Client.CreateQuantifiableAssetType(gbpId, 1, permissions);
            Assert.IsTrue(newQuantifiableAssetTypeResponse.Success);

            var alice = _test.IdentityBuilder.Create();
            var createIdentityResponse = await _test.Client.CreateIdentity(alice);
            Assert.IsTrue(createIdentityResponse.Success);
            var aliceClient = await ClientBuilder.CreateWithExistingIdentity(TestEnvironment.DefaultClientSettings, alice);

            var response = await aliceClient.CreateAssetTypeClaims(gbpId, "Test");
            Assert.IsTrue(response.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowCreateClaim()
        {
            var alice = _test.IdentityBuilder.Create();
            var gbpId = _test.CreateUniqueId("gbp");
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .WithCreateClaimForIdentity(alice.Id, false)
                .Build();
            var newQuantifiableAssetTypeResponse = await _test.Client.CreateQuantifiableAssetType(gbpId, 1, permissions);
            Assert.IsTrue(newQuantifiableAssetTypeResponse.Success);

            var createIdentityResponse = await _test.Client.CreateIdentity(alice);
            Assert.IsTrue(createIdentityResponse.Success);
            var aliceClient = await ClientBuilder.CreateWithExistingIdentity(TestEnvironment.DefaultClientSettings, alice);

            var response = await aliceClient.CreateAssetTypeClaims(gbpId, "Test");
            Assert.IsFalse(response.Success);
        }

        [TestMethod]
        public async Task ShouldAllowInstanceOwnerCreateClaim()
        {
            var gbpId = _test.CreateUniqueId("gbp");
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .WithCreateClaimForInstanceOwner(true)
                .Build();
            var newQuantifiableAssetTypeResponse = await _test.Client.CreateQuantifiableAssetType(gbpId, 1, permissions);
            Assert.IsTrue(newQuantifiableAssetTypeResponse.Success);
            var response = await _test.Client.CreateAssetTypeClaims(gbpId, "Test");
            Assert.IsTrue(response.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowInstanceOwnerCreateClaim()
        {
            var gbpId = _test.CreateUniqueId("gbp");
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .WithCreateClaimForInstanceOwner(false)
                .Build();
            var newQuantifiableAssetTypeResponse = await _test.Client.CreateQuantifiableAssetType(gbpId, 1, permissions);
            Assert.IsTrue(newQuantifiableAssetTypeResponse.Success);
            var response = await _test.Client.CreateAssetTypeClaims(gbpId, "Test");
            Assert.IsFalse(response.Success);
        }

        [TestMethod]
        public async Task ShouldAllowReadClaim()
        {
            var gbpId = _test.CreateUniqueId("gbp");
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .WithCreateClaimForEveryone(true)
                .WithReadClaimForEveryone(true)
                .Build();
            var newQuantifiableAssetTypeResponse = await _test.Client.CreateQuantifiableAssetType(gbpId, 1, permissions);
            Assert.IsTrue(newQuantifiableAssetTypeResponse.Success);

            var alice = _test.IdentityBuilder.Create();
            var createIdentityResponse = await _test.Client.CreateIdentity(alice);
            Assert.IsTrue(createIdentityResponse.Success);
            var aliceClient = await ClientBuilder.CreateWithExistingIdentity(TestEnvironment.DefaultClientSettings, alice);

            var response = await _test.Client.CreateAssetTypeClaims(gbpId, "Test");
            Assert.IsTrue(response.Success);

            var result = await TestHelper.CallAndRetry(() => aliceClient.GetAssetTypeClaim(gbpId, "Test"));
            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowReadClaim()
        {
            var alice = _test.IdentityBuilder.Create();
            var gbpId = _test.CreateUniqueId("gbp");
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .WithCreateClaimForEveryone(true)
                .WithReadClaimForIdentity(alice.Id, false)
                .Build();
            var newQuantifiableAssetTypeResponse = await _test.Client.CreateQuantifiableAssetType(gbpId, 1, permissions);
            Assert.IsTrue(newQuantifiableAssetTypeResponse.Success);

            var createIdentityResponse = await _test.Client.CreateIdentity(alice);
            Assert.IsTrue(createIdentityResponse.Success);
            var aliceClient = await ClientBuilder.CreateWithExistingIdentity(TestEnvironment.DefaultClientSettings, alice);

            var response = await _test.Client.CreateAssetTypeClaims(gbpId, "Test");
            Assert.IsTrue(response.Success);

            var result = await TestHelper.CallAndRetry(() => aliceClient.GetAssetTypeClaim(gbpId, "Test"));
            Assert.IsFalse(result.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowReadClaimForInstanceOwner()
        {
            var gbpId = _test.CreateUniqueId("gbp");
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .WithCreateClaimForEveryone(true)
                .WithReadClaimForInstanceOwner(false)
                .Build();
            var newQuantifiableAssetTypeResponse = await _test.Client.CreateQuantifiableAssetType(gbpId, 1, permissions);
            Assert.IsTrue(newQuantifiableAssetTypeResponse.Success);

            var response = await _test.Client.CreateAssetTypeClaims(gbpId, "Test");
            Assert.IsTrue(response.Success);

            var result = await TestHelper.CallAndRetry(() => _test.Client.GetAssetTypeClaim(gbpId, "Test"));
            Assert.IsFalse(result.Success);
        }

        [TestMethod]
        public async Task ShouldAllowEndorseClaim()
        {
            var gbpId = _test.CreateUniqueId("gbp");
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .WithCreateClaimForEveryone(true)
                .WithEndorseClaimForEveryone(true)
                .Build();
            var newQuantifiableAssetTypeResponse = await _test.Client.CreateQuantifiableAssetType(gbpId, 1, permissions);
            Assert.IsTrue(newQuantifiableAssetTypeResponse.Success);

            var alice = _test.IdentityBuilder.Create();
            var createIdentityResponse = await _test.Client.CreateIdentity(alice);
            Assert.IsTrue(createIdentityResponse.Success);
            var aliceClient = await ClientBuilder.CreateWithExistingIdentity(TestEnvironment.DefaultClientSettings, alice);

            var response = await _test.Client.CreateAssetTypeClaims(gbpId, "Test");
            Assert.IsTrue(response.Success);

            var endorsements = aliceClient.CreateAssetTypeEndorsements(gbpId)
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
            var gbpId = _test.CreateUniqueId("gbp");
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .WithCreateClaimForEveryone(true)
                .WithEndorseClaimForIdentity(alice.Id, false)
                .Build();
            var newQuantifiableAssetTypeResponse = await _test.Client.CreateQuantifiableAssetType(gbpId, 1, permissions);
            Assert.IsTrue(newQuantifiableAssetTypeResponse.Success);

            var createIdentityResponse = await _test.Client.CreateIdentity(alice);
            Assert.IsTrue(createIdentityResponse.Success);
            var aliceClient = await ClientBuilder.CreateWithExistingIdentity(TestEnvironment.DefaultClientSettings, alice);

            var response = await _test.Client.CreateAssetTypeClaims(gbpId, "Test");
            Assert.IsTrue(response.Success);

            var endorsements = aliceClient.CreateAssetTypeEndorsements(gbpId)
                .AddEndorsement("Test");
            var body = endorsements.GenerateAssetTypeEndorsementBody().Serialize();
            var testHeader = aliceClient.GenerateAuthorisation(body);
            var endorse = await aliceClient.CreateAssetTypeClaimsEndorsements(endorsements, endorsements.RequestId, body, testHeader);
            Assert.IsFalse(endorse.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowEndorseClaimForInstanceOwner()
        {
            var gbpId = _test.CreateUniqueId("gbp");
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .WithCreateClaimForEveryone(true)
                .WithEndorseClaimForInstanceOwner(false)
                .Build();
            var newQuantifiableAssetTypeResponse = await _test.Client.CreateQuantifiableAssetType(gbpId, 1, permissions);
            Assert.IsTrue(newQuantifiableAssetTypeResponse.Success);

            var response = await _test.Client.CreateAssetTypeClaims(gbpId, "Test");
            Assert.IsTrue(response.Success);

            var endorsements = _test.Client.CreateAssetTypeEndorsements(gbpId)
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
            var gbpId = _test.CreateUniqueId("gbp");
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateQuantifiableAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceCreateClaimForEveryone(true)
                        .BuildInstances())
                .Build();
            var newQuantifiableAssetTypeResponse = await _test.Client.CreateQuantifiableAssetType(gbpId, 1, permissions);
            Assert.IsTrue(newQuantifiableAssetTypeResponse.Success);

            var gbpAccountId = _test.CreateUniqueId("gbpAccount");
            var gbpAccountResponse = await _test.Client.CreateQuantifiableAccount(gbpAccountId, gbpId);
            Assert.IsTrue(gbpAccountResponse.Success);

            var response = await _test.Client.CreateAssetClaims(gbpId, gbpAccountId, "Test");
            Assert.IsTrue(response.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowInstanceCreateClaim()
        {
            var alice = _test.IdentityBuilder.Create();
            var gbpId = _test.CreateUniqueId("gbp");
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateQuantifiableAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceCreateClaimForIdentity(alice.Id, false)
                        .BuildInstances())
                .Build();

            var createIdentityResponse = await _test.Client.CreateIdentity(alice);
            Assert.IsTrue(createIdentityResponse.Success);

            var aliceClient = await ClientBuilder.CreateWithExistingIdentity(TestEnvironment.DefaultClientSettings, alice);

            var newQuantifiableAssetTypeResponse = await _test.Client.CreateQuantifiableAssetType(gbpId, 1, permissions);
            Assert.IsTrue(newQuantifiableAssetTypeResponse.Success);

            var gbpAccountId = _test.CreateUniqueId("gbpAccount");
            var gbpAccountResponse = await _test.Client.CreateQuantifiableAccount(gbpAccountId, gbpId);
            Assert.IsTrue(gbpAccountResponse.Success);

            var response = await aliceClient.CreateAssetClaims(gbpId, gbpAccountId, "Test");
            Assert.IsFalse(response.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowInstanceInstanceOwnerCreateClaim()
        {
            var gbpId = _test.CreateUniqueId("gbp");
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateQuantifiableAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceCreateClaimForInstanceOwner(false)
                        .BuildInstances())
                .Build();

            var newQuantifiableAssetTypeResponse = await _test.Client.CreateQuantifiableAssetType(gbpId, 1, permissions);
            Assert.IsTrue(newQuantifiableAssetTypeResponse.Success);

            var gbpAccountId = _test.CreateUniqueId("gbpAccount");
            var gbpAccountResponse = await _test.Client.CreateQuantifiableAccount(gbpAccountId, gbpId);
            Assert.IsTrue(gbpAccountResponse.Success);

            var response = await _test.Client.CreateAssetClaims(gbpId, gbpAccountId, "Test");
            Assert.IsFalse(response.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowInstanceTypeOwnerCreateClaim()
        {
            var gbpId = _test.CreateUniqueId("gbp");
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateQuantifiableAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceCreateClaimForTypeOwner(false)
                        .BuildInstances())
                .Build();

            var newQuantifiableAssetTypeResponse = await _test.Client.CreateQuantifiableAssetType(gbpId, 1, permissions);
            Assert.IsTrue(newQuantifiableAssetTypeResponse.Success);

            var gbpAccountId = _test.CreateUniqueId("gbpAccount");
            var gbpAccountResponse = await _test.Client.CreateQuantifiableAccount(gbpAccountId, gbpId);
            Assert.IsTrue(gbpAccountResponse.Success);

            var response = await _test.Client.CreateAssetClaims(gbpId, gbpAccountId, "Test");
            Assert.IsFalse(response.Success);
        }

        [TestMethod]
        public async Task ShouldAllowInstanceReadClaim()
        {
            var alice = _test.IdentityBuilder.Create();
            var gbpId = _test.CreateUniqueId("gbp");
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateQuantifiableAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceCreateClaimForEveryone(true)
                        .WithInstanceReadClaimForEveryone(true)
                        .BuildInstances())
                .Build();
            var newQuantifiableAssetTypeResponse = await _test.Client.CreateQuantifiableAssetType(gbpId, 1, permissions);
            Assert.IsTrue(newQuantifiableAssetTypeResponse.Success);

            var gbpAccountId = _test.CreateUniqueId("gbpAccount");
            var gbpAccountResponse = await _test.Client.CreateQuantifiableAccount(gbpAccountId, gbpId);
            Assert.IsTrue(gbpAccountResponse.Success);

            var response = await _test.Client.CreateAssetClaims(gbpId, gbpAccountId, "Test");
            Assert.IsTrue(response.Success);

            var createIdentityResponse = await _test.Client.CreateIdentity(alice);
            Assert.IsTrue(createIdentityResponse.Success);
            var aliceClient = await ClientBuilder.CreateWithExistingIdentity(TestEnvironment.DefaultClientSettings, alice);

            var result = await TestHelper.CallAndRetry(() => aliceClient.GetAssetClaim(gbpId, gbpAccountId, "Test"));
            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowInstanceReadClaim()
        {
            var alice = _test.IdentityBuilder.Create();
            var gbpId = _test.CreateUniqueId("gbp");
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateQuantifiableAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceCreateClaimForEveryone(true)
                        .WithInstanceReadClaimForIdentity(alice.Id, false)
                        .BuildInstances())
                .Build();
            var newQuantifiableAssetTypeResponse = await _test.Client.CreateQuantifiableAssetType(gbpId, 1, permissions);
            Assert.IsTrue(newQuantifiableAssetTypeResponse.Success);

            var gbpAccountId = _test.CreateUniqueId("gbpAccount");
            var gbpAccountResponse = await _test.Client.CreateQuantifiableAccount(gbpAccountId, gbpId);
            Assert.IsTrue(gbpAccountResponse.Success);

            var response = await _test.Client.CreateAssetClaims(gbpId, gbpAccountId, "Test");
            Assert.IsTrue(response.Success);

            var createIdentityResponse = await _test.Client.CreateIdentity(alice);
            Assert.IsTrue(createIdentityResponse.Success);
            var aliceClient = await ClientBuilder.CreateWithExistingIdentity(TestEnvironment.DefaultClientSettings, alice);

            var result = await TestHelper.CallAndRetry(() => aliceClient.GetAssetClaim(gbpId, gbpAccountId, "Test"));
            Assert.IsFalse(result.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowInstanceTypeOwnerReadClaim()
        {
            var gbpId = _test.CreateUniqueId("gbp");
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateQuantifiableAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceCreateClaimForEveryone(true)
                        .WithInstanceReadClaimForTypeOwner(false)
                        .BuildInstances())
                .Build();
            var newQuantifiableAssetTypeResponse = await _test.Client.CreateQuantifiableAssetType(gbpId, 1, permissions);
            Assert.IsTrue(newQuantifiableAssetTypeResponse.Success);

            var gbpAccountId = _test.CreateUniqueId("gbpAccount");
            var gbpAccountResponse = await _test.Client.CreateQuantifiableAccount(gbpAccountId, gbpId);
            Assert.IsTrue(gbpAccountResponse.Success);

            var response = await _test.Client.CreateAssetClaims(gbpId, gbpAccountId, "Test");
            Assert.IsTrue(response.Success);

            var result = await TestHelper.CallAndRetry(() => _test.Client.GetAssetClaim(gbpId, gbpAccountId, "Test"));
            Assert.IsFalse(result.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowInstanceInstanceOwnerReadClaim()
        {
            var gbpId = _test.CreateUniqueId("gbp");
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateQuantifiableAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceCreateClaimForEveryone(true)
                        .WithInstanceReadClaimForInstanceOwner(false)
                        .BuildInstances())
                .Build();
            var newQuantifiableAssetTypeResponse = await _test.Client.CreateQuantifiableAssetType(gbpId, 1, permissions);
            Assert.IsTrue(newQuantifiableAssetTypeResponse.Success);

            var gbpAccountId = _test.CreateUniqueId("gbpAccount");
            var gbpAccountResponse = await _test.Client.CreateQuantifiableAccount(gbpAccountId, gbpId);
            Assert.IsTrue(gbpAccountResponse.Success);

            var response = await _test.Client.CreateAssetClaims(gbpId, gbpAccountId, "Test");
            Assert.IsTrue(response.Success);

            var result = await TestHelper.CallAndRetry(() => _test.Client.GetAssetClaim(gbpId, gbpAccountId, "Test"));
            Assert.IsFalse(result.Success);
        }

        [TestMethod]
        public async Task ShouldAllowInstanceCreateEndorsement()
        {
            var alice = _test.IdentityBuilder.Create();
            var gbpId = _test.CreateUniqueId("gbp");
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateQuantifiableAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceCreateClaimForEveryone(true)
                        .WithInstanceEndorseClaimForEveryone(true)
                        .BuildInstances())
                .Build();
            var createIdentityResponse = await _test.Client.CreateIdentity(alice);
            Assert.IsTrue(createIdentityResponse.Success);
            var aliceClient = await ClientBuilder.CreateWithExistingIdentity(TestEnvironment.DefaultClientSettings, alice);

            var newQuantifiableAssetTypeResponse = await _test.Client.CreateQuantifiableAssetType(gbpId, 1, permissions);
            Assert.IsTrue(newQuantifiableAssetTypeResponse.Success);

            var gbpAccountId = _test.CreateUniqueId("gbpAccount");
            var gbpAccountResponse = await _test.Client.CreateQuantifiableAccount(gbpAccountId, gbpId);
            Assert.IsTrue(gbpAccountResponse.Success);

            var claimResponse = await _test.Client.CreateAssetClaims(gbpId, gbpAccountId, "Test");
            Assert.IsTrue(claimResponse.Success);

            var endorsements = aliceClient.CreateAssetEndorsements(gbpId, gbpAccountId)
                .AddEndorsement("Test");
            var body = endorsements.GenerateAssetEndorsementBody(gbpId).Serialize();
            var header = aliceClient.GenerateAuthorisation(body);
            var endorse = await aliceClient.CreateAssetClaimsEndorsements(endorsements, endorsements.RequestId, body, header);
            Assert.IsTrue(endorse.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowInstanceCreateEndorsement()
        {
            var alice = _test.IdentityBuilder.Create();
            var gbpId = _test.CreateUniqueId("gbp");
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateQuantifiableAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceCreateClaimForEveryone(true)
                        .WithInstanceEndorseClaimForIdentity(alice.Id, false)
                        .BuildInstances())
                .Build();
            var createIdentityResponse = await _test.Client.CreateIdentity(alice);
            Assert.IsTrue(createIdentityResponse.Success);
            var aliceClient = await ClientBuilder.CreateWithExistingIdentity(TestEnvironment.DefaultClientSettings, alice);

            var newQuantifiableAssetTypeResponse = await _test.Client.CreateQuantifiableAssetType(gbpId, 1, permissions);
            Assert.IsTrue(newQuantifiableAssetTypeResponse.Success);

            var gbpAccountId = _test.CreateUniqueId("gbpAccount");
            var gbpAccountResponse = await _test.Client.CreateQuantifiableAccount(gbpAccountId, gbpId);
            Assert.IsTrue(gbpAccountResponse.Success);

            var claimResponse = await _test.Client.CreateAssetClaims(gbpId, gbpAccountId, "Test");
            Assert.IsTrue(claimResponse.Success);

            var endorsements = aliceClient.CreateAssetEndorsements(gbpId, gbpAccountId)
                .AddEndorsement("Test");
            var body = endorsements.GenerateAssetEndorsementBody(gbpId).Serialize();
            var header = aliceClient.GenerateAuthorisation(body);
            var endorse = await aliceClient.CreateAssetClaimsEndorsements(endorsements, endorsements.RequestId, body, header);
            Assert.IsFalse(endorse.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowInstanceTypeOwnerCreateEndorsement()
        {
            var gbpId = _test.CreateUniqueId("gbp");
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateQuantifiableAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceCreateClaimForEveryone(true)
                        .WithInstanceEndorseClaimForTypeOwner(false)
                        .BuildInstances())
                .Build();

            var newQuantifiableAssetTypeResponse = await _test.Client.CreateQuantifiableAssetType(gbpId, 1, permissions);
            Assert.IsTrue(newQuantifiableAssetTypeResponse.Success);

            var gbpAccountId = _test.CreateUniqueId("gbpAccount");
            var gbpAccountResponse = await _test.Client.CreateQuantifiableAccount(gbpAccountId, gbpId);
            Assert.IsTrue(gbpAccountResponse.Success);

            var claimResponse = await _test.Client.CreateAssetClaims(gbpId, gbpAccountId, "Test");
            Assert.IsTrue(claimResponse.Success);

            var endorsements = _test.Client.CreateAssetEndorsements(gbpId, gbpAccountId)
                .AddEndorsement("Test");
            var body = endorsements.GenerateAssetEndorsementBody(gbpId).Serialize();
            var header = _test.Client.GenerateAuthorisation(body);
            var endorse = await _test.Client.CreateAssetClaimsEndorsements(endorsements, endorsements.RequestId, body, header);
            Assert.IsFalse(endorse.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowInstanceInstanceOwnerCreateEndorsement()
        {
            var gbpId = _test.CreateUniqueId("gbp");
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateQuantifiableAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceCreateClaimForEveryone(true)
                        .WithInstanceEndorseClaimForInstanceOwner(false)
                        .BuildInstances())
                .Build();

            var newQuantifiableAssetTypeResponse = await _test.Client.CreateQuantifiableAssetType(gbpId, 1, permissions);
            Assert.IsTrue(newQuantifiableAssetTypeResponse.Success);

            var gbpAccountId = _test.CreateUniqueId("gbpAccount");
            var gbpAccountResponse = await _test.Client.CreateQuantifiableAccount(gbpAccountId, gbpId);
            Assert.IsTrue(gbpAccountResponse.Success);

            var claimResponse = await _test.Client.CreateAssetClaims(gbpId, gbpAccountId, "Test");
            Assert.IsTrue(claimResponse.Success);

            var endorsements = _test.Client.CreateAssetEndorsements(gbpId, gbpAccountId)
                .AddEndorsement("Test");
            var body = endorsements.GenerateAssetEndorsementBody(gbpId).Serialize();
            var header = _test.Client.GenerateAuthorisation(body);
            var endorse = await _test.Client.CreateAssetClaimsEndorsements(endorsements, endorsements.RequestId, body, header);
            Assert.IsFalse(endorse.Success);
        }

        [TestMethod]
        public async Task ShouldAllowInstanceReadEndorsement()
        {
            var alice = _test.IdentityBuilder.Create();
            var gbpId = _test.CreateUniqueId("gbp");
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateQuantifiableAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceCreateClaimForEveryone(true)
                        .WithInstanceEndorseClaimForEveryone(true)
                        .WithInstanceReadEndorsementForEveryone(true)
                        .BuildInstances())
                .Build();
            var createIdentityResponse = await _test.Client.CreateIdentity(alice);
            Assert.IsTrue(createIdentityResponse.Success);
            var aliceClient = await ClientBuilder.CreateWithExistingIdentity(TestEnvironment.DefaultClientSettings, alice);

            var newQuantifiableAssetTypeResponse = await _test.Client.CreateQuantifiableAssetType(gbpId, 1, permissions);
            Assert.IsTrue(newQuantifiableAssetTypeResponse.Success);

            var gbpAccountId = _test.CreateUniqueId("gbpAccount");
            var gbpAccountResponse = await _test.Client.CreateQuantifiableAccount(gbpAccountId, gbpId);
            Assert.IsTrue(gbpAccountResponse.Success);

            var claimResponse = await _test.Client.CreateAssetClaims(gbpId, gbpAccountId, "Test");
            Assert.IsTrue(claimResponse.Success);

            var endorsements = _test.Client.CreateAssetEndorsements(gbpId, gbpAccountId)
                .AddEndorsement("Test");
            var body = endorsements.GenerateAssetEndorsementBody(gbpId).Serialize();
            var header = _test.Client.GenerateAuthorisation(body);
            var endorse = await _test.Client.CreateAssetClaimsEndorsements(endorsements, endorsements.RequestId, body, header);
            Assert.IsTrue(endorse.Success);

            var result = await TestHelper.CallAndRetry(() => aliceClient.GetAssetEndorsement(gbpId, gbpAccountId, "Test", _test.Identity.Id));
            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowInstanceReadEndorsement()
        {
            var alice = _test.IdentityBuilder.Create();
            var gbpId = _test.CreateUniqueId("gbp");
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateQuantifiableAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceCreateClaimForEveryone(true)
                        .WithInstanceEndorseClaimForEveryone(true)
                        .WithInstanceReadEndorsementForIdentity(alice.Id, false)
                        .BuildInstances())
                .Build();
            var createIdentityResponse = await _test.Client.CreateIdentity(alice);
            Assert.IsTrue(createIdentityResponse.Success);
            var aliceClient = await ClientBuilder.CreateWithExistingIdentity(TestEnvironment.DefaultClientSettings, alice);

            var newQuantifiableAssetTypeResponse = await _test.Client.CreateQuantifiableAssetType(gbpId, 1, permissions);
            Assert.IsTrue(newQuantifiableAssetTypeResponse.Success);

            var gbpAccountId = _test.CreateUniqueId("gbpAccount");
            var gbpAccountResponse = await _test.Client.CreateQuantifiableAccount(gbpAccountId, gbpId);
            Assert.IsTrue(gbpAccountResponse.Success);

            var claimResponse = await _test.Client.CreateAssetClaims(gbpId, gbpAccountId, "Test");
            Assert.IsTrue(claimResponse.Success);

            var endorsements = _test.Client.CreateAssetEndorsements(gbpId, gbpAccountId)
                .AddEndorsement("Test");
            var body = endorsements.GenerateAssetEndorsementBody(gbpId).Serialize();
            var header = _test.Client.GenerateAuthorisation(body);
            var endorse = await _test.Client.CreateAssetClaimsEndorsements(endorsements, endorsements.RequestId, body, header);
            Assert.IsTrue(endorse.Success);

            var result = await TestHelper.CallAndRetry(() => aliceClient.GetAssetEndorsement(gbpId, gbpAccountId, "Test", _test.Identity.Id));
            Assert.IsFalse(result.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowInstanceTypeOwnerReadEndorsement()
        {
            var gbpId = _test.CreateUniqueId("gbp");
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateQuantifiableAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceCreateClaimForEveryone(true)
                        .WithInstanceEndorseClaimForEveryone(true)
                        .WithInstanceReadEndorsementForTypeOwner(false)
                        .BuildInstances())
                .Build();
            var newQuantifiableAssetTypeResponse = await _test.Client.CreateQuantifiableAssetType(gbpId, 1, permissions);
            Assert.IsTrue(newQuantifiableAssetTypeResponse.Success);

            var gbpAccountId = _test.CreateUniqueId("gbpAccount");
            var gbpAccountResponse = await _test.Client.CreateQuantifiableAccount(gbpAccountId, gbpId);
            Assert.IsTrue(gbpAccountResponse.Success);

            var claimResponse = await _test.Client.CreateAssetClaims(gbpId, gbpAccountId, "Test");
            Assert.IsTrue(claimResponse.Success);

            var endorsements = _test.Client.CreateAssetEndorsements(gbpId, gbpAccountId)
                .AddEndorsement("Test");
            var body = endorsements.GenerateAssetEndorsementBody(gbpId).Serialize();
            var header = _test.Client.GenerateAuthorisation(body);
            var endorse = await _test.Client.CreateAssetClaimsEndorsements(endorsements, endorsements.RequestId, body, header);
            Assert.IsTrue(endorse.Success);

            var result = await TestHelper.CallAndRetry(() => _test.Client.GetAssetEndorsement(gbpId, gbpAccountId, "Test", _test.Identity.Id));
            Assert.IsFalse(result.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowInstanceInstanceOwnerReadEndorsement()
        {
            var gbpId = _test.CreateUniqueId("gbp");
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateQuantifiableAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceCreateClaimForEveryone(true)
                        .WithInstanceEndorseClaimForEveryone(true)
                        .WithInstanceReadEndorsementForInstanceOwner(false)
                        .BuildInstances())
                .Build();
            var newQuantifiableAssetTypeResponse = await _test.Client.CreateQuantifiableAssetType(gbpId, 1, permissions);
            Assert.IsTrue(newQuantifiableAssetTypeResponse.Success);

            var gbpAccountId = _test.CreateUniqueId("gbpAccount");
            var gbpAccountResponse = await _test.Client.CreateQuantifiableAccount(gbpAccountId, gbpId);
            Assert.IsTrue(gbpAccountResponse.Success);

            var claimResponse = await _test.Client.CreateAssetClaims(gbpId, gbpAccountId, "Test");
            Assert.IsTrue(claimResponse.Success);

            var endorsements = _test.Client.CreateAssetEndorsements(gbpId, gbpAccountId)
                .AddEndorsement("Test");
            var body = endorsements.GenerateAssetEndorsementBody(gbpId).Serialize();
            var header = _test.Client.GenerateAuthorisation(body);
            var endorse = await _test.Client.CreateAssetClaimsEndorsements(endorsements, endorsements.RequestId, body, header);
            Assert.IsTrue(endorse.Success);

            var result = await TestHelper.CallAndRetry(() => _test.Client.GetAssetEndorsement(gbpId, gbpAccountId, "Test", _test.Identity.Id));
            Assert.IsFalse(result.Success);
        }

        [TestMethod]
        public async Task ShouldAllowInstanceTransfer()
        {
            var alice = _test.IdentityBuilder.Create();
            var bob = _test.IdentityBuilder.Create();
            var gbpId = _test.CreateUniqueId("gbp");
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateQuantifiableAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceTransferForEveryone(true)
                        .BuildInstances())
                .Build();
            var createIdentityResponse = await _test.Client.CreateIdentity(alice);
            Assert.IsTrue(createIdentityResponse.Success);
            var aliceClient = await ClientBuilder.CreateWithExistingIdentity(TestEnvironment.DefaultClientSettings, alice);

            var bobResponse = await _test.Client.CreateIdentity(bob);
            Assert.IsTrue(bobResponse.Success);

            var newQuantifiableAssetTypeResponse = await _test.Client.CreateQuantifiableAssetType(gbpId, 1, permissions);
            Assert.IsTrue(newQuantifiableAssetTypeResponse.Success);

            var gbpAccountId = _test.CreateUniqueId("gbpAccount");
            var gbpAccountResponse = await _test.Client.CreateQuantifiableAccount(gbpAccountId, gbpId);
            Assert.IsTrue(gbpAccountResponse.Success);

            var request = new TransferBuilder(aliceClient)
                .AddOwnershipTransfer(gbpAccountId, gbpId, _test.Identity.Id, bob.Id)
                .Build();
            var response = await aliceClient.Write(request);
            Assert.IsTrue(response.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowInstanceTransfer()
        {
            var alice = _test.IdentityBuilder.Create();
            var bob = _test.IdentityBuilder.Create();
            var gbpId = _test.CreateUniqueId("gbp");
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateQuantifiableAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceTransferForIdentity(alice.Id, false)
                        .BuildInstances())
                .Build();
            var createIdentityResponse = await _test.Client.CreateIdentity(alice);
            Assert.IsTrue(createIdentityResponse.Success);
            var aliceClient = await ClientBuilder.CreateWithExistingIdentity(TestEnvironment.DefaultClientSettings, alice);

            var bobResponse = await _test.Client.CreateIdentity(bob);
            Assert.IsTrue(bobResponse.Success);

            var newQuantifiableAssetTypeResponse = await _test.Client.CreateQuantifiableAssetType(gbpId, 1, permissions);
            Assert.IsTrue(newQuantifiableAssetTypeResponse.Success);

            var gbpAccountId = _test.CreateUniqueId("gbpAccount");
            var gbpAccountResponse = await _test.Client.CreateQuantifiableAccount(gbpAccountId, gbpId);
            Assert.IsTrue(gbpAccountResponse.Success);

            var request = new TransferBuilder(aliceClient)
                .AddOwnershipTransfer(gbpAccountId, gbpId, _test.Identity.Id, bob.Id)
                .Build();
            var response = await aliceClient.Write(request);
            Assert.IsFalse(response.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowInstanceTypeOwnerTransfer()
        {
            var alice = _test.IdentityBuilder.Create();
            var gbpId = _test.CreateUniqueId("gbp");
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateQuantifiableAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceTransferForTypeOwner(false)
                        .BuildInstances())
                .Build();
            var createIdentityResponse = await _test.Client.CreateIdentity(alice);
            Assert.IsTrue(createIdentityResponse.Success);

            var newQuantifiableAssetTypeResponse = await _test.Client.CreateQuantifiableAssetType(gbpId, 1, permissions);
            Assert.IsTrue(newQuantifiableAssetTypeResponse.Success);

            var gbpAccountId = _test.CreateUniqueId("gbpAccount");
            var gbpAccountResponse = await _test.Client.CreateQuantifiableAccount(gbpAccountId, gbpId);
            Assert.IsTrue(gbpAccountResponse.Success);

            var request = new TransferBuilder(_test.Client)
                .AddOwnershipTransfer(gbpAccountId, gbpId, _test.Identity.Id, alice.Id)
                .Build();
            var response = await _test.Client.Write(request);
            Assert.IsFalse(response.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowInstanceInstanceOwnerTransfer()
        {
            var alice = _test.IdentityBuilder.Create();
            var gbpId = _test.CreateUniqueId("gbp");
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateQuantifiableAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceTransferForInstanceOwner(false)
                        .BuildInstances())
                .Build();
            var createIdentityResponse = await _test.Client.CreateIdentity(alice);
            Assert.IsTrue(createIdentityResponse.Success);

            var newQuantifiableAssetTypeResponse = await _test.Client.CreateQuantifiableAssetType(gbpId, 1, permissions);
            Assert.IsTrue(newQuantifiableAssetTypeResponse.Success);

            var gbpAccountId = _test.CreateUniqueId("gbpAccount");
            var gbpAccountResponse = await _test.Client.CreateQuantifiableAccount(gbpAccountId, gbpId);
            Assert.IsTrue(gbpAccountResponse.Success);

            var request = new TransferBuilder(_test.Client)
                .AddOwnershipTransfer(gbpAccountId, gbpId, _test.Identity.Id, alice.Id)
                .Build();
            var response = await _test.Client.Write(request);
            Assert.IsFalse(response.Success);
        }

        [TestMethod]
        public async Task ShouldAllowInstanceAddQuantity()
        {
            var alice = _test.IdentityBuilder.Create();
            var gbpId = _test.CreateUniqueId("gbp");
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateQuantifiableAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceAddQuantityForEveryone(true)
                        .BuildInstances())
                .Build();
            var createIdentityResponse = await _test.Client.CreateIdentity(alice);
            Assert.IsTrue(createIdentityResponse.Success);
            var aliceClient = await ClientBuilder.CreateWithExistingIdentity(TestEnvironment.DefaultClientSettings, alice);

            var newQuantifiableAssetTypeResponse = await _test.Client.CreateQuantifiableAssetType(gbpId, 1, permissions);
            Assert.IsTrue(newQuantifiableAssetTypeResponse.Success);

            var gbpAccountId = _test.CreateUniqueId("gbpAccount");
            var gbpAccountResponse = await _test.Client.CreateQuantifiableAccount(gbpAccountId, gbpId);
            Assert.IsTrue(gbpAccountResponse.Success);

            var result = await aliceClient.AddBalance(gbpAccountId, gbpId, 100);
            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowInstanceAddQuantity()
        {
            var alice = _test.IdentityBuilder.Create();
            var gbpId = _test.CreateUniqueId("gbp");
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateQuantifiableAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceAddQuantityForIdentity(alice.Id, false)
                        .BuildInstances())
                .Build();
            var createIdentityResponse = await _test.Client.CreateIdentity(alice);
            Assert.IsTrue(createIdentityResponse.Success);
            var aliceClient = await ClientBuilder.CreateWithExistingIdentity(TestEnvironment.DefaultClientSettings, alice);

            var newQuantifiableAssetTypeResponse = await _test.Client.CreateQuantifiableAssetType(gbpId, 1, permissions);
            Assert.IsTrue(newQuantifiableAssetTypeResponse.Success);

            var gbpAccountId = _test.CreateUniqueId("gbpAccount");
            var gbpAccountResponse = await _test.Client.CreateQuantifiableAccount(gbpAccountId, gbpId);
            Assert.IsTrue(gbpAccountResponse.Success);

            var result = await aliceClient.AddBalance(gbpAccountId, gbpId, 100);
            Assert.IsFalse(result.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowInstanceTypeOwnerAddQuantity()
        {
            var gbpId = _test.CreateUniqueId("gbp");
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateQuantifiableAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceAddQuantityForTypeOwner(false)
                        .BuildInstances())
                .Build();
            var newQuantifiableAssetTypeResponse = await _test.Client.CreateQuantifiableAssetType(gbpId, 1, permissions);
            Assert.IsTrue(newQuantifiableAssetTypeResponse.Success);

            var gbpAccountId = _test.CreateUniqueId("gbpAccount");
            var gbpAccountResponse = await _test.Client.CreateQuantifiableAccount(gbpAccountId, gbpId);
            Assert.IsTrue(gbpAccountResponse.Success);

            var result = await _test.Client.AddBalance(gbpAccountId, gbpId, 100);
            Assert.IsFalse(result.Success);
        }

        [TestMethod]
        public async Task ShouldNotAllowInstanceInstanceOwnerAddQuantity()
        {
            var gbpId = _test.CreateUniqueId("gbp");
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .WithInstancePermission(
                    new CreateQuantifiableAssetTypeInstancesPermissionsBuilder()
                        .WithInstanceAddQuantityForInstanceOwner(false)
                        .BuildInstances())
                .Build();
            var newQuantifiableAssetTypeResponse = await _test.Client.CreateQuantifiableAssetType(gbpId, 1, permissions);
            Assert.IsTrue(newQuantifiableAssetTypeResponse.Success);

            var gbpAccountId = _test.CreateUniqueId("gbpAccount");
            var gbpAccountResponse = await _test.Client.CreateQuantifiableAccount(gbpAccountId, gbpId);
            Assert.IsTrue(gbpAccountResponse.Success);

            var result = await _test.Client.AddBalance(gbpAccountId, gbpId, 100);
            Assert.IsFalse(result.Success);
        }
    }
}