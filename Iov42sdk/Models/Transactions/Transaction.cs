namespace Iov42sdk.Models.Transactions
{
    public class Transaction
    {
        public string RequestId { get; set; }
        public Participant Sender { get; set; }
        public Participant Recipient { get; set; }
        public string Quantity { get; set; }
        public string TransactionTimestamp { get; set; }
        public string Proof { get; set; }
    }
}