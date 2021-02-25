namespace Iov42sdk.Models.AddDelegate
{
    public class AddDelegateBody : WriteBody
    {
        public AddDelegateBody() 
            : base(NodeConstants.AddDelegateRequestType)
        {
        }

        public AddDelegateBody(string delegateIdentityId, string delegatorIdentityId)
            : this()
        {
            DelegateIdentityId = delegateIdentityId;
            DelegatorIdentityId = delegatorIdentityId;
        }

        public string DelegateIdentityId { get; set; }
        public string DelegatorIdentityId { get; set; }
    }
}