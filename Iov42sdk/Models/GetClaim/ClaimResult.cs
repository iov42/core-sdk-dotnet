using Iov42sdk.Models.GetEndorsement;

namespace Iov42sdk.Models.GetClaim
{
    public class ClaimResult
    {
        public string Proof { get; set; }
        public string Claim { get; set; }
        public string DelegateIdentityId { get; set; }
        public EndorsementResult[] Endorsements { get; set; }
    }
}
