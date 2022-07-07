using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BouncyCastleCrypto;
using IntegrationTests.Support;
using Iov42sdk;
using Iov42sdk.Models;
using Iov42sdk.Models.CreateClaims;
using Iov42sdk.Support;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests
{
    [TestClass]
    public class TestIdentityClaimsAndEndorsements
    {
        [TestMethod]
        public async Task ShouldCreateASingleIdentityClaim()
        {
            using var test = new IntegrationTestCreation();
            var birthdayClaim = Guid.NewGuid().ToString();
            var response = await test.Client.CreateIdentityClaims(birthdayClaim);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsFalse(string.IsNullOrEmpty(response.Value.RequestId));
            Assert.IsFalse(string.IsNullOrEmpty(response.Value.Proof));
            Assert.AreEqual(1, response.Value.Resources.Length);
            Assert.IsNull(response.Value.Errors);
        }

        [TestMethod]
        public async Task ShouldCreateMultipleIdentityClaims()
        {
            using var test = new IntegrationTestCreation();
            var birthdayClaim = Guid.NewGuid().ToString();
            var employerIov42 = Guid.NewGuid().ToString();
            var response = await test.Client.CreateIdentityClaims(birthdayClaim, employerIov42);
            
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsFalse(string.IsNullOrEmpty(response.Value.RequestId));
            Assert.IsFalse(string.IsNullOrEmpty(response.Value.Proof));
            Assert.AreEqual(2, response.Value.Resources.Length);
            Assert.IsNull(response.Value.Errors);
        }

        [TestMethod]
        public async Task ShouldRetrieveAnIdentityClaim()
        {
            using var test = new IntegrationTestCreation();
            var birthdayClaim = Guid.NewGuid().ToString();
            var employerIov42 = Guid.NewGuid().ToString();
            var _ = await test.Client.CreateIdentityClaims(birthdayClaim, employerIov42);
            
            var retrievedClaim = await TestHelper.CallAndRetry(() => test.Client.GetIdentityClaim(birthdayClaim));
            
            Assert.IsNotNull(retrievedClaim);
            Assert.IsTrue(retrievedClaim.Success);
            Assert.IsNull(retrievedClaim.Value.DelegateIdentityId);
            Assert.AreEqual(test.Identity.Crypto.GetHash(birthdayClaim), retrievedClaim.Value.Claim);
            Assert.IsFalse(string.IsNullOrEmpty(retrievedClaim.Value.Proof));
            Assert.AreEqual(0, retrievedClaim.Value.Endorsements.Length);
        }

        [TestMethod]
        public async Task ShouldRetrieveAnIdentityClaimForADifferentUser()
        {
            using var other = new IntegrationTestCreation();
            using var test = new IntegrationTestCreation();
            var birthdayClaim = Guid.NewGuid().ToString();
            var employerIov42 = Guid.NewGuid().ToString();
            var _ = await test.Client.CreateIdentityClaims(birthdayClaim, employerIov42);
            
            // Allow eventual consistency to persist the data
            await Task.Delay(500);
            var retrievedClaim = await TestHelper.CallAndRetry(() => other.Client.GetIdentityClaim(test.Identity.Id, birthdayClaim));
            
            Assert.IsNotNull(retrievedClaim);
            Assert.IsTrue(retrievedClaim.Success);
            Assert.IsNull(retrievedClaim.Value.DelegateIdentityId);
            Assert.AreEqual(test.Identity.Crypto.GetHash(birthdayClaim), retrievedClaim.Value.Claim);
            Assert.IsFalse(string.IsNullOrEmpty(retrievedClaim.Value.Proof));
            Assert.AreEqual(0, retrievedClaim.Value.Endorsements.Length);
        }

        [TestMethod]
        public async Task ShouldRetrieveAllIdentityClaims()
        {
            using var test = new IntegrationTestCreation();
            var birthdayClaim = Guid.NewGuid().ToString();
            var employerIov42 = Guid.NewGuid().ToString();
            var _ = await test.Client.CreateIdentityClaims(birthdayClaim, employerIov42);

            var retrievedClaims = await TestHelper.CallAndRetry(() => test.Client.GetIdentityClaims());
            
            Assert.IsNotNull(retrievedClaims);
            Assert.IsTrue(retrievedClaims.Success);
            Assert.IsNull(retrievedClaims.Value.Next);
            Assert.AreEqual(2, retrievedClaims.Value.Claims.Length);
            var claims = retrievedClaims.Value.Claims.OrderBy(x => x.Claim).ToArray();
            var expected = test.Identity.Crypto.GetHash(birthdayClaim);
            var claim = claims.FirstOrDefault(x => x.Claim == expected);
            Assert.IsNotNull(claim);
            Assert.IsFalse(string.IsNullOrEmpty(claim.Proof));
            Assert.IsNotNull(claim.Resource);
            Assert.IsNull(claim.DelegateIdentityId);
            expected = test.Identity.Crypto.GetHash(employerIov42);
            claim = claims.FirstOrDefault(x => x.Claim == expected);
            Assert.IsNotNull(claim);
            Assert.IsFalse(string.IsNullOrEmpty(claim.Proof));
            Assert.IsNotNull(claim.Resource);
            Assert.IsNull(claim.DelegateIdentityId);
        }

        [TestMethod]
        public async Task ShouldRetrieveAllIdentityClaimsForDifferentIdentity()
        {
            using var test = new IntegrationTestCreation();
            using var bob = new IntegrationTestCreation();
            var birthdayClaim = Guid.NewGuid().ToString();
            var employerIov42 = Guid.NewGuid().ToString();
            var _ = await bob.Client.CreateIdentityClaims(birthdayClaim, employerIov42);

            // Allow eventual consistency to persist the data
            await Task.Delay(500);
            var retrievedClaims = await TestHelper.CallAndRetry(() => test.Client.GetIdentityClaims(bob.Identity.Id));
            
            Assert.IsNotNull(retrievedClaims);
            Assert.IsTrue(retrievedClaims.Success);
            Assert.IsNull(retrievedClaims.Value.Next);
            Assert.AreEqual(2, retrievedClaims.Value.Claims.Length);
            var claims = retrievedClaims.Value.Claims.OrderBy(x => x.Claim).ToArray();
            var expected = test.Identity.Crypto.GetHash(birthdayClaim);
            var claim = claims.FirstOrDefault(x => x.Claim == expected);
            Assert.IsNotNull(claim);
            Assert.IsFalse(string.IsNullOrEmpty(claim.Proof));
            Assert.IsNotNull(claim.Resource);
            Assert.IsNull(claim.DelegateIdentityId);
            expected = test.Identity.Crypto.GetHash(employerIov42);
            claim = claims.FirstOrDefault(x => x.Claim == expected);
            Assert.IsNotNull(claim);
            Assert.IsFalse(string.IsNullOrEmpty(claim.Proof));
            Assert.IsNotNull(claim.Resource);
            Assert.IsNull(claim.DelegateIdentityId);
        }

        [TestMethod]
        public async Task ShouldFetchNextIdentityClaim()
        {
            using var test = new IntegrationTestCreation();
            var birthdayClaim = Guid.NewGuid().ToString();
            var employerIov42 = Guid.NewGuid().ToString();
            var _ = await test.Client.CreateIdentityClaims(birthdayClaim, employerIov42);

            var retrievedClaims = await TestHelper.CallAndRetry(() => test.Client.GetIdentityClaims(1));

            Assert.IsNotNull(retrievedClaims);
            Assert.IsTrue(retrievedClaims.Success);
            Assert.IsNotNull(retrievedClaims.Value.Next);
            Assert.AreEqual(1, retrievedClaims.Value.Claims.Length);
            var firstClaim = retrievedClaims.Value.Claims.First();
            Assert.IsNotNull(firstClaim);
            Assert.IsFalse(string.IsNullOrEmpty(firstClaim.Proof));
            Assert.IsNotNull(firstClaim.Resource);
            Assert.IsNull(firstClaim.DelegateIdentityId);

            retrievedClaims = await TestHelper.CallAndRetry(() => test.Client.GetIdentityClaims(1, retrievedClaims.Value.Next));
            
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
        public async Task ShouldCreateASelfEndorsedIdentityEndorsement()
        {
            using var test = new IntegrationTestCreation();
            var birthdayClaim = Guid.NewGuid().ToString();
            var employerIov42 = Guid.NewGuid().ToString();
            var _ = await test.Client.CreateIdentityClaims(birthdayClaim, employerIov42);

            var endorsements = test.Client.CreateIdentityEndorsements(test.Identity.Id)
                .AddEndorsement(birthdayClaim)
                .AddEndorsement(employerIov42);
            var body = endorsements.GenerateIdentityEndorsementBody().Serialize();
            var header = test.Client.GenerateAuthorisation(body);
            var endorse = await test.Client.CreateIdentityClaimsEndorsements(endorsements, endorsements.RequestId, body, header);
            
            Assert.IsNotNull(endorse);
            Assert.IsTrue(endorse.Success);
            Assert.IsFalse(string.IsNullOrEmpty(endorse.Value.RequestId));
            Assert.IsFalse(endorse.Value.RequestIdReusable);
            Assert.IsFalse(string.IsNullOrEmpty(endorse.Value.Proof));
            Assert.IsTrue(endorse.Value.Resources.Length >= 2);
            Assert.IsNull(endorse.Value.Errors);
        }

        [TestMethod]
        public async Task ShouldCreateAnIdentityEndorsement()
        {
            using var test = new IntegrationTestCreation();
            using var iovBank = new IntegrationTestCreation();
            var birthdayClaim = Guid.NewGuid().ToString();
            var employerIov42 = Guid.NewGuid().ToString();
            var _ = await test.Client.CreateIdentityClaims(birthdayClaim, employerIov42);

            var endorsements = iovBank.Client.CreateIdentityEndorsements(test.Identity.Id)
                .AddEndorsement(birthdayClaim)
                .AddEndorsement(employerIov42);
            var body = endorsements.GenerateIdentityEndorsementBody().Serialize();
            var testHeader = test.Client.GenerateAuthorisation(body);
            var iovBankHeader = iovBank.Client.GenerateAuthorisation(body);
            var endorse = await test.Client.CreateIdentityClaimsEndorsements(endorsements, endorsements.RequestId, body, testHeader, iovBankHeader);
            
            Assert.IsNotNull(endorse);
            Assert.IsTrue(endorse.Success);
            Assert.IsFalse(string.IsNullOrEmpty(endorse.Value.RequestId));
            Assert.IsFalse(endorse.Value.RequestIdReusable);
            Assert.IsFalse(string.IsNullOrEmpty(endorse.Value.Proof));
            Assert.IsTrue(endorse.Value.Resources.Length >= 2);
            Assert.IsNull(endorse.Value.Errors);
        }

        [TestMethod]
        public async Task ShouldFetchEndorsement()
        {
            using var test = new IntegrationTestCreation();
            using var iovBank = new IntegrationTestCreation();
            var birthdayClaim = Guid.NewGuid().ToString();
            var employerIov42 = Guid.NewGuid().ToString();
            var _ = await test.Client.CreateIdentityClaims(birthdayClaim, employerIov42);

            var endorsements = iovBank.Client.CreateIdentityEndorsements(test.Identity.Id)
                .AddEndorsement(birthdayClaim)
                .AddEndorsement(employerIov42);
            var body = endorsements.GenerateIdentityEndorsementBody().Serialize();
            var testHeader = test.Client.GenerateAuthorisation(body);
            var iovBankHeader = iovBank.Client.GenerateAuthorisation(body);
            var __ = await test.Client.CreateIdentityClaimsEndorsements(endorsements, endorsements.RequestId, body, testHeader, iovBankHeader);
            
            var endorse = await TestHelper.CallAndRetry(() => test.Client.GetIdentityEndorsement(test.Identity.Id, birthdayClaim, iovBank.Identity.Id));
            
            Assert.IsNotNull(endorse);
            Assert.IsTrue(endorse.Success);
            Assert.IsFalse(string.IsNullOrEmpty(endorse.Value.EndorserId));
            Assert.IsTrue(string.IsNullOrEmpty(endorse.Value.DelegateIdentityId));
            Assert.IsFalse(string.IsNullOrEmpty(endorse.Value.Proof));
            Assert.IsFalse(string.IsNullOrEmpty(endorse.Value.Endorsement));

            var key = new BouncyKeyPair(new SerializedKeys().WithPublicKey(iovBank.Identity.Crypto.Pair.PublicKeyBase64String));
            var crypto = IntegrationTestCreation.CreateCrypto(key);
            var result = test.Client.VerifyIdentityEndorsement(crypto, test.Identity.Id, birthdayClaim, endorse.Value.Endorsement);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ShouldRetrieveEndorsementsOnAClaim()
        {
            using var test = new IntegrationTestCreation();
            using var iovBank = new IntegrationTestCreation();
            var birthdayClaim = Guid.NewGuid().ToString();
            var employerIov42 = Guid.NewGuid().ToString();
            var _ = await test.Client.CreateIdentityClaims(birthdayClaim, employerIov42);

            var endorsements = iovBank.Client.CreateIdentityEndorsements(test.Identity.Id)
                .AddEndorsement(birthdayClaim)
                .AddEndorsement(employerIov42);
            var body = endorsements.GenerateIdentityEndorsementBody().Serialize();
            var testHeader = test.Client.GenerateAuthorisation(body);
            var iovBankHeader = iovBank.Client.GenerateAuthorisation(body);
            var __ = await test.Client.CreateIdentityClaimsEndorsements(endorsements, endorsements.RequestId, body, testHeader, iovBankHeader);
            
            var retrievedClaim = await TestHelper.CallAndRetry(() => test.Client.GetIdentityClaim(birthdayClaim));
            
            Assert.IsNotNull(retrievedClaim);
            Assert.IsTrue(retrievedClaim.Success);
            Assert.IsNull(retrievedClaim.Value.DelegateIdentityId);
            Assert.AreEqual(test.Identity.Crypto.GetHash(birthdayClaim), retrievedClaim.Value.Claim);
            Assert.IsFalse(string.IsNullOrEmpty(retrievedClaim.Value.Proof));
            Assert.AreEqual(1, retrievedClaim.Value.Endorsements.Length);
            var endorsement = retrievedClaim.Value.Endorsements[0];
            Assert.IsFalse(string.IsNullOrEmpty(endorsement.Endorsement));
            Assert.IsTrue(string.IsNullOrEmpty(endorsement.DelegateIdentityId));
            Assert.IsFalse(string.IsNullOrEmpty(endorsement.Proof));
            Assert.IsFalse(string.IsNullOrEmpty(endorsement.EndorserId));
        }

        [TestMethod]
        public async Task ShouldCreateAnIdentityEndorsementWithNoPreviousClaim()
        {
            using var test = new IntegrationTestCreation();
            var birthdayClaim = Guid.NewGuid().ToString();
            var employerIov42 = Guid.NewGuid().ToString();

            var endorsements = test.Client.CreateIdentityEndorsements(test.Identity.Id)
                .AddEndorsement(birthdayClaim)
                .AddEndorsement(employerIov42)
                .AndCreateClaims();
            var body = endorsements.GenerateIdentityEndorsementBody().Serialize();
            var testHeader = test.Client.GenerateAuthorisation(body);
            var iovBankHeader = test.Client.GenerateAuthorisation(body);
            var endorse = await test.Client.CreateIdentityClaimsEndorsements(endorsements, endorsements.RequestId, body, testHeader, iovBankHeader);

            Assert.IsNotNull(endorse);
            Assert.IsTrue(endorse.Success);
            Assert.IsFalse(string.IsNullOrEmpty(endorse.Value.RequestId));
            Assert.IsFalse(endorse.Value.RequestIdReusable);
            Assert.IsFalse(string.IsNullOrEmpty(endorse.Value.Proof));
            Assert.IsTrue(endorse.Value.Resources.Length >= 2);
            Assert.IsNull(endorse.Value.Errors);
        }



        [TestMethod]
        public async Task ShouldCreateASingleIdentityClaimUsingRequest()
        {
            using var test = new IntegrationTestCreation();
            var birthdayClaim = Guid.NewGuid().ToString();
            var claims = new[] {birthdayClaim};
            var claimMap = claims.ToDictionary(x => test.Identity.Crypto.GetHash(x), x => x);
            var headers = test.Client.GenerateClaimsHeader(claimMap);
            var body = new CreateClaimsBody(NodeConstants.CreateIdentityClaimsRequestType, test.Identity.Id, claimMap.Keys.ToArray());
            var request = test.Client.BuildRequest(body).WithAdditionalHeaders(headers);
            var response = await test.Client.Write(request);
            response.VerifyWriteResult();
        }

        [TestMethod]
        public async Task ShouldCreateAnIdentityEndorsementUsingRequest()
        {
            using var test = new IntegrationTestCreation();
            using var iovBank = new IntegrationTestCreation();
            var birthdayClaim = Guid.NewGuid().ToString();
            var employerIov42 = Guid.NewGuid().ToString();
            var _ = await test.Client.CreateIdentityClaims(birthdayClaim, employerIov42);

            var endorsements = iovBank.Client.CreateIdentityEndorsements(test.Identity.Id)
                .AddEndorsement(birthdayClaim)
                .AddEndorsement(employerIov42);
            var body = endorsements.GenerateIdentityEndorsementBody().Serialize();
            var testHeader = test.Client.GenerateAuthorisation(body);
            var iovBankHeader = iovBank.Client.GenerateAuthorisation(body);
            var claimsHeader = test.Client.GenerateClaimsHeader(new Dictionary<string, string>());
            var request = new PlatformWriteRequest(endorsements.RequestId, body, new [] { testHeader, iovBankHeader }).WithAdditionalHeaders(claimsHeader);
            var endorse = await test.Client.Write(request);
            endorse.VerifyWriteResult(2);
        }
    }
}
