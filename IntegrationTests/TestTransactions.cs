using System.Collections.Generic;
using System.Threading.Tasks;
using IntegrationTests.Support;
using Iov42sdk.Models.Transactions;
using Iov42sdk.Models.Transfers;
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
        public async Task ShouldFetchTransactions()
        {
            var horseId = _test.CreateUniqueId("horse");
            await _test.Client.CreateUniqueAssetType(horseId);
            var trevorId = _test.CreateUniqueId("trevor");
            var _ = await _test.Client.CreateUniqueAsset(trevorId, horseId);
            var gbpId = _test.CreateUniqueId("GBP");
            var account = _test.CreateUniqueId("AccountGBP");
            await _test.Client.CreateQuantifiableAssetType(gbpId, 2);
            await _test.Client.CreateQuantifiableAccount(account, gbpId, 1000);
            var __ = await _test.Client.AddBalance(account, gbpId, 50);
            var bruceClient = new IntegrationTestCreation();
            var bruceAccount = bruceClient.CreateUniqueId("AccountGBP");
            await bruceClient.Client.CreateQuantifiableAccount(bruceAccount, gbpId, 1000);
            var uniqueTransfer = _test.Client.CreateOwnershipTransfer(trevorId, horseId, _test.Identity.Id, bruceClient.Identity.Id);
            var quantityTransfer = _test.Client.CreateQuantityTransfer(account, bruceAccount, gbpId, 10);
            var transferRequest = new TransferRequest(quantityTransfer, uniqueTransfer)
                .AddAuthorisation(_test.Client.GenerateAuthorisation)
                .AddAuthorisation(bruceClient.Client.GenerateAuthorisation);
            var ____ = await _test.Client.TransferAssets(transferRequest);
            // Currently only supports credits
            // var response = await _test.Client.GetTransactions(horseId, trevorId);
            var response = await _test.Client.GetTransactions(gbpId, account);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsNull(response.Value.Next);
            CheckTransactions(response.Value.Transactions, 1);
        }

        [TestMethod]
        public async Task ShouldFetchNextTransactions()
        {
            var horseId = _test.CreateUniqueId("horse");
            await _test.Client.CreateUniqueAssetType(horseId);
            var trevorId = _test.CreateUniqueId("trevor");
            var _ = await _test.Client.CreateUniqueAsset(trevorId, horseId);
            var gbpId = _test.CreateUniqueId("GBP");
            var account = _test.CreateUniqueId("AccountGBP");
            await _test.Client.CreateQuantifiableAssetType(gbpId, 2);
            await _test.Client.CreateQuantifiableAccount(account, gbpId, 1000);
            var __ = await _test.Client.AddBalance(account, gbpId, 50);
            var bruceClient = new IntegrationTestCreation();
            var bruceAccount = bruceClient.CreateUniqueId("AccountGBP");
            await bruceClient.Client.CreateQuantifiableAccount(bruceAccount, gbpId, 1000);
            var uniqueTransfer = _test.Client.CreateOwnershipTransfer(trevorId, horseId, _test.Identity.Id, bruceClient.Identity.Id);
            var quantity1Transfer = _test.Client.CreateQuantityTransfer(account, bruceAccount, gbpId, 5);
            var quantity2Transfer = _test.Client.CreateQuantityTransfer(account, bruceAccount, gbpId, 10);
            var transferRequest = new TransferRequest(quantity1Transfer, quantity2Transfer, uniqueTransfer)
                .AddAuthorisation(_test.Client.GenerateAuthorisation)
                .AddAuthorisation(bruceClient.Client.GenerateAuthorisation);
            var ____ = await _test.Client.TransferAssets(transferRequest);
            // Currently only supports credits
            // var response = await _test.Client.GetTransactions(horseId, trevorId, 1, null);
            var response = await _test.Client.GetTransactions(gbpId, account, 1);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Value.Next);
            CheckTransactions(response.Value.Transactions, 1);
            response = await _test.Client.GetTransactions(gbpId, account, 1, response.Value.Next);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsNull(response.Value.Next);
            CheckTransactions(response.Value.Transactions, 1);
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
                Assert.IsNotNull(transaction.Quantity);
                Assert.IsNotNull(transaction.Sender);
                Assert.IsNotNull(transaction.Sender.AssetId);
                Assert.IsNotNull(transaction.Sender.AssetTypeId);
                Assert.IsNotNull(transaction.Recipient);
                Assert.IsNotNull(transaction.Recipient.AssetId);
                Assert.IsNotNull(transaction.Recipient.AssetTypeId);
            }
        }
    }
}
