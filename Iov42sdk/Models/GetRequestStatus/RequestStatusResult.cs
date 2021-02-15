namespace Iov42sdk.Models.GetRequestStatus
{
    public class RequestStatusResult
    {
        public string RequestId { get; set; }
        public string[] Resources { get; set; }
        public string Proof { get; set; }
    }
}
