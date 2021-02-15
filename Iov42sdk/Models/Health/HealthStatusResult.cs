namespace Iov42sdk.Models.Health
{
    public class HealthStatusResult
    {
        public BuildInfo BuildInfo { get; set; }
        public WriteServiceStatus Broker { get; set; }
        public ReadServiceStatus RequestStore { get; set; }
        public ReadServiceStatus AssetStore { get; set; }
        public ReadServiceStatus ClaimStore { get; set; }
        public ReadServiceStatus EndorsementStore { get; set; }
        public ReadServiceStatus ProofStore { get; set; }
        public HsmStatus Hsm { get; set; }
    }
}