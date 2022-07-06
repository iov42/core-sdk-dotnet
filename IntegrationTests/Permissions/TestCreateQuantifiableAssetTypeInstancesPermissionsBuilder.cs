using Iov42sdk.Models.Permissions;
using Iov42sdk.Support.Permissions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests.Permissions
{
    [TestClass]
    public class TestCreateQuantifiableAssetTypeInstancesPermissionsBuilder
    {
        [TestMethod]
        public void ShouldCreateAllNullFields()
        {
            var instances = new CreateQuantifiableAssetTypeInstancesPermissionsBuilder()
                .BuildInstances();
            PermissionTestHelper.AllNull(instances.CreateClaim, instances.EndorseClaim, instances.ReadEndorsement, instances.Read, instances.ReadClaim, instances.Create, instances.Transfer, instances.AddQuantity);
        }

        [TestMethod]
        public void ShouldCreatePermissionsForCreateClaim()
        {
            var instances = new CreateQuantifiableAssetTypeInstancesPermissionsBuilder()
                .WithInstanceCreateClaimForEveryone(false)
                .WithInstanceCreateClaimForIdentity("123", true)
                .WithInstanceCreateClaimForTypeOwner(false)
                .WithInstanceCreateClaimForInstanceOwner(true)
                .BuildInstances();
            PermissionTestHelper.AllNull(instances.EndorseClaim, instances.ReadEndorsement, instances.Read, instances.ReadClaim, instances.Create, instances.Transfer, instances.AddQuantity);
            PermissionTestHelper.CheckInstance(instances.CreateClaim, 4, new [] {"123", InstancePermission.InstanceOwner}, new [] {InstancePermission.Everyone, InstancePermission.TypeOwner});
        }

        [TestMethod]
        public void ShouldCreatePermissionsForCreate()
        {
            var instances = new CreateQuantifiableAssetTypeInstancesPermissionsBuilder()
                .WithInstanceCreateForEveryone(true)
                .WithInstanceCreateForTypeOwner(false)
                .BuildInstances();
            PermissionTestHelper.AllNull(instances.EndorseClaim, instances.ReadEndorsement, instances.Read, instances.ReadClaim, instances.CreateClaim, instances.Transfer, instances.AddQuantity);
            PermissionTestHelper.CheckInstance(instances.Create, 2, new [] {InstancePermission.Everyone}, new [] {InstancePermission.TypeOwner});
        }

        [TestMethod]
        public void ShouldReadPermissionsForRead()
        {
            var instances = new CreateQuantifiableAssetTypeInstancesPermissionsBuilder()
                .WithInstanceReadForEveryone(true)
                .WithInstanceReadForIdentity("123", true)
                .WithInstanceReadForTypeOwner(false)
                .WithInstanceReadForInstanceOwner(false)
                .BuildInstances();
            PermissionTestHelper.AllNull(instances.EndorseClaim, instances.ReadEndorsement, instances.Create, instances.ReadClaim, instances.CreateClaim, instances.Transfer, instances.AddQuantity);
            PermissionTestHelper.CheckInstance(instances.Read, 4, new [] {InstancePermission.Everyone, "123"}, new [] {InstancePermission.TypeOwner, InstancePermission.InstanceOwner});
        }

        [TestMethod]
        public void ShouldCreatePermissionsForReadClaim()
        {
            var instances = new CreateQuantifiableAssetTypeInstancesPermissionsBuilder()
                .WithInstanceReadClaimForEveryone(false)
                .WithInstanceReadClaimForIdentity("123", true)
                .WithInstanceReadClaimForTypeOwner(false)
                .WithInstanceReadClaimForInstanceOwner(true)
                .BuildInstances();
            PermissionTestHelper.AllNull(instances.EndorseClaim, instances.ReadEndorsement, instances.Read, instances.CreateClaim, instances.Create, instances.Transfer, instances.AddQuantity);
            PermissionTestHelper.CheckInstance(instances.ReadClaim, 4, new [] {"123", InstancePermission.InstanceOwner}, new [] {InstancePermission.Everyone, InstancePermission.TypeOwner});
        }

        [TestMethod]
        public void ShouldCreatePermissionsForEndorseClaim()
        {
            var instances = new CreateQuantifiableAssetTypeInstancesPermissionsBuilder()
                .WithInstanceEndorseClaimForEveryone(true)
                .WithInstanceEndorseClaimForIdentity("123", false)
                .WithInstanceEndorseClaimForTypeOwner(true)
                .WithInstanceEndorseClaimForInstanceOwner(true)
                .BuildInstances();
            PermissionTestHelper.AllNull(instances.ReadClaim, instances.ReadEndorsement, instances.Read, instances.CreateClaim, instances.Create, instances.Transfer, instances.AddQuantity);
            PermissionTestHelper.CheckInstance(instances.EndorseClaim, 4, new [] {InstancePermission.InstanceOwner, InstancePermission.Everyone, InstancePermission.TypeOwner}, new [] {"123"});
        }

        [TestMethod]
        public void ShouldCreatePermissionsForReadEndorsement()
        {
            var instances = new CreateQuantifiableAssetTypeInstancesPermissionsBuilder()
                .WithInstanceReadEndorsementForEveryone(false)
                .WithInstanceReadEndorsementForIdentity("123", false)
                .WithInstanceReadEndorsementForTypeOwner(false)
                .WithInstanceReadEndorsementForInstanceOwner(true)
                .BuildInstances();
            PermissionTestHelper.AllNull(instances.ReadClaim, instances.EndorseClaim, instances.Read, instances.CreateClaim, instances.Create, instances.Transfer, instances.AddQuantity);
            PermissionTestHelper.CheckInstance(instances.ReadEndorsement, 4, new [] {InstancePermission.InstanceOwner}, new[] {InstancePermission.Everyone, InstancePermission.TypeOwner, "123"});
        }

        [TestMethod]
        public void ShouldCreatePermissionsForTransfer()
        {
            var instances = new CreateQuantifiableAssetTypeInstancesPermissionsBuilder()
                .WithInstanceTransferForEveryone(false)
                .WithInstanceTransferForIdentity("123", true)
                .WithInstanceTransferForTypeOwner(true)
                .WithInstanceTransferForInstanceOwner(false)
                .BuildInstances();
            PermissionTestHelper.AllNull(instances.ReadClaim, instances.EndorseClaim, instances.Read, instances.CreateClaim, instances.Create, instances.ReadEndorsement, instances.AddQuantity);
            PermissionTestHelper.CheckInstance(instances.Transfer, 4, new [] {InstancePermission.TypeOwner, "123"}, new[] {InstancePermission.Everyone, InstancePermission.InstanceOwner});
        }

        [TestMethod]
        public void ShouldCreatePermissionsForAddQuantity()
        {
            var instances = new CreateQuantifiableAssetTypeInstancesPermissionsBuilder()
                .WithInstanceAddQuantityForEveryone(false)
                .WithInstanceAddQuantityForIdentity("123", true)
                .WithInstanceAddQuantityForTypeOwner(true)
                .WithInstanceAddQuantityForInstanceOwner(false)
                .BuildInstances();
            PermissionTestHelper.AllNull(instances.ReadClaim, instances.EndorseClaim, instances.Read, instances.CreateClaim, instances.Create, instances.ReadEndorsement, instances.Transfer);
            PermissionTestHelper.CheckInstance(instances.AddQuantity, 4, new [] {InstancePermission.TypeOwner, "123"}, new[] {InstancePermission.Everyone, InstancePermission.InstanceOwner});
        }

        [TestMethod]
        public void ShouldCreatePermissionsForMultipleIdentities()
        {
            var instances = new CreateQuantifiableAssetTypeInstancesPermissionsBuilder()
                .WithInstanceTransferForIdentity("123", true)
                .WithInstanceTransferForIdentity("456", false)
                .WithInstanceTransferForInstanceOwner(false)
                .BuildInstances();
            PermissionTestHelper.AllNull(instances.ReadClaim, instances.EndorseClaim, instances.Read, instances.CreateClaim, instances.Create, instances.ReadEndorsement);
            PermissionTestHelper.CheckInstance(instances.Transfer, 3, new [] {"123"}, new[] {"456", InstancePermission.InstanceOwner});
        }
    }
}