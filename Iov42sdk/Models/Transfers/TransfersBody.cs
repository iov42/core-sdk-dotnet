using System;

namespace Iov42sdk.Models.Transfers
{
    public class TransfersBody : WriteBody
    {
        public SingleTransfer[] Transfers { get; set; }

        public TransfersBody() 
            : base(NodeConstants.TransfersRequestType)
        {
            Transfers = Array.Empty<SingleTransfer>();
        }

        public TransfersBody(params SingleTransfer[] transfers)
            : this()
        {
            Transfers = transfers;
        }
    }
}