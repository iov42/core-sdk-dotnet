using Iov42sdk.Crypto;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Security;

namespace BouncyCastleCrypto
{
    public class EcsdaCryptoEngine : ICryptoEngine
    {
        private const int EcdsaKeySize = 256;
        
        public BouncyKeyPair GenerateKeyPair()
        {
            var secureRandom = new SecureRandom();
            var generator = new ECKeyPairGenerator("ECDSA");
            var keyGenerationParameters = new KeyGenerationParameters(secureRandom, EcdsaKeySize);
            generator.Init(keyGenerationParameters);
            var pair = generator.GenerateKeyPair();
            return new BouncyKeyPair(pair);
        }

        public string ProtocolId => SupportedProtocols.Sha256WithEcdsa;
    }
}