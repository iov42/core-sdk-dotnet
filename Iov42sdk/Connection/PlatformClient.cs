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
using Iov42sdk.Models.CreateEndorsements;
using Iov42sdk.Models.GetAsset;
using Iov42sdk.Models.GetAssetType;
using Iov42sdk.Models.GetClaim;
using Iov42sdk.Models.GetDelegates;
using Iov42sdk.Models.GetEndorsement;
using Iov42sdk.Models.GetIdentity;
using Iov42sdk.Models.GetIdentityPublicKey;
using Iov42sdk.Models.GetProof;
using Iov42sdk.Models.GetRequestStatus;
using Iov42sdk.Models.Headers;
using Iov42sdk.Models.Health;
using Iov42sdk.Models.IssueIdentity;
using Iov42sdk.Models.Transactions;
using Iov42sdk.Models.Transfers;
using Iov42sdk.Models.UpdateBalance;
using Iov42sdk.Support;

namespace Iov42sdk.Connection
{
    public class PlatformClient : IPlatformClient
    {
        private readonly IdentityDetails _identity;
        private readonly IovClient _iovClient;

        // Use ClientBuilder to create an instance
        internal PlatformClient(string baseUrl, IdentityDetails identity)
        {
            _identity = identity;
            _iovClient = new IovClient(baseUrl);
        }

        internal async Task<IKeyPair> Init(bool isNewIdentity, string requestIdRoot = null)
        {
            var info  = await GetNodeInfo();
            _iovClient.Init(_identity, info.Value, requestIdRoot);
            if (isNewIdentity)
                await CreateIdentity(_identity);
            return _identity.Crypto.Pair;
        }

        public virtual void Dispose()
        {
            _iovClient?.Dispose();
        }

        public async Task<ResponseResult<HealthStatusResult>> GetHealthStatus()
        {
            return await _iovClient.ProcessSimpleGetRequest<HealthStatusResult>(NodeConstants.HealthChecksEndPoint);
        }

        public async Task<ResponseResult<RequestStatusResult>> GetRequestStatus(string requestId)
        {
            return await _iovClient.ProcessSimpleGetRequest<RequestStatusResult>(_iovClient.BuildPath(NodeConstants.RequestsEndPoint, requestId));
        }

        public async Task<ResponseResult<CreateIdentityResult>> CreateIdentity(IdentityDetails identity)
        {
            var body = new IssueIdentityBody(identity.Id, new Credentials
                {
                    Key = identity.Crypto.Pair.PublicKeyBase64String,
                    ProtocolId = identity.Crypto.ProtocolId
                });
            return await _iovClient.ProcessSignedPutRequest<IssueIdentityBody, CreateIdentityResult>(new [] { identity }, body);
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
            var path = _iovClient.BuildPath(NodeConstants.IdentitiesEndPoint, identity);
            return await _iovClient.ProcessSignedGetRequest<IdentityResult>(_identity.Id, path);
        }

        public async Task<ResponseResult<IdentityPublicKeyResult>> GetIdentityPublicKey(string identity)
        {
            var path = _iovClient.BuildPath(NodeConstants.IdentitiesEndPoint, identity, NodeConstants.PublicKeyEndPoint);
            return await _iovClient.ProcessSignedGetRequest<IdentityPublicKeyResult>(_identity.Id, path);
        }

        public async Task<ResponseResult<AddDelegateResult>> AddDelegate(IdentityDetails delegateIdentity)
        {
            var body = new AddDelegateBody(delegateIdentity.Id, _identity.Id);
            return await _iovClient.ProcessSignedPutRequest<AddDelegateBody, AddDelegateResult>(new[] { _identity, delegateIdentity }, body);
        }

        public async Task<ResponseResult<GetDelegatesResult>> GetIdentityDelegates(string identityId = null)
        {
            var id = identityId ?? _identity.Id;
            var path = _iovClient.BuildPath(NodeConstants.IdentitiesEndPoint, id, NodeConstants.DelegatesEndPoint);
            return await _iovClient.ProcessSignedGetRequest<GetDelegatesResult>(id, path);
        }

        public async Task<ResponseResult<CreateAssetTypeResult>> CreateUniqueAssetType(string assetTypeId)
        {
            var body = new CreateUniqueAssetTypeBody(assetTypeId);
            return await _iovClient.ProcessSignedPutRequest<CreateAssetTypeBody, CreateAssetTypeResult>(body);
        }

        public async Task<ResponseResult<CreateAssetTypeResult>> CreateQuantifiableAssetType(string assetTypeId, int scale)
        {
            var body = new CreateQuantifiableAssetTypeBody(assetTypeId, scale);
            return await _iovClient.ProcessSignedPutRequest<CreateAssetTypeBody, CreateAssetTypeResult>(body);
        }

        public async Task<ResponseResult<UniqueAssetTypeResult>> GetUniqueAssetType(string assetTypeAddress)
        {
            var path = _iovClient.BuildPath(NodeConstants.AssetTypesEndPoint, assetTypeAddress);
            return await _iovClient.ProcessSignedGetRequest<UniqueAssetTypeResult>(_identity.Id, path);
        }

        public async Task<ResponseResult<QuantifiableAssetTypeResult>> GetQuantifiableAssetType(string assetTypeAddress)
        {
            var path = _iovClient.BuildPath(NodeConstants.AssetTypesEndPoint, assetTypeAddress);
            return await _iovClient.ProcessSignedGetRequest<QuantifiableAssetTypeResult>(_identity.Id, path);
        }

        public async Task<ResponseResult<ProofResult>> GetProof(string requestId)
        {
            var path = _iovClient.BuildPath(NodeConstants.ProofsEndPoint, requestId);
            return await _iovClient.ProcessSignedGetRequest<ProofResult>(_identity.Id, path);
        }

        public async Task<ResponseResult<CreateAssetResult>> CreateUniqueAsset(string address, string assetTypeAddress)
        {
            var body = new CreateUniqueAssetBody(address, assetTypeAddress);
            return await _iovClient.ProcessSignedPutRequest<CreateAssetBody, CreateAssetResult>(body);
        }

        public async Task<ResponseResult<CreateAssetResult>> CreateQuantifiableAccount(string address, string assetTypeAddress, BigInteger quantity = new BigInteger())
        {
            var body = new CreateQuantifiableAssetBody(address, assetTypeAddress, quantity.ToString());
            return await _iovClient.ProcessSignedPutRequest<CreateAssetBody, CreateAssetResult>(body);
        }

        public async Task<ResponseResult<UpdateBalanceResult>> AddBalance(string address, string assetTypeAddress, BigInteger quantity)
        {
            var body = new UpdateBalanceBody(address, assetTypeAddress, quantity.ToString());
            return await _iovClient.ProcessSignedPutRequest<UpdateBalanceBody, UpdateBalanceResult>(body);
        }

        public async Task<ResponseResult<UniqueAssetResult>> GetUniqueAsset(string address, string assetTypeAddress)
        {
            var path = _iovClient.BuildPath(NodeConstants.AssetTypesEndPoint, assetTypeAddress, NodeConstants.AssetsEndPoint, address);
            return await _iovClient.ProcessSignedGetRequest<UniqueAssetResult>(_identity.Id, path);
        }

        public async Task<ResponseResult<QuantifiableAssetResult>> GetQuantifiableAsset(string address, string assetTypeAddress)
        {
            var path = _iovClient.BuildPath(NodeConstants.AssetTypesEndPoint, assetTypeAddress, NodeConstants.AssetsEndPoint, address);
            return await _iovClient.ProcessSignedGetRequest<QuantifiableAssetResult>(_identity.Id, path);
        }

        public async Task<ResponseResult<TransfersResult>> TransferAssets(params SingleTransfer[] transfers)
        {
            var body = new TransfersBody(transfers);
            return await _iovClient.ProcessSignedPutRequest<TransfersBody, TransfersResult>(body);
        }

        public async Task<ResponseResult<TransfersResult>> TransferAssets(TransferRequest request)
        {
            return await _iovClient.ProcessSignedPutRequest<TransfersBody, TransfersResult>(request.Body, request.Authorisations);
        }

        public SingleTransfer CreateOwnershipTransfer(string assetId, string assetTypeId, string fromIdentityId, string toIdentityId)
        {
            return new TransferOwnership(assetId, assetTypeId, fromIdentityId, toIdentityId);
        }

        public SingleTransfer CreateQuantityTransfer(string fromAssetId, string toAssetId, string assetTypeId, BigInteger quantity)
        {
            return new TransferQuantity(fromAssetId, toAssetId,  assetTypeId, quantity.ToString());
        }

        public async Task<ResponseResult<NodeInfo>> GetNodeInfo()
        {
            return await _iovClient.ProcessSimpleGetRequest<NodeInfo>(_iovClient.BuildPath(NodeConstants.NodeInfoEndPoint));
        }

        public async Task<ResponseResult<CreateClaimsResult>> CreateIdentityClaims(params string[] claims)
        {
            return await _iovClient.CreateClaims(claimMap => 
                new CreateClaimsBody(NodeConstants.CreateIdentityClaimsRequestType, _identity.Id, claimMap.Keys.ToArray()), claims);
        }

        public async Task<ResponseResult<ClaimsResult>> GetIdentityClaims(int limit = 20, string next = null)
        {
            var parameters = IovClient.BuildPagingParameters(limit, next);
            var path = _iovClient.BuildPath(NodeConstants.IdentitiesEndPoint, _identity.Id, NodeConstants.ClaimsEndPoint);
            return await _iovClient.ProcessSignedGetRequest<ClaimsResult>(_identity.Id, path, parameters);
        }

        public async Task<ResponseResult<ClaimsResult>> GetIdentityClaims(string identityId, int limit = 20, string next = null)
        {
            var parameters = IovClient.BuildPagingParameters(limit, next);
            var path = _iovClient.BuildPath(NodeConstants.IdentitiesEndPoint, identityId, NodeConstants.ClaimsEndPoint);
            return await _iovClient.ProcessSignedGetRequest<ClaimsResult>(_identity.Id, path, parameters);
        }

        public async Task<ResponseResult<ClaimResult>> GetIdentityClaim(string claim)
        {
            var path = _iovClient.BuildPath(NodeConstants.IdentitiesEndPoint, _identity.Id, NodeConstants.ClaimsEndPoint, _identity.Crypto.GetHash(claim));
            return await _iovClient.ProcessSignedGetRequest<ClaimResult>(_identity.Id, path);
        }

        public async Task<ResponseResult<ClaimResult>> GetIdentityClaim(string identityId, string claim)
        {
            var path = _iovClient.BuildPath(NodeConstants.IdentitiesEndPoint, identityId, NodeConstants.ClaimsEndPoint, _identity.Crypto.GetHash(claim));
            return await _iovClient.ProcessSignedGetRequest<ClaimResult>(_identity.Id, path);
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

        public async Task<ResponseResult<CreateEndorsementsResult>> CreateIdentityClaimsEndorsements(Endorsements endorsements, params Authorisation[] authorisations)
        {
            return await _iovClient.CreateClaimsEndorsements(endorsements, endorsements.GenerateIdentityEndorsementBody, authorisations);
        }

        public async Task<ResponseResult<EndorsementResult>> GetIdentityEndorsement(string identity, string claim, string endorser)
        {
            var path = _iovClient.BuildPath(NodeConstants.IdentitiesEndPoint, identity, NodeConstants.ClaimsEndPoint, _identity.Crypto.GetHash(claim), NodeConstants.EndorsementsEndPoint, endorser);
            return await _iovClient.ProcessSignedGetRequest<EndorsementResult>(_identity.Id, path);
        }

        public async Task<ResponseResult<CreateClaimsResult>> CreateAssetTypeClaims(string assetTypeId, params string[] claims)
        {
            return await _iovClient.CreateClaims(claimMap =>
                new CreateClaimsBody(NodeConstants.CreateAssetTypeClaimsRequestType, assetTypeId, claimMap.Keys.ToArray()), claims);
        }

        public async Task<ResponseResult<ClaimsResult>> GetAssetTypeClaims(string assetTypeId, int limit = 20, string next = null)
        {
            var parameters = IovClient.BuildPagingParameters(limit, next);
            var path = _iovClient.BuildPath(NodeConstants.AssetTypesEndPoint, assetTypeId, NodeConstants.ClaimsEndPoint);
            return await _iovClient.ProcessSignedGetRequest<ClaimsResult>(_identity.Id, path, parameters);
        }

        public async Task<ResponseResult<ClaimResult>> GetAssetTypeClaim(string assetTypeId, string claim)
        {
            var path = _iovClient.BuildPath(NodeConstants.AssetTypesEndPoint, assetTypeId, NodeConstants.ClaimsEndPoint, _identity.Crypto.GetHash(claim));
            return await _iovClient.ProcessSignedGetRequest<ClaimResult>(_identity.Id, path);
        }

        public async Task<ResponseResult<CreateEndorsementsResult>> CreateAssetTypeClaimsEndorsements(Endorsements endorsements, params Authorisation[] authorisations)
        {
            return await _iovClient.CreateClaimsEndorsements(endorsements, endorsements.GenerateAssetTypeEndorsementBody, authorisations);
        }

        public async Task<ResponseResult<EndorsementResult>> GetAssetTypeEndorsement(string assetTypeId, string claim, string endorser)
        {
            var path = _iovClient.BuildPath(NodeConstants.AssetTypesEndPoint, assetTypeId, NodeConstants.ClaimsEndPoint, _identity.Crypto.GetHash(claim), NodeConstants.EndorsementsEndPoint, endorser);
            return await _iovClient.ProcessSignedGetRequest<EndorsementResult>(_identity.Id, path);
        }

        public async Task<ResponseResult<CreateClaimsResult>> CreateAssetClaims(string assetTypeId, string assetId, params string[] claims)
        {
            return await _iovClient.CreateClaims(claimMap =>
                new CreateClaimsBody(NodeConstants.CreateAssetClaimsRequestType, assetTypeId, assetId, claimMap.Keys.ToArray()), claims);
        }

        public async Task<ResponseResult<ClaimsResult>> GetAssetClaims(string assetTypeId, string assetId, int limit = 20, string next = null)
        {
            var parameters = IovClient.BuildPagingParameters(limit, next);
            var path = _iovClient.BuildPath(NodeConstants.AssetTypesEndPoint, assetTypeId, NodeConstants.AssetsEndPoint, assetId, NodeConstants.ClaimsEndPoint);
            return await _iovClient.ProcessSignedGetRequest<ClaimsResult>(_identity.Id, path, parameters);
        }

        public async Task<ResponseResult<ClaimResult>> GetAssetClaim(string assetTypeId, string assetId, string claim)
        {
            var path = _iovClient.BuildPath(NodeConstants.AssetTypesEndPoint, assetTypeId, NodeConstants.AssetsEndPoint, assetId, NodeConstants.ClaimsEndPoint, _identity.Crypto.GetHash(claim));
            return await _iovClient.ProcessSignedGetRequest<ClaimResult>(_identity.Id, path);
        }

        public async Task<ResponseResult<CreateEndorsementsResult>> CreateAssetClaimsEndorsements(string assetTypeId, Endorsements endorsements, params Authorisation[] authorisations)
        {
            return await _iovClient.CreateClaimsEndorsements(endorsements, () => endorsements.GenerateAssetEndorsementBody(assetTypeId), authorisations);
        }

        public async Task<ResponseResult<EndorsementResult>> GetAssetEndorsement(string assetTypeId, string assetId, string claim, string endorser)
        {
            var path = _iovClient.BuildPath(NodeConstants.AssetTypesEndPoint, assetTypeId, NodeConstants.AssetsEndPoint, assetId, NodeConstants.ClaimsEndPoint, _identity.Crypto.GetHash(claim), NodeConstants.EndorsementsEndPoint, endorser);
            return await _iovClient.ProcessSignedGetRequest<EndorsementResult>(_identity.Id, path);
        }

        public async Task<ResponseResult<TransactionsResult>> GetTransactions(string assetTypeId, string assetId, int limit = 20, string next = null)
        {
            var path = _iovClient.BuildPath(NodeConstants.AssetTypesEndPoint, assetTypeId, NodeConstants.AssetsEndPoint, assetId, NodeConstants.TransactionsEndPoint);
            var parameters = IovClient.BuildPagingParameters(limit, next);
            return await _iovClient.ProcessSignedGetRequest<TransactionsResult>(_identity.Id, path, parameters);
        }

        public Authorisation GenerateAuthorisation<T>(T body)
        {
            return _iovClient.GenerateAuthorisationHeader(body, _identity);
        }

        private Endorsements CreateEndorsements(string subjectTypeId, string subject)
        {
            return new Endorsements(_iovClient.CreateUniqueId(), subjectTypeId, subject, _identity);
        }
    }
}