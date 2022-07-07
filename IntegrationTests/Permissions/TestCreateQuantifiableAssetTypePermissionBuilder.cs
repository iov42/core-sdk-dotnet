using System;
using IntegrationTests.Support;
using Iov42sdk.Models.Permissions;
using Iov42sdk.Support.Permissions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests.Permissions
{
    [TestClass]
    public class TestCreateQuantifiableAssetTypePermissionBuilder
    {
        [TestMethod]
        public void ShouldCreateAllNullFields()
        {
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .Build();
            TestHelper.AllNull(permissions.CreateClaim, permissions.EndorseClaim, permissions.ReadEndorsement, permissions.ReadClaim, permissions.Read, permissions.Instances);
        }

        [TestMethod]
        public void ShouldCreatePermissionsForCreateClaim()
        {
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .WithCreateClaimForEveryone(false)
                .WithCreateClaimForIdentity("123", true)
                .WithCreateClaimForInstanceOwner(true)
                .Build();
            TestHelper.AllNull(permissions.EndorseClaim, permissions.ReadEndorsement, permissions.ReadClaim, permissions.Read, permissions.Instances);
            PermissionTestHelper.CheckInstance(permissions.CreateClaim, 3, new[] { InstancePermission.Identity("123"), InstancePermission.InstanceOwner }, new[] { InstancePermission.Everyone });
        }

        [TestMethod]
        public void ShouldCreatePermissionsForEndorseClaim()
        {
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .WithEndorseClaimForEveryone(true)
                .WithEndorseClaimForIdentity("123", true)
                .WithEndorseClaimForInstanceOwner(false)
                .Build();
            TestHelper.AllNull(permissions.CreateClaim, permissions.ReadEndorsement, permissions.ReadClaim, permissions.Read, permissions.Instances);
            PermissionTestHelper.CheckInstance(permissions.EndorseClaim, 3, new[] { InstancePermission.Identity("123"), InstancePermission.Everyone}, new[] { InstancePermission.InstanceOwner });
        }

        [TestMethod]
        public void ShouldCreatePermissionsForReadEndorsement()
        {
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .WithReadEndorsementForEveryone(true)
                .WithReadEndorsementForIdentity("123", false)
                .WithReadEndorsementForInstanceOwner(true)
                .Build();
            TestHelper.AllNull(permissions.CreateClaim, permissions.EndorseClaim, permissions.ReadClaim, permissions.Read, permissions.Instances);
            PermissionTestHelper.CheckInstance(permissions.ReadEndorsement, 3, new[] { InstancePermission.Everyone, InstancePermission.InstanceOwner }, new[] { InstancePermission.Identity("123") });
        }

        [TestMethod]
        public void ShouldCreatePermissionsForReadClaim()
        {
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .WithReadClaimForEveryone(false)
                .WithReadClaimForIdentity("123", false)
                .WithReadClaimForInstanceOwner(true)
                .Build();
            TestHelper.AllNull(permissions.CreateClaim, permissions.EndorseClaim, permissions.ReadEndorsement, permissions.Read, permissions.Instances);
            PermissionTestHelper.CheckInstance(permissions.ReadClaim, 3, new[] { InstancePermission.InstanceOwner }, new[] { InstancePermission.Everyone, InstancePermission.Identity("123") });
        }

        [TestMethod]
        public void ShouldCreatePermissionsForRead()
        {
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .WithReadForEveryone(false)
                .WithReadForIdentity("123", true)
                .WithReadForTypeOwner(true)
                .Build();
            TestHelper.AllNull(permissions.CreateClaim, permissions.EndorseClaim, permissions.ReadEndorsement, permissions.ReadClaim, permissions.Instances);
            PermissionTestHelper.CheckInstance(permissions.Read, 3, new[] { InstancePermission.TypeOwner, InstancePermission.Identity("123") }, new[] { InstancePermission.Everyone });
        }

        [TestMethod]
        public void ShouldCreatePermissionsForInstances()
        {
            var permissions = new CreateQuantifiableAssetTypePermissionBuilder()
                .WithInstancePermission(new CreateQuantifiableAssetTypeInstancesPermissionsBuilder()
                    .WithInstanceAddQuantityForEveryone(true)
                    .WithInstanceAddQuantityForIdentity("123", false)
                    .WithInstanceCreateClaimForInstanceOwner(true)
                    .BuildInstances())
                .Build();
            TestHelper.AllNull(permissions.CreateClaim, permissions.EndorseClaim, permissions.ReadEndorsement, permissions.ReadClaim, permissions.Read);
            PermissionTestHelper.CheckInstance(permissions.Instances.AddQuantity, 2, new[] { InstancePermission.Everyone }, new[] { InstancePermission.Identity("123") });
            PermissionTestHelper.CheckInstance(permissions.Instances.CreateClaim, 1, new[] { InstancePermission.InstanceOwner }, Array.Empty<string>());
        }
    }
}