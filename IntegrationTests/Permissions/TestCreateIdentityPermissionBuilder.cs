using Iov42sdk.Models.Permissions;
using Iov42sdk.Support.Permissions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests.Permissions
{
    [TestClass]
    public class TestCreateIdentityPermissionBuilder
    {
        [TestMethod]
        public void ShouldCreateAllNullFields()
        {
            var permissions = new PermissionBuilder()
                .CreateIdentityPermissions()
                .Build();
            PermissionTestHelper.AllNull(permissions.CreateClaim, permissions.EndorseClaim, permissions.ReadEndorsement, permissions.ReadClaim, permissions.CreateIdentity);
        }

        [TestMethod]
        public void ShouldCreatePermissionsForCreateClaim()
        {
            var permissions = new CreateIdentityPermissionBuilder()
                .WithCreateClaimForEveryone(false)
                .WithCreateClaimForIdentity("123", true)
                .Build();
            PermissionTestHelper.AllNull(permissions.EndorseClaim, permissions.ReadEndorsement, permissions.ReadClaim, permissions.CreateIdentity);
            PermissionTestHelper.CheckInstance(permissions.CreateClaim, 2, new[] { "123" }, new[] { InstancePermission.Everyone });
        }

        [TestMethod]
        public void ShouldCreatePermissionsForEndorseClaim()
        {
            var permissions = new CreateIdentityPermissionBuilder()
                .WithEndorseClaimForEveryone(true)
                .WithEndorseClaimForIdentity("123", false)
                .Build();
            PermissionTestHelper.AllNull(permissions.CreateClaim, permissions.ReadEndorsement, permissions.ReadClaim, permissions.CreateIdentity);
            PermissionTestHelper.CheckInstance(permissions.EndorseClaim, 2, new[] { InstancePermission.Everyone }, new[] { "123" });
        }

        [TestMethod]
        public void ShouldCreatePermissionsForReadClaim()
        {
            var permissions = new CreateIdentityPermissionBuilder()
                .WithReadClaimForEveryone(true)
                .WithReadClaimForIdentity("123", false)
                .Build();
            PermissionTestHelper.AllNull(permissions.CreateClaim, permissions.ReadEndorsement, permissions.EndorseClaim, permissions.CreateIdentity);
            PermissionTestHelper.CheckInstance(permissions.ReadClaim, 2, new[] { InstancePermission.Everyone }, new[] { "123" });
        }

        [TestMethod]
        public void ShouldCreatePermissionsForReadEndorsement()
        {
            var permissions = new CreateIdentityPermissionBuilder()
                .WithReadEndorsementForEveryone(false)
                .WithReadEndorsementForIdentity("123", true)
                .Build();
            PermissionTestHelper.AllNull(permissions.CreateClaim, permissions.ReadClaim, permissions.EndorseClaim, permissions.CreateIdentity);
            PermissionTestHelper.CheckInstance(permissions.ReadEndorsement, 2, new[] { "123" }, new[] { InstancePermission.Everyone });
        }

        [TestMethod]
        public void ShouldCreatePermissionsForCreateIdentityGrant()
        {
            var permissions = new CreateIdentityPermissionBuilder()
                .WithCreateIdentity(true)
                .Build();
            PermissionTestHelper.AllNull(permissions.CreateClaim, permissions.ReadClaim, permissions.EndorseClaim, permissions.ReadEndorsement);
            Assert.AreEqual(GrantOrDeny.Grant, permissions.CreateIdentity);
        }

        [TestMethod]
        public void ShouldCreatePermissionsForCreateIdentityDeny()
        {
            var permissions = new CreateIdentityPermissionBuilder()
                .WithCreateIdentity(false)
                .Build();
            PermissionTestHelper.AllNull(permissions.CreateClaim, permissions.ReadClaim, permissions.EndorseClaim, permissions.ReadEndorsement);
            Assert.AreEqual(GrantOrDeny.Deny, permissions.CreateIdentity);
        }

        [TestMethod]
        public void ShouldCreatePermissionsForMultipleIdentities()
        {
            var permissions = new CreateIdentityPermissionBuilder()
                .WithReadEndorsementForEveryone(false)
                .WithReadEndorsementForIdentity("123", true)
                .WithReadEndorsementForIdentity("456", false)
                .Build();
            PermissionTestHelper.AllNull(permissions.CreateClaim, permissions.ReadClaim, permissions.EndorseClaim, permissions.CreateIdentity);
            PermissionTestHelper.CheckInstance(permissions.ReadEndorsement, 3, new[] { "123" }, new[] { InstancePermission.Everyone, "456" });
        }

    }
}
