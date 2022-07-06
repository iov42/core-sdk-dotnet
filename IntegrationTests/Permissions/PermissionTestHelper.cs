using Iov42sdk.Models.Permissions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests.Permissions
{
    public class PermissionTestHelper
    {
        public static void AllNull(params object[] items)
        {
            foreach (var item in items) 
                Assert.IsNull(item);
        }

        public static void CheckInstance(InstancePermission permission, int expectedLength, string[] granted, string[] denied)
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