using System.Collections.Generic;
using System.Threading.Tasks;
using IntegrationTests.Support;
using Iov42sdk.Connection;
using Iov42sdk.Models.Transactions;
using Iov42sdk.Support;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests
{
    [TestClass]
    public class TestTransactions
    {
        private static IntegrationTestCreation _test;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            _test = new IntegrationTestCreation();
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            _test?.Dispose();
        }

        [TestMethod]
        public async Task ShouldFetchQuantifiableCreditTransactions()
        {
            var gbpId = _test.CreateUniqueId("GBP");
            var writeResult = await _test.Client.CreateQuantifiableAssetType(gbpId, 2);
            Assert.IsTrue(writeResult.Success);
            
            var account = _test.CreateUniqueId("TestAccountGBP");
            writeResult = await _test.Client.CreateQuantifiableAccount(account, gbpId, 1000);
            Assert.IsTrue(writeResult.Success);
            
            writeResult = await _test.Client.AddBalance(account, gbpId, 50);
            Assert.IsTrue(writeResult.Success);
            
            var bruceClient = new IntegrationTestCreation();
            var bruceAccount = bruceClient.CreateUniqueId("BruceAccountGBP");
            writeResult = await bruceClient.Client.CreateQuantifiableAccount(bruceAccount, gbpId, 1000);
            Assert.IsTrue(writeResult.Success);
            
            var transferRequest = new TransferBuilder(_test.Client)
                .AddQuantityTransfer(account, bruceAccount, gbpId, 10)
                .Build();
            writeResult = await _test.Client.Write(transferRequest);
            Assert.IsTrue(writeResult.Success);
            
            var response = await bruceClient.Client.GetTransactions(gbpId, bruceAccount);
            CheckTransactionResponse(response);
        }

        [TestMethod]
        public async Task ShouldFetchQuantifiableDebitTransactions()
        {
            var gbpId = _test.CreateUniqueId("GBP");
            var writeResult = await _test.Client.CreateQuantifiableAssetType(gbpId, 2);
            Assert.IsTrue(writeResult.Success);
            
            var account = _test.CreateUniqueId("TestAccountGBP");
            writeResult = await _test.Client.CreateQuantifiableAccount(account, gbpId, 1000);
            Assert.IsTrue(writeResult.Success);
            
            writeResult = await _test.Client.AddBalance(account, gbpId, 50);
            Assert.IsTrue(writeResult.Success);
            
            var bruceClient = new IntegrationTestCreation();
            var bruceAccount = bruceClient.CreateUniqueId("BruceAccountGBP");
            writeResult = await bruceClient.Client.CreateQuantifiableAccount(bruceAccount, gbpId, 1000);
            Assert.IsTrue(writeResult.Success);
            
            var transferRequest = new TransferBuilder(_test.Client)
                .AddQuantityTransfer(account, bruceAccount, gbpId, 10)
                .Build();
            writeResult = await _test.Client.Write(transferRequest);
            Assert.IsTrue(writeResult.Success);
            
            var response = await _test.Client.GetTransactions(gbpId, account);
            CheckTransactionResponse(response);
        }

        [TestMethod]
        public async Task ShouldFetchUniqueCreditTransactions()
        {
            var horseId = _test.CreateUniqueId("horse");
            var writeResult = await _test.Client.CreateUniqueAssetType(horseId);
            Assert.IsTrue(writeResult.Success);
            
            var trevorId = _test.CreateUniqueId("trevor");
            writeResult = await _test.Client.CreateUniqueAsset(trevorId, horseId);
            Assert.IsTrue(writeResult.Success);
            
            var bruceClient = new IntegrationTestCreation();

            var transferRequest = new TransferBuilder(_test.Client)
                .AddOwnershipTransfer(trevorId, horseId, _test.Identity.Id, bruceClient.Identity.Id)
                .Build();
            writeResult = await _test.Client.Write(transferRequest);
            Assert.IsTrue(writeResult.Success);

            var response = await bruceClient.Client.GetTransactions(horseId, trevorId);
            CheckTransactionResponse(response);
        }

        [TestMethod]
        public async Task ShouldFetchUniqueDebitTransactions()
        {
            var horseId = _test.CreateUniqueId("horse");
            var writeResult = await _test.Client.CreateUniqueAssetType(horseId);
            Assert.IsTrue(writeResult.Success);
            
            var trevorId = _test.CreateUniqueId("trevor");
            writeResult = await _test.Client.CreateUniqueAsset(trevorId, horseId);
            Assert.IsTrue(writeResult.Success);
            
            var bruceClient = new IntegrationTestCreation();
            var transferRequest = new TransferBuilder(_test.Client)
                .AddOwnershipTransfer(trevorId, horseId, _test.Identity.Id, bruceClient.Identity.Id)
                .Build();
            writeResult = await _test.Client.Write(transferRequest);
            Assert.IsTrue(writeResult.Success);
            
            var response = await _test.Client.GetTransactions(horseId, trevorId);
            CheckTransactionResponse(response);
        }

        [TestMethod]
        public async Task ShouldFetchNextQuantifiableTransactions()
        {
            var gbpId = _test.CreateUniqueId("GBP");
            var writeResult = await _test.Client.CreateQuantifiableAssetType(gbpId, 2);
            Assert.IsTrue(writeResult.Success);
            
            var account = _test.CreateUniqueId("AccountGBP");
            writeResult = await _test.Client.CreateQuantifiableAccount(account, gbpId, 1000);
            Assert.IsTrue(writeResult.Success);
            
            writeResult = await _test.Client.AddBalance(account, gbpId, 50);
            Assert.IsTrue(writeResult.Success);
            
            var bruceClient = new IntegrationTestCreation();
            var bruceAccount = bruceClient.CreateUniqueId("AccountGBP");
            writeResult = await bruceClient.Client.CreateQuantifiableAccount(bruceAccount, gbpId, 1000);
            Assert.IsTrue(writeResult.Success);

            var transferRequest = new TransferBuilder(_test.Client)
                .AddQuantityTransfer(account, bruceAccount, gbpId, 5)
                .AddQuantityTransfer(account, bruceAccount, gbpId, 10)
                .Build();
            writeResult = await _test.Client.Write(transferRequest);
            Assert.IsTrue(writeResult.Success);
            
            var response = await _test.Client.GetTransactions(gbpId, account, 1);
            CheckTransactionResponse(response, 1, true);

            response = await _test.Client.GetTransactions(gbpId, account, 1, response.Value.Next);
            CheckTransactionResponse(response);            
        }

        [TestMethod]
        public async Task ShouldFetchNextUniqueTransactions()
        {
            var horseId = _test.CreateUniqueId("horse");
            var writeResult = await _test.Client.CreateUniqueAssetType(horseId);
            Assert.IsTrue(writeResult.Success);
            
            var trevorId = _test.CreateUniqueId("trevor");
            writeResult = await _test.Client.CreateUniqueAsset(trevorId, horseId);
            Assert.IsTrue(writeResult.Success);

            var bruceClient = new IntegrationTestCreation();
            
            var transferRequest = new TransferBuilder(_test.Client)
                .AddOwnershipTransfer(trevorId, horseId, _test.Identity.Id, bruceClient.Identity.Id)
                .Build();
            writeResult = await _test.Client.Write(transferRequest);
            Assert.IsTrue(writeResult.Success);

            transferRequest = new TransferBuilder(bruceClient.Client)
                .AddOwnershipTransfer(trevorId, horseId, bruceClient.Identity.Id, _test.Identity.Id)
                .Build();
            writeResult = await _test.Client.Write(transferRequest);
            Assert.IsTrue(writeResult.Success);

            var response = await _test.Client.GetTransactions(horseId, trevorId, 1);
            CheckTransactionResponse(response, 1, true);
            response = await _test.Client.GetTransactions(horseId, trevorId, 1, response.Value.Next);
            CheckTransactionResponse(response);
        }

        [TestMethod]
        public async Task ShouldDelegatesSeeHistory()
        {
            // Create asset type
            var gbpId = _test.CreateUniqueId("GBP");
            await _test.Client.CreateQuantifiableAssetType(gbpId, 2);
            
            var delegate1 = _test.IdentityBuilder.Create();
            var writeResult = await _test.Client.CreateIdentity(delegate1);
            Assert.IsTrue(writeResult.Success);

            writeResult = await _test.Client.AddDelegate(delegate1);
            Assert.IsTrue(writeResult.Success);

            using var delegateClient1 = await ClientBuilder.CreateWithNewIdentity(TestEnvironment.DefaultClientSettings, delegate1);
            delegateClient1.UseDelegator(_test.Identity.Id);

            var delegate2 = _test.IdentityBuilder.Create();
            writeResult = await _test.Client.CreateIdentity(delegate2);
            Assert.IsTrue(writeResult.Success);
            writeResult = await _test.Client.AddDelegate(delegate2);
            Assert.IsTrue(writeResult.Success);
            using var delegateClient2 = await ClientBuilder.CreateWithNewIdentity(TestEnvironment.DefaultClientSettings, delegate2);
            delegateClient2.UseDelegator(_test.Identity.Id);

            var accountA = _test.CreateUniqueId("AccountA");
            writeResult = await delegateClient1.CreateQuantifiableAccount(accountA, gbpId);
            Assert.IsTrue(writeResult.Success);
            writeResult = await delegateClient1.AddBalance(accountA, gbpId, 10);
            Assert.IsTrue(writeResult.Success);

            var accountB = _test.CreateUniqueId("AccountB");
            writeResult = await delegateClient1.CreateQuantifiableAccount(accountB, gbpId);
            Assert.IsTrue(writeResult.Success);

            var transferRequest = new TransferBuilder(delegateClient1)
                .AddQuantityTransfer(accountA, accountB, gbpId, 3)
                .Build();
            writeResult = await _test.Client.Write(transferRequest);
            Assert.IsTrue(writeResult.Success);

            // This transfer seems to take longer so build in a delay
            await Task.Delay(500);

            var getQuantifiableAssetResponse = await delegateClient2.GetQuantifiableAsset(accountA, gbpId);
            Assert.AreEqual("7", getQuantifiableAssetResponse.Value.Quantity);

            getQuantifiableAssetResponse = await delegateClient2.GetQuantifiableAsset(accountB, gbpId);
            Assert.AreEqual("3", getQuantifiableAssetResponse.Value.Quantity);

            var response = await delegateClient2.GetTransactions(gbpId, accountA);
            CheckTransactionResponse(response);
            response = await delegateClient2.GetTransactions(gbpId, accountB);
            CheckTransactionResponse(response);
        }

        private static void CheckTransactionResponse(ResponseResult<TransactionsResult> response, int expected = 1, bool expectedNext = false)
        {
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            if (expectedNext)
                Assert.IsNotNull(response.Value.Next);
            else
                Assert.IsNull(response.Value.Next);
            CheckTransactions(response.Value.Transactions, expected);
        }

        private static void CheckTransactions(IReadOnlyCollection<Transaction> transactions, int expected)
        {
            Assert.IsNotNull(transactions);
            Assert.AreEqual(expected, transactions.Count);
            foreach (var transaction in transactions)
            {

                Assert.IsNotNull(transaction.Proof);
                Assert.IsNotNull(transaction.RequestId);
                Assert.IsNotNull(transaction.TransactionTimestamp);
                switch (transaction)
                {
                    case QuantifiableTransaction quantifiable:
                        Assert.IsNotNull(quantifiable.Quantity);
                        Assert.IsNotNull(quantifiable.Sender);
                        Assert.IsNotNull(quantifiable.Sender.AssetId);
                        Assert.IsNotNull(quantifiable.Sender.AssetTypeId);
                        Assert.IsNotNull(quantifiable.Recipient);
                        Assert.IsNotNull(quantifiable.Recipient.AssetId);
                        Assert.IsNotNull(quantifiable.Recipient.AssetTypeId);
                        break;
                    case UniqueTransaction unique:
                        Assert.IsNotNull(unique.Asset);
                        Assert.IsNotNull(unique.Asset.AssetId);
                        Assert.IsNotNull(unique.Asset.AssetTypeId);
                        Assert.IsNotNull(unique.FromOwner);
                        Assert.IsNotNull(unique.ToOwner);
                        break;
                }
            }
        }
    }
}
