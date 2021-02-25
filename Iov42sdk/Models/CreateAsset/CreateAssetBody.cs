namespace Iov42sdk.Models.CreateAsset
{
    public abstract class CreateAssetBody : WriteBody
    {
        protected CreateAssetBody() 
            : base(NodeConstants.CreateAssetRequestType)
        {
        }

        protected CreateAssetBody(string assetId, string assetTypeId)
            : this()
        {
            AssetId = assetId;
            AssetTypeId = assetTypeId;
        }

        public string AssetId { get; set; }
        public string AssetTypeId { get; set; }
    }
}
