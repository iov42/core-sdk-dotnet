using BouncyCastleCrypto;
using Iov42sdk.Crypto;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests.Crypto
{
    [TestClass]
    public class TestEcdsaCryptoEngine : TestCrypto
    {
        public override ICrypto GetCrypto(BouncyKeyPair pair = null)
        {
            return new BouncyCrypto(new EcsdaCryptoEngine(), pair);
        }
    }
}