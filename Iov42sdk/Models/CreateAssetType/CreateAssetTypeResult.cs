namespace Iov42sdk.Models.CreateAssetType
{
    public class CreateAssetTypeResult
    {
        public string RequestId { get; set; }
        public string[] Resources { get; set; }
        public string Proof { get; set; }
        public ErrorResult[] Errors { get; set; }
    }
}