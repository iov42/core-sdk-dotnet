namespace Iov42sdk.Models.CreateClaims
{
    public class CreateClaimsResult
    {
        public string RequestId { get; set; }
        public string Proof { get; set; }
        public string[] Resources { get; set; }
        public ErrorResult[] Errors { get; set; }
    }
}