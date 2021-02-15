using BouncyCastleCrypto;
using Iov42sdk.Crypto;
using Iov42sdk.Support;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests.Crypto
{
    public abstract class TestCrypto
    {
        public abstract ICrypto GetCrypto(BouncyKeyPair pair = null);

        [TestMethod]
        public void ShouldSignData()
        {
            var crypto = GetCrypto();
            var original = UsefulConversions.ToBytes("Some test data");
            var signed = UsefulConversions.ToBase64Url(crypto.Sign(original));
            var result = crypto.VerifySignature(crypto.Pair, signed, original);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ShouldNotVerifyIncorrectSignedData()
        {
            var crypto = GetCrypto();
            var original = UsefulConversions.ToBytes("Some test data");
            var signed = UsefulConversions.ToBase64Url(crypto.Sign(original));
            var result = crypto.VerifySignature(crypto.Pair, signed, UsefulConversions.ToBytes("some other data"));
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ShouldNotVerifyIncorrectSignedDataWithDifferentPair()
        {
            var crypto = GetCrypto();
            var original = UsefulConversions.ToBytes("Some test data");
            var signed = UsefulConversions.ToBase64Url(crypto.Sign(original));
            var otherCrypto = GetCrypto();
            var result = otherCrypto.VerifySignature(otherCrypto.Pair, signed, original);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ShouldVerifyFromSerializedKeys()
        {
            var crypto = GetCrypto();
            var original = UsefulConversions.ToBytes("Some test data");
            var signed = UsefulConversions.ToBase64Url(crypto.Sign(original));
            var serialized = crypto.Pair.Serialize();
            var pair = new BouncyKeyPair(serialized);
            Assert.IsNotNull(pair);
            var otherCrypto = GetCrypto(pair);
            var result = otherCrypto.VerifySignature(otherCrypto.Pair, signed, original);
            Assert.IsTrue(result);

            var newSigned = UsefulConversions.ToBase64Url(otherCrypto.Sign(original));
            result = crypto.VerifySignature(crypto.Pair, newSigned, original);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ShouldVerifyFromRebuiltSerializedKeys()
        {
            var crypto = GetCrypto();
            var original = UsefulConversions.ToBytes("Some test data");
            var signed = UsefulConversions.ToBase64Url(crypto.Sign(original));
            var serialized = crypto.Pair.Serialize();
            var rebuilt = crypto.FromSerialized(serialized) as BouncyKeyPair;
            Assert.IsNotNull(rebuilt);
            var otherCrypto = GetCrypto(rebuilt);
            var result = otherCrypto.VerifySignature(otherCrypto.Pair, signed, original);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ShouldHandlePem()
        {
            var crypto = GetCrypto();
            var pair = crypto.Pair;
            var text = pair.ToPem();
            Assert.IsTrue(!string.IsNullOrEmpty(text));
            var newPair = new BouncyKeyPair(text);
            Assert.AreEqual(pair.PrivateKeyBase64String, newPair.PrivateKeyBase64String);
            Assert.AreEqual(pair.PublicKeyBase64String, newPair.PublicKeyBase64String);
        }
    }
}