using Iov42sdk.Models.Permissions;

namespace Iov42sdk.Models.CreateAssetType
{
    public class CreateUniqueAssetTypeBody : CreateAssetTypeBody
    {
        public UniqueAssetTypePermissions Permissions { get; set; }

        // ReSharper disable once UnusedMember.Global
        public CreateUniqueAssetTypeBody()
            : base(UniqueAssetType)
        {
        }

        public CreateUniqueAssetTypeBody(string assetTypeId, UniqueAssetTypePermissions permissions = null) 
            : base(UniqueAssetType, assetTypeId)
        {
            Permissions = permissions;
        }
    }
}