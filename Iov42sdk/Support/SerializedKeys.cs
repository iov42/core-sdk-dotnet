namespace Iov42sdk.Support
{
    public class SerializedKeys
    {
        public string PrivateKey { get; set; }
        public string PublicKey { get; set; }

        public SerializedKeys(string privateKey, string publicKey)
        {
            PrivateKey = privateKey;
            PublicKey = publicKey;
        }
    }
}