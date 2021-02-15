namespace Iov42sdk.Models.CreateAsset
{
    public class CreateQuantifiableAssetBody : CreateAssetBody
    {
        // ReSharper disable once UnusedMember.Global
        public CreateQuantifiableAssetBody()
        {
        }

        public CreateQuantifiableAssetBody(string assetId, string assetTypeId, string quantity)
            : base(assetId, assetTypeId)
        {
            Quantity = quantity;
        }

        public string Quantity { get; set; }
    }
}