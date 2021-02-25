using System.Threading.Tasks;
using IntegrationTests.Support;
using Iov42sdk.Models;
using Iov42sdk.Models.GetProof;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests
{
    [TestClass]
    public class TestProofCalls
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
        public async Task ShouldGetAProof()
        {
            var newId = _test.IdentityBuilder.Create();
            var issueIdentityResponse = await _test.Client.CreateIdentity(newId);

            var proofId = issueIdentityResponse.Value.RequestId;
            var proofResponse = await _test.Client.GetProof(proofId);
            
            Assert.IsTrue(proofResponse.Success);
            Assert.IsNotNull(proofResponse.Value);
            Assert.IsNotNull(proofResponse.Value.RequestId);
            
            VerifyFingerprints(proofResponse.Value.ParentFingerprints);
            VerifySignatories(proofResponse.Value.Signatories);
            VerifyProof(proofResponse.Value.Proof);
            // Could be extended to actually check all the signing as well
        }

        private static void VerifyProof(Proof proof)
        {
            Assert.IsNotNull(proof);
            VerifyNodes(proof.Nodes);
        }

        private static void VerifyNodes(ProofNode[] nodes)
        {
            Assert.IsNotNull(nodes);
            Assert.IsTrue(nodes.Length > 0);
            foreach (var node in nodes) 
                VerifyNode(node);
        }

        private static void VerifyNode(ProofNode node)
        {
            Assert.IsNotNull(node);
            VerifyProofNodeId(node.Id);
            if (IsRoot(node.Id?.Type))
                return;
            if (!IsCompletion(node.Id?.Type) && !IsAuthentication(node.Id?.Type))
                Assert.IsNotNull(node.Payload);
            VerifyNodeLinks(node.Links);
        }

        private static void VerifyNodeLinks(ProofNodeLink[] links)
        {
            Assert.IsNotNull(links);
            Assert.IsTrue(links.Length > 0);
            foreach (var link in links) 
                VerifyNodeLink(link);
        }

        private static void VerifyNodeLink(ProofNodeLink link)
        {
            Assert.IsNotNull(link);
            VerifySeals(link.Seals);
            VerifyTarget(link.Target);
        }

        private static void VerifyTarget(LinkTarget linkTarget)
        {
            Assert.IsNotNull(linkTarget);
            Assert.IsNotNull(linkTarget.Type);
            if (!IsCompletionDetails(linkTarget.Type) && !IsSecurity(linkTarget.Type) && !IsRoot(linkTarget.Type))
                Assert.IsNotNull(linkTarget.Id);
        }

        private static void VerifySeals(ProofSeal[] seals)
        {
            Assert.IsNotNull(seals);
            foreach (var seal in seals)
                VerifySeal(seal);
        }

        private static void VerifyProofNodeId(ProofNodeId nodeId)
        {
            Assert.IsNotNull(nodeId);
            Assert.IsNotNull(nodeId.Type);
            if (!ContainsCompletion(nodeId.Type) && !IsSecurity(nodeId.Type) && !IsRoot(nodeId.Type))
                Assert.IsNotNull(nodeId.Id);
        }

        private static void VerifySignatories(ProofSignatory[] signatories)
        {
            Assert.IsNotNull(signatories);
            Assert.IsTrue(signatories.Length > 0);
            Assert.IsNotNull(signatories[0].Identity);
            VerifyCredentials(signatories[0].Credentials);
        }

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private static void VerifyCredentials(Credentials credentials)
        {
            Assert.IsNotNull(credentials);
            Assert.IsNotNull(credentials.ProtocolId);
            Assert.IsNotNull(credentials.Key);
        }

        private static void VerifyFingerprints(Fingerprint[] fingerprints)
        {
            Assert.IsNotNull(fingerprints);
            Assert.IsTrue(fingerprints.Length > 0);
            foreach (var fingerprint in fingerprints)
                VerifyFingerprint(fingerprint);
        }

        private static void VerifyFingerprint(Fingerprint fingerprint)
        {
            Assert.IsNotNull(fingerprint.RequestId);
            Assert.IsNotNull(fingerprint.Seal);
            VerifySeal(fingerprint.Seal);
        }

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private static void VerifySeal(ProofSeal seal)
        {
            Assert.IsNotNull(seal.IdentityId);
            Assert.IsNotNull(seal.ProtocolId);
            Assert.IsNotNull(seal.Signature);
        }

        private static bool IsRoot(string text)
        {
            return text == "Root";
        }

        private static bool IsSecurity(string text)
        {
            return IsAuthentication(text) || text == "Authorisation";
        }

        private static bool IsAuthentication(string text)
        {
            return text == "Authentication";
        }

        private static bool ContainsCompletion(string text)
        {
            return IsCompletion(text) || IsCompletionDetails(text);
        }

        private static bool IsCompletion(string text)
        {
            return text == "Completion";
        }

        private static bool IsCompletionDetails(string text)
        {
            return text == "CompletionDetails";
        }
    }
}