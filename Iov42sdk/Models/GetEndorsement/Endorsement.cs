namespace Iov42sdk.Models.GetEndorsement
{
    public class Endorsement
    {
        public string Claim { get; }
        public string ClaimHash { get; }
        public string EndorsementString { get; }

        public Endorsement(string claim, string claimHash, string endorsementString)
        {
            Claim = claim;
            ClaimHash = claimHash;
            EndorsementString = endorsementString;
        }
    }
}