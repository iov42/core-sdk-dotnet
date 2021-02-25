using System;
using System.Threading.Tasks;
using BouncyCastleCrypto;
using IntegrationTests.Support;
using Iov42sdk.Connection;
using Iov42sdk.Identity;
using Iov42sdk.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests.UseCases
{
    [TestClass]
    public class TestCarTransfer
    {
        [TestMethod]
        public async Task ShouldTransferCar()
        {
            // Create an Identity for Motor Vehicle Authority (MVA). MVA is a imaginary state authority for motor vehicles
            var identityBuilder = new IdentityBuilder(k => new BouncyCrypto(new EcsdaCryptoEngine()));
            var mvaIdentity = identityBuilder.Create(CreateUniqueId("MVA"));
            var mvaClient = await ClientBuilder.CreateWithNewIdentity(TestEnvironment.Environment, mvaIdentity);
            
            // MVA creates an AssetType to represent a Car
            var carType = CreateUniqueId("Car");
            var carTypeResponse = await mvaClient.CreateUniqueAssetType(carType);
            Assert.IsTrue(carTypeResponse.Success);

            // Create an Identity for Alice (an individual)
            var aliceIdentity = identityBuilder.Create(CreateUniqueId("Alice"));
            var aliceClient = await ClientBuilder.CreateWithNewIdentity(TestEnvironment.Environment, aliceIdentity);

            // Create an Identity for Bob (an individual)
            var bobIdentity = identityBuilder.Create(CreateUniqueId("Bob"));
            var bobClient = await ClientBuilder.CreateWithNewIdentity(TestEnvironment.Environment, bobIdentity);

            // Alice creates an instance of a car AssetType
            var carId = CreateUniqueId("ACar");
            var carResponse = await aliceClient.CreateUniqueAsset(carId, carType);
            Assert.IsTrue(carResponse.Success);

            // Alice claims the first registration of the car happened in 2010
            var firstRegistration = "first-registration:10/02/2010";
            var claimResponse = await aliceClient.CreateAssetClaims(carType, carId, firstRegistration);
            Assert.IsTrue(claimResponse.Success);

            // MVA endorses Alice's claim of the registration date (as it's the authority then anyone can trust that endorsement). This requires
            // that both Alice and MVA signs the request
            var endorsements = mvaClient.CreateAssetEndorsements(carType, carId)
                .AddEndorsement(firstRegistration);
            var body = endorsements.GenerateAssetEndorsementBody(carType).Serialize();
            var aliceAuthorisation = aliceClient.GenerateAuthorisation(body);
            var mvaAuthorisation = mvaClient.GenerateAuthorisation(body);
            var endorsementResponse = await mvaClient.CreateAssetClaimsEndorsements(endorsements, endorsements.RequestId, body, aliceAuthorisation, mvaAuthorisation);
            Assert.IsTrue(endorsementResponse.Success);

            // Allow eventual consistency to persist the data
            await Task.Delay(500);

            // Bob wants to buy the car and checks that the registration claim from Alice is in fact endorsed by the MVA
            var getEndorsementResponse = await bobClient.GetAssetEndorsement(carType, carId, firstRegistration, mvaIdentity.Id);
            Assert.IsTrue(getEndorsementResponse.Success);
            Assert.IsNotNull(getEndorsementResponse.Value.Proof);

            // (In the real world) Bob is happy with the car and trusts the registration year now - he pays Alice the requested amount of money

            // Alice in turn transfers the car instance to Bob
            var transfer = aliceClient.CreateOwnershipTransfer(carId, carType, aliceIdentity.Id, bobIdentity.Id);
            var transferResponse = await aliceClient.TransferAssets(transfer);
            Assert.IsTrue(transferResponse.Success);

            // The end - at this point Bob is the owner of the Unique Asset (Alice's Car)
        }

        /// <summary>
        /// This ensures we get a unique id each time, with an optional root, so we can run the test multiple times. If we just used
        /// "car" for example them it would fail the second time you run the test - or if someone else had run the test previously
        /// </summary>
        /// <param name="root">An optional prefix to the id</param>
        /// <returns>The unique id</returns>
        private static string CreateUniqueId(string root = null)
        {
            var id = Guid.NewGuid().ToString();
            return root != null ? $"{root}-{id}" : id;
        }
    }
}
