namespace Iov42sdk.Models.Transfers
{
    public class TransfersBody : WriteBody
    {
        public SingleTransfer[] Transfers { get; set; }

        public TransfersBody() 
            : base(NodeConstants.TransfersRequestType)
        {
            Transfers = new SingleTransfer[0];
        }

        public TransfersBody(params SingleTransfer[] transfers)
            : this()
        {
            Transfers = transfers;
        }
    }
}