namespace Iov42sdk.Models.UpdateBalance
{
    public class UpdateBalanceBody : WriteBody
    {
        public UpdateBalanceBody() 
            : base(NodeConstants.AddQuantityRequestType)
        {
        }

        public UpdateBalanceBody(string assetId, string assetTypeId, string quantity)
            : this()
        {
            Quantity = quantity;
            AssetId = assetId;
            AssetTypeId = assetTypeId;
        }

        public string AssetId { get; set; }
        public string AssetTypeId { get; set; }
        public string Quantity { get; set; }
    }
}