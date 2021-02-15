using System.IO;
using Iov42sdk.Crypto;
using Iov42sdk.Support;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace BouncyCastleCrypto
{
    public class BouncyKeyPair : IKeyPair
    {
        private AsymmetricKeyParameter _publicKey;
        private SubjectPublicKeyInfo _publicKeyInfo;
        private AsymmetricKeyParameter _privateKey;
        private PrivateKeyInfo _privateKeyInfo;

        internal BouncyKeyPair(AsymmetricCipherKeyPair pair)
        {
            PrivateKey = pair.Private;
            PublicKey = pair.Public;
        }

        public BouncyKeyPair(SerializedKeys keys)
        {
            if (!string.IsNullOrEmpty(keys.PrivateKey))
                PrivateKey = PrivateKeyFactory.CreateKey(UsefulConversions.FromBase64UrlToBytes(keys.PrivateKey));
            if (!string.IsNullOrEmpty(keys.PublicKey))
                PublicKey = PublicKeyFactory.CreateKey(UsefulConversions.FromBase64UrlToBytes(keys.PublicKey));
        }

        public BouncyKeyPair(string pemPrivateKey)
        {
            using (var sr = new StringReader(pemPrivateKey))
            {
                var pr = new PemReader(sr);
                var keyPair = (AsymmetricCipherKeyPair)pr.ReadObject();
                PrivateKey = keyPair.Private;
                PublicKey = keyPair.Public;
            }
        }

        internal AsymmetricKeyParameter PublicKey
        {
            get => _publicKey;
            private set
            {
                _publicKey = value;
                _publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(_publicKey);
            }
        }

        internal AsymmetricKeyParameter PrivateKey
        {
            get => _privateKey;
            private set
            {
                _privateKey = value;
                _privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(PrivateKey);
            }
        }

        public string PrivateKeyBase64String
        {
            get
            {
                var serializedPrivateBytes = _privateKeyInfo.GetDerEncoded();
                return UsefulConversions.ToBase64Url(serializedPrivateBytes);
            }
        }

        public string PublicKeyBase64String
        {
            get
            {
                var serializedPrivateBytes = _publicKeyInfo.GetDerEncoded();
                return UsefulConversions.ToBase64Url(serializedPrivateBytes);
            }
        }

        public SerializedKeys Serialize()
        {
            var serializedPrivate = UsefulConversions.ToBase64Url(_privateKeyInfo.ToAsn1Object().GetDerEncoded());
            var serializedPublic = UsefulConversions.ToBase64Url(_publicKeyInfo.ToAsn1Object().GetDerEncoded());
            return new SerializedKeys(serializedPrivate, serializedPublic);
        }

        public string ToPem()
        {
            using (var sw = new StringWriter())
            {
                var pw = new PemWriter(sw);
                pw.WriteObject(PrivateKey);
                pw.Writer.Flush();
                return sw.ToString();
            }
        }
    }
}