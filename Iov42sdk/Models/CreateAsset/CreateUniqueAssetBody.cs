namespace Iov42sdk.Models.CreateAsset
{
    public class CreateUniqueAssetBody : CreateAssetBody
    {
        // ReSharper disable once UnusedMember.Global
        public CreateUniqueAssetBody()
        {
        }

        public CreateUniqueAssetBody(string assetId, string assetTypeId)
            : base(assetId, assetTypeId)
        {
        }
    }
}