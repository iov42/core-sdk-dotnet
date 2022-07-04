using System;
using BouncyCastleCrypto;
using Iov42sdk.Connection;
using Iov42sdk.Crypto;
using Iov42sdk.Identity;
using Iov42sdk.Models;
using Iov42sdk.Support;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests.Support
{
    public class IntegrationTestCreation : IDisposable
    {
        public IntegrationTestCreation(Func<ICryptoEngine> engine = null)
        {
            IdentityBuilder = new IdentityBuilder(k => CreateCrypto(k, engine));
            Identity = IdentityBuilder.Create();
            Client = ClientBuilder.CreateWithNewIdentity(TestEnvironment.DefaultClientSettings, Identity).Result;
        }

        internal static BouncyCrypto CreateCrypto(IKeyPair k, Func<ICryptoEngine> engine = null)
        {
            return new BouncyCrypto(engine != null ? engine() : new EcsdaCryptoEngine(), k as BouncyKeyPair);
        }

        public IPlatformClient Client { get; }
        public IdentityBuilder IdentityBuilder { get; }
        public IdentityDetails Identity { get; }

        public void Dispose()
        {
            Client?.Dispose();
        }

        internal string CreateUniqueId(string root = null)
        {
            var id = Guid.NewGuid().ToString();
            return root != null ? $"{root}-{id}" : id;
        }
    }

    public static class WriteResultExtension
    {
        public static void VerifyWriteResult(this ResponseResult<WriteResult> result, int resources = 1)
        {
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Value.RequestId);
            Assert.IsNotNull(result.Value.Proof);
            Assert.IsNotNull(result.Value.Resources);
            Assert.IsTrue(result.Value.Resources.Length >= resources);
            Assert.IsNull(result.Value.Errors);
        }
    }
}