namespace BouncyCastleCrypto
{
    public interface ICryptoEngine
    {
        BouncyKeyPair GenerateKeyPair();
        string ProtocolId { get; }
    }
}