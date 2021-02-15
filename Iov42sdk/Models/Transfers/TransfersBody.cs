namespace Iov42sdk.Models.Transfers
{
    public class TransfersBody : PutBody
    {
        public SingleTransfer[] Transfers { get; set; }

        public TransfersBody() 
            : base(NodeConstants.TransfersRequestType)
        {
            Transfers = new SingleTransfer[0];
        }

        public TransfersBody(SingleTransfer[] transfers)
            : this()
        {
            Transfers = transfers;
        }
    }
}