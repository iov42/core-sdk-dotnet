using System.Security.Cryptography;
using Iov42sdk.Crypto;
using Iov42sdk.Support;
using Org.BouncyCastle.Security;

namespace BouncyCastleCrypto
{
    public class BouncyCrypto : ICrypto
    {
        private readonly BouncyKeyPair _pair;
        private readonly ICryptoEngine _engine;

        public BouncyCrypto(ICryptoEngine engine, BouncyKeyPair pair = null)
        {
            _engine = engine;
            _pair = pair ?? _engine.GenerateKeyPair();
        }

        public IKeyPair Pair => _pair;

        public string ProtocolId => _engine.ProtocolId;

        public byte[] Sign(byte[] data)
        {
            var signer = SignerUtilities.GetSigner(ProtocolId);
            signer.Init(true, _pair.PrivateKey);
            signer.BlockUpdate(data, 0, data.Length);
            return signer.GenerateSignature();
        }

        public bool VerifySignature(IKeyPair pair, string base64Signature, byte[] original)
        {
            try
            {
                var sigBytes = UsefulConversions.FromBase64UrlToBytes(base64Signature);
                var signer = SignerUtilities.GetSigner(ProtocolId);
                signer.Init(false, (pair as BouncyKeyPair)?.PublicKey);
                signer.BlockUpdate(original, 0, original.Length);
                return signer.VerifySignature(sigBytes);
            }
            catch
            {
                return false;
            }
        }

        public string GetHash(string value)
        {
            using (var hash = SHA256.Create())
            {
                var hashBytes = hash.ComputeHash(UsefulConversions.ToBytes(value));
                return UsefulConversions.ToBase64Url(hashBytes);
            }
        }

        public IKeyPair FromSerialized(SerializedKeys keys)
        {
            return new BouncyKeyPair(keys);
        }
    }
}
