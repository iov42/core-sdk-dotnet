namespace Iov42sdk.Models.Health
{
    public class HealthStatusResult
    {
        public BuildInfo BuildInfo { get; set; }
        public BrokerHealthStatus Broker { get; set; }
        public StoreHealthStatus AssetStore { get; set; }
        public StoreHealthStatus ClaimStore { get; set; }
        public StoreHealthStatus EndorsementStore { get; set; }
        public StoreHealthStatus PermissionStore { get; set; }
        public StoreHealthStatus TransactionStore { get; set; }
        public StoreHealthStatus TransferStore { get; set; }
        public StoreHealthStatus TransferByIdentityStore { get; set; }
        public HsmHealthStatus Hsm { get; set; }
    }
}