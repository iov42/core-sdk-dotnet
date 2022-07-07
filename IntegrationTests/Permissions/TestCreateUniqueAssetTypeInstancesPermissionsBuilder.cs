using Iov42sdk.Models.Permissions;
using Iov42sdk.Support.Permissions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests.Permissions
{
    [TestClass]
    public class TestCreateUniqueAssetTypeInstancesPermissionsBuilder
    {
        [TestMethod]
        public void ShouldCreateAllNullFields()
        {
            var instances = new CreateUniqueAssetTypeInstancesPermissionsBuilder()
                .BuildInstances();
            AllNull(instances.CreateClaim, instances.EndorseClaim, instances.ReadEndorsement, instances.Read, instances.ReadClaim, instances.Create, instances.Transfer);
        }

        [TestMethod]
        public void ShouldCreatePermissionsForCreateClaim()
        {
            var instances = new CreateUniqueAssetTypeInstancesPermissionsBuilder()
                .WithInstanceCreateClaimForEveryone(false)
                .WithInstanceCreateClaimForIdentity("123", true)
                .WithInstanceCreateClaimForTypeOwner(false)
                .WithInstanceCreateClaimForInstanceOwner(true)
                .BuildInstances();
            AllNull(instances.EndorseClaim, instances.ReadEndorsement, instances.Read, instances.ReadClaim, instances.Create, instances.Transfer);
            CheckInstance(instances.CreateClaim, 4, new [] {"Identity(123)", InstancePermission.InstanceOwner}, new [] {InstancePermission.Everyone, InstancePermission.TypeOwner});
        }

        [TestMethod]
        public void ShouldCreatePermissionsForCreate()
        {
            var instances = new CreateUniqueAssetTypeInstancesPermissionsBuilder()
                .WithInstanceCreateForEveryone(true)
                .WithInstanceCreateForIdentity("123", false)
                .WithInstanceCreateForTypeOwner(true)
                .BuildInstances();
            AllNull(instances.EndorseClaim, instances.ReadEndorsement, instances.Read, instances.ReadClaim, instances.CreateClaim, instances.Transfer);
            CheckInstance(instances.Create, 3, new [] {InstancePermission.Everyone, InstancePermission.TypeOwner}, new [] {"Identity(123)"});
        }

        [TestMethod]
        public void ShouldReadPermissionsForRead()
        {
            var instances = new CreateUniqueAssetTypeInstancesPermissionsBuilder()
                .WithInstanceReadForEveryone(true)
                .WithInstanceReadForIdentity("123", true)
                .WithInstanceReadForTypeOwner(false)
                .WithInstanceReadForInstanceOwner(false)
                .BuildInstances();
            AllNull(instances.EndorseClaim, instances.ReadEndorsement, instances.Create, instances.ReadClaim, instances.CreateClaim, instances.Transfer);
            CheckInstance(instances.Read, 4, new [] {InstancePermission.Everyone, "Identity(123)"}, new [] {InstancePermission.TypeOwner, InstancePermission.InstanceOwner});
        }

        [TestMethod]
        public void ShouldCreatePermissionsForReadClaim()
        {
            var instances = new CreateUniqueAssetTypeInstancesPermissionsBuilder()
                .WithInstanceReadClaimForEveryone(false)
                .WithInstanceReadClaimForIdentity("123", true)
                .WithInstanceReadClaimForTypeOwner(false)
                .WithInstanceReadClaimForInstanceOwner(true)
                .BuildInstances();
            AllNull(instances.EndorseClaim, instances.ReadEndorsement, instances.Read, instances.CreateClaim, instances.Create, instances.Transfer);
            CheckInstance(instances.ReadClaim, 4, new [] {"Identity(123)", InstancePermission.InstanceOwner}, new [] {InstancePermission.Everyone, InstancePermission.TypeOwner});
        }

        [TestMethod]
        public void ShouldCreatePermissionsForEndorseClaim()
        {
            var instances = new CreateUniqueAssetTypeInstancesPermissionsBuilder()
                .WithInstanceEndorseClaimForEveryone(true)
                .WithInstanceEndorseClaimForIdentity("123", false)
                .WithInstanceEndorseClaimForTypeOwner(true)
                .WithInstanceEndorseClaimForInstanceOwner(true)
                .BuildInstances();
            AllNull(instances.ReadClaim, instances.ReadEndorsement, instances.Read, instances.CreateClaim, instances.Create, instances.Transfer);
            CheckInstance(instances.EndorseClaim, 4, new [] {InstancePermission.InstanceOwner, InstancePermission.Everyone, InstancePermission.TypeOwner}, new [] {"Identity(123)"});
        }

        [TestMethod]
        public void ShouldCreatePermissionsForReadEndorsement()
        {
            var instances = new CreateUniqueAssetTypeInstancesPermissionsBuilder()
                .WithInstanceReadEndorsementForEveryone(false)
                .WithInstanceReadEndorsementForIdentity("123", false)
                .WithInstanceReadEndorsementForTypeOwner(false)
                .WithInstanceReadEndorsementForInstanceOwner(true)
                .BuildInstances();
            AllNull(instances.ReadClaim, instances.EndorseClaim, instances.Read, instances.CreateClaim, instances.Create, instances.Transfer);
            CheckInstance(instances.ReadEndorsement, 4, new [] {InstancePermission.InstanceOwner}, new[] {InstancePermission.Everyone, InstancePermission.TypeOwner, "Identity(123)"});
        }

        [TestMethod]
        public void ShouldCreatePermissionsForTransfer()
        {
            var instances = new CreateUniqueAssetTypeInstancesPermissionsBuilder()
                .WithInstanceTransferForEveryone(false)
                .WithInstanceTransferForIdentity("123", true)
                .WithInstanceTransferForTypeOwner(true)
                .WithInstanceTransferForInstanceOwner(false)
                .BuildInstances();
            AllNull(instances.ReadClaim, instances.EndorseClaim, instances.Read, instances.CreateClaim, instances.Create, instances.ReadEndorsement);
            CheckInstance(instances.Transfer, 4, new [] {InstancePermission.TypeOwner, "Identity(123)"}, new[] {InstancePermission.Everyone, InstancePermission.InstanceOwner});
        }

        [TestMethod]
        public void ShouldCreatePermissionsForMultipleIdentities()
        {
            var instances = new CreateUniqueAssetTypeInstancesPermissionsBuilder()
                .WithInstanceTransferForIdentity("123", true)
                .WithInstanceTransferForIdentity("456", false)
                .WithInstanceTransferForInstanceOwner(false)
                .BuildInstances();
            AllNull(instances.ReadClaim, instances.EndorseClaim, instances.Read, instances.CreateClaim, instances.Create, instances.ReadEndorsement);
            CheckInstance(instances.Transfer, 3, new [] {"Identity(123)"}, new[] {"Identity(456)", InstancePermission.InstanceOwner});
        }

        private static void AllNull(params InstancePermission[] instances)
        {
            foreach (var instancePermission in instances) 
                Assert.IsNull(instancePermission);
        }

        private static void CheckInstance(InstancePermission permission, int expectedLength, string[] granted, string[] denied)
        {
            Assert.IsNotNull(permission);
            Assert.AreEqual(expectedLength, permission.Count);
            foreach (var grant in granted) 
                Assert.AreEqual(GrantOrDeny.Grant, permission[grant]);
            foreach (var deny in denied) 
                Assert.AreEqual(GrantOrDeny.Deny, permission[deny]);
        }
    }
}
