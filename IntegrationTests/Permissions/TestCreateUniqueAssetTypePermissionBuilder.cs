using System;
using Iov42sdk.Models.Permissions;
using Iov42sdk.Support.Permissions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests.Permissions
{
    [TestClass]
    public class TestCreateUniqueAssetTypePermissionBuilder
    {
        [TestMethod]
        public void ShouldCreateAllNullFields()
        {
            var permissions = new PermissionBuilder()
                .CreateUniqueAssetTypePermissions()
                .Build();
            PermissionTestHelper.AllNull(permissions.CreateClaim, permissions.EndorseClaim, permissions.ReadEndorsement, permissions.ReadClaim, permissions.Read, permissions.Instances);
        }

        [TestMethod]
        public void ShouldCreatePermissionsForCreateClaim()
        {
            var permissions = new CreateUniqueAssetTypePermissionBuilder()
                .WithCreateClaimForEveryone(false)
                .WithCreateClaimForIdentity("123", true)
                .WithCreateClaimForInstanceOwner(true)
                .Build();
            PermissionTestHelper.AllNull(permissions.EndorseClaim, permissions.ReadEndorsement, permissions.ReadClaim, permissions.Read, permissions.Instances);
            PermissionTestHelper.CheckInstance(permissions.CreateClaim, 3, new[] { "123", InstancePermission.InstanceOwner }, new[] { InstancePermission.Everyone });
        }

                [TestMethod]
        public void ShouldCreatePermissionsForEndorseClaim()
        {
            var permissions = new CreateUniqueAssetTypePermissionBuilder()
                .WithEndorseClaimForEveryone(true)
                .WithEndorseClaimForIdentity("123", true)
                .WithEndorseClaimForInstanceOwner(false)
                .Build();
            PermissionTestHelper.AllNull(permissions.CreateClaim, permissions.ReadEndorsement, permissions.ReadClaim, permissions.Read, permissions.Instances);
            PermissionTestHelper.CheckInstance(permissions.EndorseClaim, 3, new[] { "123", InstancePermission.Everyone}, new[] { InstancePermission.InstanceOwner });
        }

        [TestMethod]
        public void ShouldCreatePermissionsForReadEndorsement()
        {
            var permissions = new CreateUniqueAssetTypePermissionBuilder()
                .WithReadEndorsementForEveryone(true)
                .WithReadEndorsementForIdentity("123", false)
                .WithReadEndorsementForInstanceOwner(true)
                .Build();
            PermissionTestHelper.AllNull(permissions.CreateClaim, permissions.EndorseClaim, permissions.ReadClaim, permissions.Read, permissions.Instances);
            PermissionTestHelper.CheckInstance(permissions.ReadEndorsement, 3, new[] { InstancePermission.Everyone, InstancePermission.InstanceOwner }, new[] { "123" });
        }

        [TestMethod]
        public void ShouldCreatePermissionsForReadClaim()
        {
            var permissions = new CreateUniqueAssetTypePermissionBuilder()
                .WithReadClaimForEveryone(false)
                .WithReadClaimForIdentity("123", false)
                .WithReadClaimForInstanceOwner(true)
                .Build();
            PermissionTestHelper.AllNull(permissions.CreateClaim, permissions.EndorseClaim, permissions.ReadEndorsement, permissions.Read, permissions.Instances);
            PermissionTestHelper.CheckInstance(permissions.ReadClaim, 3, new[] { InstancePermission.InstanceOwner }, new[] { InstancePermission.Everyone, "123" });
        }

        [TestMethod]
        public void ShouldCreatePermissionsForRead()
        {
            var permissions = new CreateUniqueAssetTypePermissionBuilder()
                .WithReadForEveryone(false)
                .WithReadForIdentity("123", true)
                .WithReadForTypeOwner(true)
                .Build();
            PermissionTestHelper.AllNull(permissions.CreateClaim, permissions.EndorseClaim, permissions.ReadEndorsement, permissions.ReadClaim, permissions.Instances);
            PermissionTestHelper.CheckInstance(permissions.Read, 3, new[] { InstancePermission.TypeOwner, "123" }, new[] { InstancePermission.Everyone });
        }

        [TestMethod]
        public void ShouldCreatePermissionsForInstances()
        {
            var permissions = new CreateUniqueAssetTypePermissionBuilder()
                .WithInstancePermission(new CreateUniqueAssetTypeInstancesPermissionsBuilder()
                    .WithInstanceCreateClaimForInstanceOwner(true)
                    .WithInstanceReadEndorsementForIdentity("123", true)
                    .WithInstanceReadEndorsementForTypeOwner(false)
                    .BuildInstances())
                .Build();
            PermissionTestHelper.AllNull(permissions.CreateClaim, permissions.EndorseClaim, permissions.ReadEndorsement, permissions.ReadClaim, permissions.Read);
            PermissionTestHelper.CheckInstance(permissions.Instances.ReadEndorsement, 2, new[] { "123" }, new[] { InstancePermission.TypeOwner });
            PermissionTestHelper.CheckInstance(permissions.Instances.CreateClaim, 1, new[] { InstancePermission.InstanceOwner }, Array.Empty<string>());
        }
    }
}