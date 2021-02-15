namespace Iov42sdk.Models.GetProof
{
    public class ProofNode
    {
        public ProofNodeId Id { get; set; }
        public string Payload { get; set; }
        public ProofNodeLink[] Links { get; set; }
    }
}