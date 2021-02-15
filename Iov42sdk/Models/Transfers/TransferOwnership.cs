namespace Iov42sdk.Models.Transfers
{
    public class TransferOwnership : SingleTransfer
    {
        public string AssetId { get; set; }
        public string FromIdentityId { get; set; }
        public string ToIdentityId { get; set; }

        // ReSharper disable once UnusedMember.Global
        public TransferOwnership()
        {
        }

        public TransferOwnership(string assetId, string assetTypeId, string fromIdentityId, string toIdentityId)
            : base(assetTypeId)
        {
            AssetId = assetId;
            FromIdentityId = fromIdentityId;
            ToIdentityId = toIdentityId;
        }
    }
}
