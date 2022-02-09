using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Iov42sdk.Crypto;
using Iov42sdk.Identity;
using Iov42sdk.Models;
using Iov42sdk.Models.AddDelegate;
using Iov42sdk.Models.CreateAsset;
using Iov42sdk.Models.CreateAssetType;
using Iov42sdk.Models.CreateClaims;
using Iov42sdk.Models.GetAsset;
using Iov42sdk.Models.GetAssetType;
using Iov42sdk.Models.GetClaim;
using Iov42sdk.Models.GetDelegates;
using Iov42sdk.Models.GetEndorsement;
using Iov42sdk.Models.GetIdentity;
using Iov42sdk.Models.GetIdentityPublicKey;
using Iov42sdk.Models.GetProof;
using Iov42sdk.Models.Headers;
using Iov42sdk.Models.Health;
using Iov42sdk.Models.IssueIdentity;
using Iov42sdk.Models.Transactions;
using Iov42sdk.Models.UpdateBalance;
using Iov42sdk.Support;

namespace Iov42sdk.Connection
{
    public class PlatformClient : IPlatformClient
    {
        private const string ApiLocation = "/api/v1/";
        private readonly Uri _baseUrl;
        private IdentityDetails _identity;
        private readonly IovClient _iovClient;
        private PlatformGetRequestBuilder _getBuilder;

        // Use ClientBuilder to create an instance
        internal PlatformClient(string baseUrl)
        {
            _baseUrl = new Uri(new Uri(baseUrl), ApiLocation);
            _iovClient = new IovClient(_baseUrl);
        }

        internal async Task<IKeyPair> Init(IdentityDetails identity, bool isNewIdentity)
        {
            _identity = identity;
            var info = await GetNodeInfo();
            _getBuilder = new PlatformGetRequestBuilder(_baseUrl, info.Value.NodeId);
            _iovClient.Init(_identity);
            if (isNewIdentity)
                await CreateIdentity(_identity);
            return _identity.Crypto.Pair;
        }

        public virtual void Dispose()
        {
            _iovClient?.Dispose();
        }
        public async Task<ResponseResult<WriteResult>> Write(PlatformWriteRequest request)
        {
            return await _iovClient.ProcessSignedPutRequest(request);
        }

        public async Task<ResponseResult<HealthStatusResult>> GetHealthStatus()
        {
            var request = _getBuilder.CreateUnsigned(NodeConstants.HealthChecksEndPoint);
            return await _iovClient.ProcessSimpleGetRequest<HealthStatusResult>(request.Path);
        }

        public async Task<ResponseResult<WriteResult>> CreateIdentity(IdentityDetails identity)
        {
            var body = new IssueIdentityBody(identity.Id, new Credentials(identity.Crypto.Pair.PublicKeyBase64String, identity.Crypto.ProtocolId));
            var request = BuildRequest(body, new[] {identity}, identity);
            return await Write(request);
        }

        public void UseDelegator(string delegatorId)
        {
            _iovClient.WithDelegator(delegatorId);
        }

        public void StopUsingDelegator()
        {
            UseDelegator(null);
        }

        public async Task<ResponseResult<IdentityResult>> GetIdentity(string identity)
        {
            var request = _getBuilder.Create(NodeConstants.IdentitiesEndPoint, identity);
            return await _iovClient.ProcessSignedGetRequest<IdentityResult>(request.Path);
        }

        public async Task<ResponseResult<IdentityPublicKeyResult>> GetIdentityPublicKey(string identity)
        {
            var request = _getBuilder.Create(NodeConstants.IdentitiesEndPoint, identity, NodeConstants.PublicKeyEndPoint);
            return await _iovClient.ProcessSignedGetRequest<IdentityPublicKeyResult>(request.Path);
        }

        public async Task<ResponseResult<WriteResult>> AddDelegate(IdentityDetails delegateIdentity)
        {
            var body = new AddDelegateBody(delegateIdentity.Id, _identity.Id);
            var request = BuildRequest(body, new[] {_identity, delegateIdentity}, _identity);
            return await Write(request);
        }
        
        public async Task<ResponseResult<GetDelegatesResult>> GetIdentityDelegates(string identityId = null)
        {
            var id = identityId ?? _identity.Id;
            var request = _getBuilder.Create(NodeConstants.IdentitiesEndPoint, id, NodeConstants.DelegatesEndPoint);
            return await _iovClient.ProcessSignedGetRequest<GetDelegatesResult>(request.Path);
        }

        public async Task<ResponseResult<WriteResult>> CreateUniqueAssetType(string assetTypeId)
        {
            var body = new CreateUniqueAssetTypeBody(assetTypeId);
            var request = BuildRequest(body, new[] {_identity}, _identity);
            return await Write(request);
        }
        
        public async Task<ResponseResult<WriteResult>> CreateQuantifiableAssetType(string assetTypeId, int scale)
        {
            var body = new CreateQuantifiableAssetTypeBody(assetTypeId, scale);
            var request = BuildRequest(body, new[] {_identity}, _identity);
            return await Write(request);
        }
        
        public async Task<ResponseResult<UniqueAssetTypeResult>> GetUniqueAssetType(string assetTypeAddress)
        {
            var request = _getBuilder.Create(NodeConstants.AssetTypesEndPoint, assetTypeAddress);
            return await _iovClient.ProcessSignedGetRequest<UniqueAssetTypeResult>(request.Path);
        }

        public async Task<ResponseResult<QuantifiableAssetTypeResult>> GetQuantifiableAssetType(string assetTypeAddress)
        {
            var request = _getBuilder.Create(NodeConstants.AssetTypesEndPoint, assetTypeAddress);
            return await _iovClient.ProcessSignedGetRequest<QuantifiableAssetTypeResult>(request.Path);
        }

        public async Task<ResponseResult<ProofResult>> GetProof(string requestId)
        {
            var request = _getBuilder.Create(NodeConstants.ProofsEndPoint, requestId);
            return await _iovClient.ProcessSignedGetRequest<ProofResult>(request.Path);
        }

        public async Task<ResponseResult<WriteResult>> CreateUniqueAsset(string address, string assetTypeAddress)
        {
            var body = new CreateUniqueAssetBody(address, assetTypeAddress);
            var request = BuildRequest(body, new[] {_identity}, _identity);
            return await Write(request);
        }

        public async Task<ResponseResult<WriteResult>> CreateQuantifiableAccount(string address, string assetTypeAddress, BigInteger quantity = new BigInteger())
        {
            var body = new CreateQuantifiableAssetBody(address, assetTypeAddress, quantity.ToString());
            var request = BuildRequest(body, new[] {_identity}, _identity);
            return await Write(request);
        }

        public async Task<ResponseResult<WriteResult>> AddBalance(string address, string assetTypeAddress, BigInteger quantity)
        {
            var body = new UpdateBalanceBody(address, assetTypeAddress, quantity.ToString());
            var request = BuildRequest(body, new[] {_identity}, _identity);
            return await Write(request);
        }

        public async Task<ResponseResult<UniqueAssetResult>> GetUniqueAsset(string address, string assetTypeAddress)
        {
            var request = _getBuilder.Create(NodeConstants.AssetTypesEndPoint, assetTypeAddress, NodeConstants.AssetsEndPoint, address);
            return await _iovClient.ProcessSignedGetRequest<UniqueAssetResult>(request.Path);
        }

        public async Task<ResponseResult<QuantifiableAssetResult>> GetQuantifiableAsset(string address, string assetTypeAddress)
        {
            var request = _getBuilder.Create(NodeConstants.AssetTypesEndPoint, assetTypeAddress, NodeConstants.AssetsEndPoint, address);
            return await _iovClient.ProcessSignedGetRequest<QuantifiableAssetResult>(request.Path);
        }
        
        public async Task<ResponseResult<NodeInfo>> GetNodeInfo()
        {
            return await _iovClient.ProcessSimpleGetRequest<NodeInfo>(NodeConstants.NodeInfoEndPoint);
        }

        public async Task<ResponseResult<WriteResult>> CreateIdentityClaims(params string[] claims)
        {
            return await _iovClient.CreateClaims(claimMap =>
                new CreateClaimsBody(NodeConstants.CreateIdentityClaimsRequestType, _identity.Id, claimMap.Keys.ToArray()), claims);
        }

        public async Task<ResponseResult<ClaimsResult>> GetIdentityClaims(int limit = 20, string next = null)
        {
            var request = _getBuilder.Create(NodeConstants.IdentitiesEndPoint, _identity.Id, NodeConstants.ClaimsEndPoint)
                .WithPagingParameters(limit, next);
            return await _iovClient.ProcessSignedGetRequest<ClaimsResult>(request.Path);
        }

        public async Task<ResponseResult<ClaimsResult>> GetIdentityClaims(string identityId, int limit = 20, string next = null)
        {
            var request = _getBuilder.Create(NodeConstants.IdentitiesEndPoint, identityId, NodeConstants.ClaimsEndPoint)
                .WithPagingParameters(limit, next);
            return await _iovClient.ProcessSignedGetRequest<ClaimsResult>(request.Path);
        }

        public async Task<ResponseResult<ClaimResult>> GetIdentityClaim(string claim)
        {
            var request = _getBuilder.Create(NodeConstants.IdentitiesEndPoint, _identity.Id, NodeConstants.ClaimsEndPoint, _identity.Crypto.GetHash(claim));
            return await _iovClient.ProcessSignedGetRequest<ClaimResult>(request.Path);
        }

        public async Task<ResponseResult<ClaimResult>> GetIdentityClaim(string identityId, string claim)
        {
            var request = _getBuilder.Create(NodeConstants.IdentitiesEndPoint, identityId, NodeConstants.ClaimsEndPoint, _identity.Crypto.GetHash(claim));
            return await _iovClient.ProcessSignedGetRequest<ClaimResult>(request.Path);
        }

        public Endorsements CreateIdentityEndorsements(string identityId = null)
        {
            return CreateEndorsements(null, identityId ?? _identity.Id);
        }

        public Endorsements CreateAssetTypeEndorsements(string assetTypeId)
        {
            return CreateEndorsements(null, assetTypeId);
        }

        public Endorsements CreateAssetEndorsements(string subjectTypeId, string subject)
        {
            return CreateEndorsements(subjectTypeId, subject);
        }

        public async Task<ResponseResult<WriteResult>> CreateIdentityClaimsEndorsements(Endorsements endorsements, string requestId, string body, params Authorisation[] authorisations)
        {
            return await _iovClient.CreateClaimsEndorsements(endorsements, requestId, body, authorisations);
        }

        public async Task<ResponseResult<EndorsementResult>> GetIdentityEndorsement(string identity, string claim, string endorser)
        {
            var request = _getBuilder.Create(NodeConstants.IdentitiesEndPoint, identity, NodeConstants.ClaimsEndPoint, _identity.Crypto.GetHash(claim), NodeConstants.EndorsementsEndPoint, endorser);
            return await _iovClient.ProcessSignedGetRequest<EndorsementResult>(request.Path);
        }

        public async Task<ResponseResult<WriteResult>> CreateAssetTypeClaims(string assetTypeId, params string[] claims)
        {
            return await _iovClient.CreateClaims(claimMap =>
                new CreateClaimsBody(NodeConstants.CreateAssetTypeClaimsRequestType, assetTypeId, claimMap.Keys.ToArray()), claims);
        }

        public async Task<ResponseResult<ClaimsResult>> GetAssetTypeClaims(string assetTypeId, int limit = 20, string next = null)
        {
            var request = _getBuilder.Create(NodeConstants.AssetTypesEndPoint, assetTypeId, NodeConstants.ClaimsEndPoint)
                .WithPagingParameters(limit, next);
            return await _iovClient.ProcessSignedGetRequest<ClaimsResult>(request.Path);
        }

        public async Task<ResponseResult<ClaimResult>> GetAssetTypeClaim(string assetTypeId, string claim)
        {
            var request = _getBuilder.Create(NodeConstants.AssetTypesEndPoint, assetTypeId, NodeConstants.ClaimsEndPoint, _identity.Crypto.GetHash(claim));
            return await _iovClient.ProcessSignedGetRequest<ClaimResult>(request.Path);
        }

        public async Task<ResponseResult<WriteResult>> CreateAssetTypeClaimsEndorsements(Endorsements endorsements, string requestId, string body, params Authorisation[] authorisations)
        {
            return await _iovClient.CreateClaimsEndorsements(endorsements, requestId, body, authorisations);
        }

        public async Task<ResponseResult<EndorsementResult>> GetAssetTypeEndorsement(string assetTypeId, string claim, string endorser)
        {
            var request = _getBuilder.Create(NodeConstants.AssetTypesEndPoint, assetTypeId, NodeConstants.ClaimsEndPoint, _identity.Crypto.GetHash(claim), NodeConstants.EndorsementsEndPoint, endorser);
            return await _iovClient.ProcessSignedGetRequest<EndorsementResult>(request.Path);
        }

        public async Task<ResponseResult<WriteResult>> CreateAssetClaims(string assetTypeId, string assetId, params string[] claims)
        {
            return await _iovClient.CreateClaims(claimMap =>
                new CreateClaimsBody(NodeConstants.CreateAssetClaimsRequestType, assetTypeId, assetId, claimMap.Keys.ToArray()), claims);
        }

        public async Task<ResponseResult<ClaimsResult>> GetAssetClaims(string assetTypeId, string assetId, int limit = 20, string next = null)
        {
            var request = _getBuilder.Create(NodeConstants.AssetTypesEndPoint, assetTypeId, NodeConstants.AssetsEndPoint, assetId, NodeConstants.ClaimsEndPoint)
                .WithPagingParameters(limit, next);
            return await _iovClient.ProcessSignedGetRequest<ClaimsResult>(request.Path);
        }

        public async Task<ResponseResult<ClaimResult>> GetAssetClaim(string assetTypeId, string assetId, string claim)
        {
            var request = _getBuilder.Create(NodeConstants.AssetTypesEndPoint, assetTypeId, NodeConstants.AssetsEndPoint, assetId, NodeConstants.ClaimsEndPoint, _identity.Crypto.GetHash(claim));
            return await _iovClient.ProcessSignedGetRequest<ClaimResult>(request.Path);
        }

        public async Task<ResponseResult<WriteResult>> CreateAssetClaimsEndorsements(Endorsements endorsements, string requestId, string body, params Authorisation[] authorisations)
        {
            return await _iovClient.CreateClaimsEndorsements(endorsements, requestId, body, authorisations);
        }

        public async Task<ResponseResult<EndorsementResult>> GetAssetEndorsement(string assetTypeId, string assetId, string claim, string endorser)
        {
            var request = _getBuilder.Create(NodeConstants.AssetTypesEndPoint, assetTypeId, NodeConstants.AssetsEndPoint, assetId, NodeConstants.ClaimsEndPoint, _identity.Crypto.GetHash(claim), NodeConstants.EndorsementsEndPoint, endorser);
            return await _iovClient.ProcessSignedGetRequest<EndorsementResult>(request.Path);
        }

        public async Task<ResponseResult<TransactionsResult>> GetTransactions(string assetTypeId, string assetId, int limit = 20, string next = null)
        {
            var request = _getBuilder.Create(NodeConstants.AssetTypesEndPoint, assetTypeId, NodeConstants.AssetsEndPoint, assetId, NodeConstants.TransactionsEndPoint)
                .WithPagingParameters(limit, next);
            var transactions = await _iovClient.ProcessSignedGetRequest<CombinedTransactionsResult>(request.Path);
            var expanded = transactions.Value.Transactions.Select(x => string.IsNullOrEmpty(x.Quantity) ? (Transaction)new UniqueTransaction(x) : new QuantifiableTransaction(x)).ToArray();
            var processed = new TransactionsResult { Next = transactions.Value.Next, Transactions = expanded };
            return new ResponseResult<TransactionsResult>(processed, transactions.Success, transactions.ResponseCode, transactions.Reason);
        }

        public Authorisation GenerateAuthorisation(string body, IdentityDetails identity = null)
        {
            return _iovClient.GenerateAuthorisationHeader(body, identity ?? _identity);
        }

        public AuthenticationHeader GenerateAuthentication(Authorisation[] authorisations, IdentityDetails identity = null)
        {
            return _iovClient.GenerateAuthenticationHeader(identity ?? _identity, authorisations);
        }

        private Endorsements CreateEndorsements(string subjectTypeId, string subject)
        {
            return new Endorsements(IovClient.CreateUniqueId(), subjectTypeId, subject, _identity);
        }

        public PlatformWriteRequest BuildRequest(WriteBody body, IdentityDetails[] authorisationIdentities = null, IdentityDetails authenticationIdentity = null)
        {
            var bodyText = body.Serialize();
            var authorisations = (authorisationIdentities ?? new[] {_identity}).Select(x => GenerateAuthorisation(bodyText, x)).ToArray();
            var authentication = GenerateAuthentication(authorisations, authenticationIdentity);
            return new PlatformWriteRequest(body.RequestId, bodyText, authorisations, authentication);
        }

        public Dictionary<string, string> GenerateClaimsHeader(Dictionary<string, string> claimMap)
        {
            return _iovClient.GenerateClaimsHeader(claimMap);
        }

        public bool VerifyIdentityEndorsement(ICrypto crypto, string identityId, string plaintextClaim, string endorsement)
        {
            return VerifyAssetEndorsement(crypto, null, identityId, plaintextClaim, endorsement);
        }

        public bool VerifyAssetTypeEndorsement(ICrypto crypto, string assetType, string plaintextClaim, string endorsement)
        {
            return VerifyAssetEndorsement(crypto, null, assetType, plaintextClaim, endorsement);
        }

        public bool VerifyAssetEndorsement(ICrypto crypto, string assetType, string asset, string plaintextClaim, string endorsement)
        {
            var buildClaimContent = Endorsements.BuildClaimContent(crypto, assetType, asset, plaintextClaim);
            return crypto.VerifySignature(crypto.Pair, endorsement, UsefulConversions.ToBytes(buildClaimContent));
        }
    }
}