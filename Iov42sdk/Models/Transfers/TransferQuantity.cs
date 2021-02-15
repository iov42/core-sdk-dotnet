namespace Iov42sdk.Models.Transfers
{
    public class TransferQuantity : SingleTransfer
    {
        public string FromAssetId { get; set; }
        public string ToAssetId { get; set; }
        public string Quantity { get; set; }

        // ReSharper disable once UnusedMember.Global
        public TransferQuantity()
        {
        }

        public TransferQuantity(string fromAssetId, string toAssetId, string assetTypeId, string quantity)
            : base(assetTypeId)
        {
            FromAssetId = fromAssetId;
            ToAssetId = toAssetId;
            Quantity = quantity;
        }
    }
}