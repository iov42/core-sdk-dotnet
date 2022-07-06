namespace Iov42sdk.Models.Permissions
{
    public class QuantifiableAssetTypePermissions
    {
        public InstancePermission Read { get; set; }
        public InstancePermission CreateClaim { get; set; }
        public InstancePermission EndorseClaim { get; set; }
        public InstancePermission ReadClaim { get; set; }
        public InstancePermission ReadEndorsement { get; set; }
        public QuantifiableAssetTypeInstancesPermissions Instances { get; set; }
    }
}