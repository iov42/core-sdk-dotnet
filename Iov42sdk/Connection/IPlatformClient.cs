using System;
using System.Numerics;
using System.Threading.Tasks;
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
    public interface IPlatformClient : IDisposable
    {
        /// <summary>
        /// If this is a delegate identity then set the delegator
        /// </summary>
        /// <param name="delegatorId">The delegator for this delegate identity</param>
        void UseDelegator(string delegatorId);

        /// <summary>
        /// Stop using a delegator for the operations
        /// </summary>
        void StopUsingDelegator();

        /// <summary>
        /// Get the status of the node you are connecting to
        /// </summary>
        /// <returns>The node status</returns>
        Task<ResponseResult<HealthStatusResult>> GetHealthStatus();

        /// <summary>
        /// Get the information about the node connected to
        /// </summary>
        /// <returns>The node information</returns>
        Task<ResponseResult<NodeInfo>> GetNodeInfo();

        /// <summary>
        /// Get the status of a request
        /// </summary>
        /// <param name="requestId">The id of the request to check</param>
        /// <returns>The status of the request</returns>
        Task<ResponseResult<RequestStatusResult>> GetRequestStatus(string requestId);

        /// <summary>
        /// Creates a new identity
        /// </summary>
        /// <param name="identity">The identity of the user to create - use IdentityBuilder to create it</param>
        Task<ResponseResult<CreateIdentityResult>> CreateIdentity(IdentityDetails identity);

        /// <summary>
        /// Fetch the details for an identity
        /// </summary>
        /// <param name="identity">The address of the identity to fetch</param>
        /// <returns>The identity details</returns>
        Task<ResponseResult<IdentityResult>> GetIdentity(string identity);

        /// <summary>
        /// Fetch the public key for an identity
        /// </summary>
        /// <param name="identity">The address of the identity to fetch</param>
        /// <returns>The public key details</returns>
        Task<ResponseResult<IdentityPublicKeyResult>> GetIdentityPublicKey(string identity);

        /// <summary>
        /// Add a delegate to the current identity
        /// </summary>
        /// <param name="delegateIdentity">The identity of the delegate</param>
        /// <returns></returns>
        Task<ResponseResult<AddDelegateResult>> AddDelegate(IdentityDetails delegateIdentity);

        /// <summary>
        /// Fetch the delegates for an identity
        /// </summary>
        /// <param name="identityId">The address of the identity to fetch - defaults to current identity if null passed</param>
        /// <returns>The delegates</returns>
        Task<ResponseResult<GetDelegatesResult>> GetIdentityDelegates(string identityId = null);

        /// <summary>
        /// Create a new unique asset type
        /// </summary>
        /// <param name="assetTypeId">The address to use for the asset type</param>
        /// <returns>The new asset type details</returns>
        Task<ResponseResult<CreateAssetTypeResult>> CreateUniqueAssetType(string assetTypeId);

        /// <summary>
        /// Create a new quantifiable asset type
        /// </summary>
        /// <param name="assetTypeId">The address to use for the asset type</param>
        /// <param name="scale">The scale of the quantities, 2 would be 2 decimal places</param>
        /// <returns>The new asset type details</returns>
        Task<ResponseResult<CreateAssetTypeResult>> CreateQuantifiableAssetType(string assetTypeId, int scale);

        /// <summary>
        /// Get the unique asset type details
        /// </summary>
        /// <param name="assetTypeAddress">The address of the asset type</param>
        /// <returns>The unique asset type details</returns>
        Task<ResponseResult<UniqueAssetTypeResult>> GetUniqueAssetType(string assetTypeAddress);

        /// <summary>
        /// Get the quantifiable asset type details
        /// </summary>
        /// <param name="assetTypeAddress">The address of the asset type</param>
        /// <returns>The quantifiable asset type details</returns>
        Task<ResponseResult<QuantifiableAssetTypeResult>> GetQuantifiableAssetType(string assetTypeAddress);

        /// <summary>
        /// Fetches the proof for the given request
        /// </summary>
        /// <param name="requestId">The id of the request</param>
        /// <returns>The proof details</returns>
        Task<ResponseResult<ProofResult>> GetProof(string requestId);

        /// <summary>
        /// Create an instance of a unique asset
        /// </summary>
        /// <param name="address">The address for the instance</param>
        /// <param name="assetTypeAddress">The address of the type of unique asset</param>
        /// <returns>The create asset result</returns>
        Task<ResponseResult<CreateAssetResult>> CreateUniqueAsset(string address, string assetTypeAddress);

        /// <summary>
        /// Create a quantity of a quantifiable asset
        /// </summary>
        /// <param name="address">The address of the new asset</param>
        /// <param name="assetTypeAddress">The address of the type of quantifiable asset</param>
        /// <param name="quantity">The amount to create - bear in mind the scale of the asset type definition. If you have a scale of 2dp and you want to create 100 then pass in 10000 (i.e. 100.00). The default is 0.</param>
        /// <returns>The create asset result</returns>
        Task<ResponseResult<CreateAssetResult>> CreateQuantifiableAccount(string address, string assetTypeAddress, BigInteger quantity = new BigInteger());

        /// <summary>
        /// Adds a quantity to a quantifiable asset
        /// </summary>
        /// <param name="address">The address of the asset account</param>
        /// <param name="assetTypeAddress">The address of the type of quantifiable asset</param>
        /// <param name="quantity">The amount to add - bear in mind the scale of the asset type definition. If you have a scale of 2dp and you want to create 100 then pass in 10000 (i.e. 100.00)</param>
        /// <returns>The update balance result</returns>
        Task<ResponseResult<UpdateBalanceResult>> AddBalance(string address, string assetTypeAddress, BigInteger quantity);

        /// <summary>
        /// Get the details of the unique asset
        /// </summary>
        /// <param name="address">The address of the instance</param>
        /// <param name="assetTypeAddress">The address of the type of unique asset</param>
        /// <returns>The unique asset details</returns>
        Task<ResponseResult<UniqueAssetResult>> GetUniqueAsset(string address, string assetTypeAddress);

        /// <summary>
        /// Get the details (including the quantity) for the quantifiable asset that the caller holds
        /// </summary>
        /// <param name="address"></param>
        /// <param name="assetTypeAddress">The address of the type of quantifiable asset</param>
        /// <returns>The quantifiable asset details</returns>
        Task<ResponseResult<QuantifiableAssetResult>> GetQuantifiableAsset(string address, string assetTypeAddress);

        /// <summary>
        /// Transfer one or more assets
        /// </summary>
        /// <param name="request">The transfers and authorisations</param>
        /// <returns>The transfer result</returns>
        Task<ResponseResult<TransfersResult>> TransferAssets(TransferRequest request);

        /// <summary>
        /// Transfer one or more assets which the current identity owns. If there are other owners involved use the other TransferAssets call
        /// </summary>
        /// <param name="transfers">The transfers</param>
        /// <returns>The transfer result</returns>
        Task<ResponseResult<TransfersResult>> TransferAssets(params SingleTransfer[] transfers);

        /// <summary>
        /// Create an ownership transfer (unique asset)
        /// </summary>
        /// <param name="assetId">The asset id to transfer</param>
        /// <param name="assetTypeId">The type of asset to transder</param>
        /// <param name="fromIdentityId">Who it is transferring from</param>
        /// <param name="toIdentityId">Who it is transferring to</param>
        /// <returns>The transfer</returns>
        SingleTransfer CreateOwnershipTransfer(string assetId, string assetTypeId, string fromIdentityId, string toIdentityId);

        /// <summary>
        /// Create a quantity transfer
        /// </summary>
        /// <param name="fromAssetId">The from account</param>
        /// <param name="toAssetId">The to account</param>
        /// <param name="assetTypeId">The asset type in the account</param>
        /// <param name="quantity">The quantity to transfer</param>
        /// <returns>The transfer</returns>
        SingleTransfer CreateQuantityTransfer(string fromAssetId, string toAssetId, string assetTypeId, BigInteger quantity);

        /// <summary>
        /// Create the claims on the identity
        /// </summary>
        /// <param name="claims">The claims in plaintext</param>
        /// <returns></returns>
        Task<ResponseResult<CreateClaimsResult>> CreateIdentityClaims(params string[] claims);

        /// <summary>
        /// Get the claims for the identity
        /// </summary>
        /// <param name="limit">The number to return (default is 20, max is 50)</param>
        /// <param name="next">The next claim to start with (optional)</param>
        /// <returns></returns>
        Task<ResponseResult<ClaimsResult>> GetIdentityClaims(int limit = 20, string next = null);

        /// <summary>
        /// Get the claims for the identity
        /// </summary>
        /// <param name="identityId">The identity who has the claims</param>
        /// <param name="limit">The number to return (default is 20, max is 50)</param>
        /// <param name="next">The next claim to start with (optional)</param>
        /// <returns></returns>
        Task<ResponseResult<ClaimsResult>> GetIdentityClaims(string identityId, int limit = 20, string next = null);

        /// <summary>
        /// Get the specific claim against an identity, if it exists
        /// </summary>
        /// <param name="claim">The claim to retrieve</param>
        /// <returns></returns>
        Task<ResponseResult<ClaimResult>> GetIdentityClaim(string claim);

        /// <summary>
        /// Get the specific claim against a specified identity, if it exists
        /// </summary>
        /// <param name="identityId">The identity for the id</param>
        /// <param name="claim">The claim to retrieve</param>
        /// <returns></returns>
        Task<ResponseResult<ClaimResult>> GetIdentityClaim(string identityId, string claim);

        /// <summary>
        /// Get the specific endorsement against the specific claim for an identity, if it exists
        /// </summary>
        /// <param name="identity">The identity to check</param>
        /// <param name="claim">The claim plaintext to check</param>
        /// <param name="endorser">The endorser to check</param>
        /// <returns></returns>
        Task<ResponseResult<EndorsementResult>> GetIdentityEndorsement(string identity, string claim, string endorser);

        /// <summary>
        /// Endorses claims on an identity
        /// </summary>
        /// <param name="endorsements">The endorsements</param>
        /// <param name="endorsementBody">The endorsement body</param>
        /// <param name="authorisations">The authorisations for the endorses and claimant</param>
        /// <returns></returns>
        Task<ResponseResult<CreateEndorsementsResult>> CreateIdentityClaimsEndorsements(Endorsements endorsements, EndorsementBody endorsementBody, params Authorisation[] authorisations);

        /// <summary>
        /// Create the claims on the asset type
        /// </summary>
        /// <param name="assetTypeId">The asset type to claim against</param>
        /// <param name="claims">The claims in plaintext</param>
        /// <returns></returns>
        Task<ResponseResult<CreateClaimsResult>> CreateAssetTypeClaims(string assetTypeId, params string[] claims);

        /// <summary>
        /// Get the claims for the asset type
        /// </summary>
        /// <param name="assetTypeId">The asset type to get claims for</param>
        /// <param name="limit">The number to return (default is 20, max is 50)</param>
        /// <param name="next">The next claim to start with (optional)</param>
        /// <returns></returns>
        Task<ResponseResult<ClaimsResult>> GetAssetTypeClaims(string assetTypeId, int limit = 20, string next = null);

        /// <summary>
        /// Get the specific claim against an asset type, if it exists
        /// </summary>
        /// <param name="assetTypeId">The asset type to get claims for</param>
        /// <param name="claim">The claim to retrieve</param>
        /// <returns></returns>
        Task<ResponseResult<ClaimResult>> GetAssetTypeClaim(string assetTypeId, string claim);

        /// <summary>
        /// Endorses claims on an asset type
        /// </summary>
        /// <param name="endorsements">The endorsements</param>
        /// <param name="body">The endorsement body</param>
        /// <param name="authorisations">The authorisations for the endorses and claimant</param>
        /// <returns></returns>
        Task<ResponseResult<CreateEndorsementsResult>> CreateAssetTypeClaimsEndorsements(Endorsements endorsements, EndorsementBody body, params Authorisation[] authorisations);

        /// <summary>
        /// Get the specific endorsement against the specific claim for an asset type, if it exists
        /// </summary>
        /// <param name="assetTypeId">The asset type to check</param>
        /// <param name="claim">The claim plaintext to check</param>
        /// <param name="endorser">The endorser to check</param>
        /// <returns></returns>
        Task<ResponseResult<EndorsementResult>> GetAssetTypeEndorsement(string assetTypeId, string claim, string endorser);

        /// <summary>
        /// Create the claims on the asset
        /// </summary>
        /// <param name="assetTypeId">The asset type to claim against</param>
        /// <param name="assetId">The asset to claim against</param>
        /// <param name="claims">The claims in plaintext</param>
        /// <returns></returns>
        Task<ResponseResult<CreateClaimsResult>> CreateAssetClaims(string assetTypeId, string assetId, params string[] claims);

        /// <summary>
        /// Get the claims for the asset
        /// </summary>
        /// <param name="assetTypeId">The asset type to get claims for</param>
        /// <param name="assetId">The asset to get claims for</param>
        /// <param name="limit">The number to return (default is 20, max is 50)</param>
        /// <param name="next">The next claim to start with (optional)</param>
        /// <returns></returns>
        Task<ResponseResult<ClaimsResult>> GetAssetClaims(string assetTypeId, string assetId, int limit = 20, string next = null);

        /// <summary>
        /// Get the specific claim against an asset, if it exists
        /// </summary>
        /// <param name="assetTypeId">The asset type to get claims for</param>
        /// <param name="assetId">The asset to get claims for</param>
        /// <param name="claim">The claim to retrieve</param>
        /// <returns></returns>
        Task<ResponseResult<ClaimResult>> GetAssetClaim(string assetTypeId, string assetId, string claim);

        /// <summary>
        /// Endorses claims on an asset
        /// </summary>
        /// <param name="endorsements">The endorsements</param>
        /// <param name="body">The endorsement body</param>
        /// <param name="authorisations">The authorisations for the endorses and claimant</param>
        /// <returns></returns>
        Task<ResponseResult<CreateEndorsementsResult>> CreateAssetClaimsEndorsements(Endorsements endorsements, EndorsementBody body, params Authorisation[] authorisations);

        /// <summary>
        /// Get the specific endorsement against the specific claim for an asset, if it exists
        /// </summary>
        /// <param name="assetTypeId">The asset type to check</param>
        /// <param name="assetId">The asset to check</param>
        /// <param name="claim">The claim plaintext to check</param>
        /// <param name="endorser">The endorser to check</param>
        /// <returns></returns>
        Task<ResponseResult<EndorsementResult>> GetAssetEndorsement(string assetTypeId, string assetId, string claim, string endorser);

        /// <summary>
        /// Generate the authentication (used for endorsements)
        /// </summary>
        /// <typeparam name="T">Type of the body to sign</typeparam>
        /// <param name="body">The body to sign</param>
        /// <returns></returns>
        Authorisation GenerateAuthorisation<T>(T body);

        /// <summary>
        /// Create the endorsements container for the identity
        /// </summary>
        /// <param name="identityId">The identity to endorse. If null it will endorse the claim on the current identity</param>
        /// <returns></returns>
        Endorsements CreateIdentityEndorsements(string identityId = null);

        /// <summary>
        /// Get the transactions for a specific asset type
        /// </summary>
        /// <param name="assetTypeId">The asset type to retrieve transactions for</param>
        /// <param name="assetId">The asset id to retrieve transactions for</param>
        /// <param name="limit">The number to return (default is 20, max is 50)</param>
        /// <param name="next">The next transaction to start with (optional)</param>
        /// <returns></returns>
        Task<ResponseResult<TransactionsResult>> GetTransactions(string assetTypeId, string assetId, int limit = 20, string next = null);

        /// <summary>
        /// Create the endorsements container for the asset type
        /// </summary>
        /// <param name="assetTypeId">The asset type to endorse</param>
        /// <returns></returns>
        Endorsements CreateAssetTypeEndorsements(string assetTypeId);

        /// <summary>
        /// Create the endorsements container for the asset
        /// </summary>
        /// <param name="assetTypeId">The asset type</param>
        /// <param name="assetId">The asset</param>
        /// <returns></returns>
        Endorsements CreateAssetEndorsements(string assetTypeId, string assetId);
    }
}