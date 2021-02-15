namespace Iov42sdk.Models.IssueIdentity
{
    public class CreateIdentityResult
    {
        public string RequestId { get; set; }
        public bool RequestIdReusable { get; set; }
        public string Proof { get; set; }
        public ErrorResult[] Errors { get; set; }
    }
}
