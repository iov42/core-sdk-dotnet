namespace Iov42sdk.Models.Transactions
{
    public class UniqueTransaction : Transaction
    {
        public UniqueTransaction(CombinedTransaction transaction)
            : base(transaction)
        {
            FromOwner = transaction.FromOwner;
            ToOwner = transaction.ToOwner;
            Asset = transaction.Asset;
        }

        public string FromOwner { get; set; }
        public string ToOwner { get; set; }
        public Participant Asset { get; set; }
    }
}