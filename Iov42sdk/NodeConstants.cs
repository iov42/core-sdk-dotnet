namespace Iov42sdk
{
    public class NodeConstants
    {
        // Headers
        public const string Iov42Authorisations = "X-IOV42-Authorisations";
        public const string Iov42Authentication = "X-IOV42-Authentication";
        public const string Iov42Claims = "X-IOV42-Claims";
        // Endpoints
        public const string IdentitiesEndPoint = "identities";
        public const string AssetTypesEndPoint = "asset-types";
        public const string HealthChecksEndPoint = "healthchecks";
        public const string NodeInfoEndPoint = "node-info";
        public const string ProofsEndPoint = "proofs";
        public const string RequestsEndPoint = "requests";
        public const string AssetsEndPoint = "assets";
        public const string ClaimsEndPoint = "claims";
        public const string EndorsementsEndPoint = "endorsements";
        public const string PublicKeyEndPoint = "public-key";
        public const string TransactionsEndPoint = "transactions";
        public const string DelegatesEndPoint = "delegates";
        public const string PutEndPoint = "requests";
        // Request types
        public const string IssueIdentityRequestType = "IssueIdentityRequest";
        public const string DefineAssetTypeRequestType = "DefineAssetTypeRequest";
        public const string CreateAssetRequestType = "CreateAssetRequest";
        public const string AddQuantityRequestType = "AddQuantityRequest";
        public const string TransfersRequestType = "TransfersRequest";
        public const string AddDelegateRequestType = "AddDelegateRequest";
        public const string CreateIdentityClaimsRequestType = "CreateIdentityClaimsRequest";
        public const string CreateAssetTypeClaimsRequestType = "CreateAssetTypeClaimsRequest";
        public const string CreateAssetClaimsRequestType = "CreateAssetClaimsRequest";
        public const string CreateIdentityEndorsementsRequestType = "CreateIdentityEndorsementsRequest";
        public const string CreateAssetTypeEndorsementsRequestType = "CreateAssetTypeEndorsementsRequest";
        public const string CreateAssetEndorsementsRequestType = "CreateAssetEndorsementsRequest";
    }
}