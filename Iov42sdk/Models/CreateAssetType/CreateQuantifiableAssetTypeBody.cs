namespace Iov42sdk.Models.CreateAssetType
{
    public class CreateQuantifiableAssetTypeBody : CreateAssetTypeBody
    {
        // ReSharper disable once UnusedMember.Global
        public CreateQuantifiableAssetTypeBody()
            : base(QuantifiableAssetType)
        {
        }

        public CreateQuantifiableAssetTypeBody(string assetTypeId, int scale)
            : base(QuantifiableAssetType, assetTypeId)
        {
            Scale = scale;
        }

        public int Scale { get; set; }
    }
}