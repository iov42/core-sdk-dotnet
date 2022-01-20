namespace Iov42sdk.Models.Transactions
{
    public class QuantifiableTransaction : Transaction
    {
        public QuantifiableTransaction(CombinedTransaction transaction) 
            : base(transaction)
        {
            Sender = transaction.Sender;
            Recipient = transaction.Recipient;
            Quantity = transaction.Quantity;
        }

        public Participant Sender { get; set; }
        public Participant Recipient { get; set; }
        public string Quantity { get; set; }
    }
}