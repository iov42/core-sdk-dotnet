namespace Iov42sdk.Models.CreateAssetType
{
    public abstract class CreateAssetTypeBody : WriteBody
    {
        protected static readonly string UniqueAssetType = "Unique";
        protected static readonly string QuantifiableAssetType = "Quantifiable";

        protected CreateAssetTypeBody(string type, string assetTypeId = null)
            : base(NodeConstants.DefineAssetTypeRequestType)
        {
            Type = type;
            AssetTypeId = assetTypeId;
        }

        public string AssetTypeId { get; set; }
        public string Type { get; set; }
    }
}