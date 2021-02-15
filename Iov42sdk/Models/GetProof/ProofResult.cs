namespace Iov42sdk.Models.GetProof
{
    public class ProofResult
    {
        public string RequestId { get; set; }
        public Proof Proof { get; set; }
        public ProofSignatory[] Signatories { get; set; }
        public Fingerprint[] ParentFingerprints { get; set; }
    }
}
