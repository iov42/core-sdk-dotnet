namespace Iov42sdk.Models.Permissions
{
    public class QuantifiableAssetTypeInstancesPermissions
    {
        public InstancePermission Create { get; set; }
        public InstancePermission Read { get; set; }
        public InstancePermission CreateClaim { get; set; }
        public InstancePermission EndorseClaim { get; set; }
        public InstancePermission ReadClaim { get; set; }
        public InstancePermission ReadEndorsement { get; set; }
        public InstancePermission Transfer { get; set; }
        public InstancePermission AddQuantity { get; set; }
    }
}