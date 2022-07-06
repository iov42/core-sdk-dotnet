namespace Iov42sdk.Support.Permissions
{
    public class PermissionBuilder
    {
        public CreateIdentityPermissionBuilder CreateIdentityPermissions()
        {
            return new CreateIdentityPermissionBuilder();
        }

        public CreateUniqueAssetTypePermissionBuilder CreateUniqueAssetTypePermissions()
        {
            return new CreateUniqueAssetTypePermissionBuilder();
        }

        public CreateQuantifiableAssetTypePermissionBuilder CreateQuantifiableAssetTypePermissions()
        {
            return new CreateQuantifiableAssetTypePermissionBuilder();
        }
    }
}
