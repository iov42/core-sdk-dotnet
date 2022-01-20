namespace Iov42sdk.Models.Transactions
{
    public class Transaction
    {
        internal Transaction(CombinedTransaction transaction)
        {
            RequestId = transaction.RequestId;
            TransactionTimestamp = transaction.TransactionTimestamp;
            Proof = transaction.Proof;
        }

        public string RequestId { get; set; }
        public string TransactionTimestamp { get; set; }
        public string Proof { get; set; }
    }
}