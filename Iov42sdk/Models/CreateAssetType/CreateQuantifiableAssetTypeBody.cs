using Iov42sdk.Models.Permissions;

namespace Iov42sdk.Models.CreateAssetType
{
    public class CreateQuantifiableAssetTypeBody : CreateAssetTypeBody
    {
        // ReSharper disable once UnusedMember.Global
        public CreateQuantifiableAssetTypeBody()
            : base(QuantifiableAssetType)
        {
        }

        public CreateQuantifiableAssetTypeBody(string assetTypeId, int scale, QuantifiableAssetTypePermissions permissions = null)
            : base(QuantifiableAssetType, assetTypeId)
        {
            Scale = scale;
            Permissions = permissions;
        }

        public int Scale { get; set; }
        public QuantifiableAssetTypePermissions Permissions { get; set; }
    }
}