namespace Iov42sdk.Support
{
    public class SerializedKeys
    {
        public string PrivateKey { get; set; }
        public string PublicKey { get; set; }

        // ReSharper disable once UnusedMember.Global
        public SerializedKeys()
        {
            // Populate with two helper methods below
        }

        public SerializedKeys(string privateKey, string publicKey)
        {
            PrivateKey = privateKey;
            PublicKey = publicKey;
        }

        public SerializedKeys WithPublicKey(string publicKey)
        {
            PublicKey = publicKey;
            return this;
        }

        public SerializedKeys WithPrivateKey(string privateKey)
        {
            PrivateKey = privateKey;
            return this;
        }
    }
}