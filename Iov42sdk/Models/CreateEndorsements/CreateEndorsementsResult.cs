namespace Iov42sdk.Models.CreateEndorsements
{
    public class CreateEndorsementsResult
    {
        public string RequestId { get; set; }
        public bool RequestIdReusable { get; set; }
        public string Proof { get; set; }
        public string[] Resources { get; set; }
        public ErrorResult[] Errors { get; set; }
    }
}
