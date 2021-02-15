namespace Iov42sdk.Models.Transfers
{
    public abstract class SingleTransfer
    {
        public string AssetTypeId { get; set; }

        // ReSharper disable once UnusedMember.Global
        protected SingleTransfer()
        {
        }

        protected SingleTransfer(string assetTypeId)
        {
            AssetTypeId = assetTypeId;
        }
    }
}