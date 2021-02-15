namespace Iov42sdk.Models.CreateAssetType
{
    public class CreateUniqueAssetTypeBody : CreateAssetTypeBody
    {
        // ReSharper disable once UnusedMember.Global
        public CreateUniqueAssetTypeBody()
            : base(UniqueAssetType)
        {
        }

        public CreateUniqueAssetTypeBody(string assetTypeId) 
            : base(UniqueAssetType, assetTypeId)
        {
        }
    }
}