using System;
using System.Linq;
using System.Threading.Tasks;
using IntegrationTests.Support;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests
{
    [TestClass]
    public class TestAssetTypeClaimsAndEndorsements
    {
        [TestMethod]
        public async Task ShouldCreateAnAssetTypeClaim()
        {
            using var test = new IntegrationTestCreation();
            var locationClaim = Guid.NewGuid().ToString();
            var regulatoryClaim = Guid.NewGuid().ToString();
            var horseId = test.CreateUniqueId("horse");
            var _ = await test.Client.CreateUniqueAssetType(horseId);
            var response = await test.Client.CreateAssetTypeClaims(horseId, locationClaim, regulatoryClaim);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsFalse(string.IsNullOrEmpty(response.Value.RequestId));
            Assert.IsFalse(string.IsNullOrEmpty(response.Value.Proof));
            Assert.AreEqual(2, response.Value.Resources.Length);
            Assert.IsNull(response.Value.Errors);
        }

        [TestMethod]
        public async Task ShouldRetrieveAnAssetTypeClaim()
        {
            using var test = new IntegrationTestCreation();
            var locationClaim = Guid.NewGuid().ToString();
            var regulatoryClaim = Guid.NewGuid().ToString();
            var horseId = test.CreateUniqueId("horse");
            var _ = await test.Client.CreateUniqueAssetType(horseId);
            var ___ = await test.Client.CreateAssetTypeClaims(horseId, locationClaim, regulatoryClaim);
            var retrievedClaim = await test.Client.GetAssetTypeClaim(horseId, locationClaim);
            Assert.IsNotNull(retrievedClaim);
            Assert.IsTrue(retrievedClaim.Success);
            Assert.IsNull(retrievedClaim.Value.DelegateIdentityId);
            Assert.AreEqual(test.Identity.Crypto.GetHash(locationClaim), retrievedClaim.Value.Claim);
            Assert.IsFalse(string.IsNullOrEmpty(retrievedClaim.Value.Proof));
            Assert.AreEqual(0, retrievedClaim.Value.Endorsements.Length);
        }

        [TestMethod]
        public async Task ShouldRetrieveAllAssetTypeClaims()
        {
            using var test = new IntegrationTestCreation();
            var locationClaim = Guid.NewGuid().ToString();
            var regulatoryClaim = Guid.NewGuid().ToString();
            var horseId = test.CreateUniqueId("horse");
            var _ = await test.Client.CreateUniqueAssetType(horseId);
            var ___ = await test.Client.CreateAssetTypeClaims(horseId, locationClaim, regulatoryClaim);
            var retrievedClaims = await test.Client.GetAssetTypeClaims(horseId);
            Assert.IsNotNull(retrievedClaims);
            Assert.IsTrue(retrievedClaims.Success);
            Assert.IsNull(retrievedClaims.Value.Next);
            Assert.AreEqual(2, retrievedClaims.Value.Claims.Length);
            var claims = retrievedClaims.Value.Claims.ToArray();
            var expected = test.Identity.Crypto.GetHash(locationClaim);
            var claim = claims.FirstOrDefault(x => x.Claim == expected);
            Assert.IsNotNull(claim);
            Assert.IsFalse(string.IsNullOrEmpty(claim.Proof));
            Assert.IsNotNull(claim.Resource);
            Assert.IsNull(claim.DelegateIdentityId);
            expected = test.Identity.Crypto.GetHash(regulatoryClaim);
            claim = claims.FirstOrDefault(x => x.Claim == expected);
            Assert.IsNotNull(claim);
            Assert.IsFalse(string.IsNullOrEmpty(claim.Proof));
            Assert.IsNotNull(claim.Resource);
            Assert.IsNull(claim.DelegateIdentityId);
        }

        [TestMethod]
        public async Task ShouldFetchNextAssetTypeClaim()
        {
            using var test = new IntegrationTestCreation();
            var locationClaim = Guid.NewGuid().ToString();
            var regulatoryClaim = Guid.NewGuid().ToString();
            var horseId = test.CreateUniqueId("horse");
            var _ = await test.Client.CreateUniqueAssetType(horseId);
            var ___ = await test.Client.CreateAssetTypeClaims(horseId, locationClaim, regulatoryClaim);
            var retrievedClaims = await test.Client.GetAssetTypeClaims(horseId, 1);
            Assert.IsNotNull(retrievedClaims);
            Assert.IsTrue(retrievedClaims.Success);
            Assert.IsNotNull(retrievedClaims.Value.Next);
            Assert.AreEqual(1, retrievedClaims.Value.Claims.Length);
            var firstClaim = retrievedClaims.Value.Claims.First();
            Assert.IsNotNull(firstClaim);
            Assert.IsFalse(string.IsNullOrEmpty(firstClaim.Proof));
            Assert.IsNotNull(firstClaim.Resource);
            Assert.IsNull(firstClaim.DelegateIdentityId);

            retrievedClaims = await test.Client.GetAssetTypeClaims(horseId, 1, retrievedClaims.Value.Next);
            Assert.IsNotNull(retrievedClaims);
            Assert.IsTrue(retrievedClaims.Success);
            Assert.IsNull(retrievedClaims.Value.Next);
            Assert.AreEqual(1, retrievedClaims.Value.Claims.Length);
            var secondClaim = retrievedClaims.Value.Claims.First();
            Assert.IsNotNull(secondClaim);
            Assert.IsFalse(string.IsNullOrEmpty(secondClaim.Proof));
            Assert.IsNotNull(secondClaim.Resource);
            Assert.IsNull(secondClaim.DelegateIdentityId);

            Assert.AreNotEqual(firstClaim.Claim, secondClaim.Claim);
        }

        [TestMethod]
        public async Task ShouldCreateAnAssetTypeEndorsement()
        {
            using var test = new IntegrationTestCreation();
            using var hsbc = new IntegrationTestCreation();
            var locationClaim = Guid.NewGuid().ToString();
            var regulatoryClaim = Guid.NewGuid().ToString();
            var horseId = test.CreateUniqueId("horse");
            var _ = await test.Client.CreateUniqueAssetType(horseId);
            var ____ = await test.Client.CreateAssetTypeClaims(horseId, locationClaim, regulatoryClaim);
            var endorsements = hsbc.Client.CreateAssetTypeEndorsements(horseId)
                .AddEndorsement(locationClaim)
                .AddEndorsement(regulatoryClaim);
            var body = endorsements.GenerateAssetTypeEndorsementBody();
            var testHeader = test.Client.GenerateAuthorisation(body);
            var hsbcHeader = hsbc.Client.GenerateAuthorisation(body);
            var endorse = await test.Client.CreateAssetTypeClaimsEndorsements(endorsements, body, testHeader, hsbcHeader);
            Assert.IsNotNull(endorse);
            Assert.IsTrue(endorse.Success);
            Assert.IsFalse(string.IsNullOrEmpty(endorse.Value.RequestId));
            Assert.IsFalse(endorse.Value.RequestIdReusable);
            Assert.IsFalse(string.IsNullOrEmpty(endorse.Value.Proof));
            Assert.IsTrue(endorse.Value.Resources.Length >= 2);
            Assert.IsNull(endorse.Value.Errors);
        }

        [TestMethod]
        public async Task ShouldFetchAssetTypeEndorsement()
        {
            using var test = new IntegrationTestCreation();
            using var hsbc = new IntegrationTestCreation();
            var locationClaim = Guid.NewGuid().ToString();
            var regulatoryClaim = Guid.NewGuid().ToString();
            var horseId = test.CreateUniqueId("horse");
            var _ = await test.Client.CreateUniqueAssetType(horseId);
            var ____ = await test.Client.CreateAssetTypeClaims(horseId, locationClaim, regulatoryClaim);
            var endorsements = hsbc.Client.CreateAssetTypeEndorsements(horseId)
                .AddEndorsement(locationClaim)
                .AddEndorsement(regulatoryClaim);
            var body = endorsements.GenerateAssetTypeEndorsementBody();
            var testHeader = test.Client.GenerateAuthorisation(body);
            var hsbcHeader = hsbc.Client.GenerateAuthorisation(body);
            var ______ = await test.Client.CreateAssetTypeClaimsEndorsements(endorsements, body, testHeader, hsbcHeader);
            var endorse = await test.Client.GetAssetTypeEndorsement(horseId, locationClaim, hsbc.Identity.Id);
            Assert.IsNotNull(endorse);
            Assert.IsTrue(endorse.Success);
            Assert.IsFalse(string.IsNullOrEmpty(endorse.Value.Proof));
            Assert.IsTrue(string.IsNullOrEmpty(endorse.Value.DelegateIdentityId));
            Assert.IsFalse(string.IsNullOrEmpty(endorse.Value.EndorserId));
            Assert.IsFalse(string.IsNullOrEmpty(endorse.Value.Endorsement));
        }
        
        [TestMethod]
        public async Task ShouldRetrieveAssetTypeEndorsementsOnAClaim()
        {
            using var test = new IntegrationTestCreation();
            using var hsbc = new IntegrationTestCreation();
            var locationClaim = Guid.NewGuid().ToString();
            var regulatoryClaim = Guid.NewGuid().ToString();
            var horseId = test.CreateUniqueId("horse");
            var _ = await test.Client.CreateUniqueAssetType(horseId);
            var ____ = await test.Client.CreateAssetTypeClaims(horseId, locationClaim, regulatoryClaim);
            var endorsements = hsbc.Client.CreateAssetTypeEndorsements(horseId)
                .AddEndorsement(locationClaim)
                .AddEndorsement(regulatoryClaim);
            var body = endorsements.GenerateAssetTypeEndorsementBody();
            var testHeader = test.Client.GenerateAuthorisation(body);
            var hsbcHeader = hsbc.Client.GenerateAuthorisation(body);
            var ______ = await test.Client.CreateAssetTypeClaimsEndorsements(endorsements, body, testHeader, hsbcHeader);
            var retrievedClaim = await test.Client.GetAssetTypeClaim(horseId, locationClaim);
            Assert.IsNotNull(retrievedClaim);
            Assert.IsTrue(retrievedClaim.Success);
            Assert.IsNull(retrievedClaim.Value.DelegateIdentityId);
            Assert.AreEqual(test.Identity.Crypto.GetHash(locationClaim), retrievedClaim.Value.Claim);
            Assert.IsFalse(string.IsNullOrEmpty(retrievedClaim.Value.Proof));
            Assert.AreEqual(1, retrievedClaim.Value.Endorsements.Length);
            var endorsement = retrievedClaim.Value.Endorsements[0];
            Assert.IsFalse(string.IsNullOrEmpty(endorsement.Proof));
            Assert.IsTrue(string.IsNullOrEmpty(endorsement.DelegateIdentityId));
            Assert.IsFalse(string.IsNullOrEmpty(endorsement.Endorsement));
            Assert.IsFalse(string.IsNullOrEmpty(endorsement.EndorserId));
        }
    }
}