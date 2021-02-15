using Iov42sdk.Crypto;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Security;

namespace BouncyCastleCrypto
{
    public class RsaCryptoEngine : ICryptoEngine
    {
        private const int RsaKeySize = 2048;

        public string ProtocolId => SupportedProtocols.Sha256WithRsa;

        public BouncyKeyPair GenerateKeyPair()
        {
            var secureRandom = new SecureRandom();
            var keyGenerationParameters = new KeyGenerationParameters(secureRandom, RsaKeySize);
            var keyPairGenerator = new RsaKeyPairGenerator();
            keyPairGenerator.Init(keyGenerationParameters);
            var pair = keyPairGenerator.GenerateKeyPair();
            return new BouncyKeyPair(pair);
        }
    }
}