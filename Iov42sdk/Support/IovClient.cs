using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Iov42sdk.Identity;
using Iov42sdk.Models;
using Iov42sdk.Models.CreateClaims;
using Iov42sdk.Models.CreateEndorsements;
using Iov42sdk.Models.GetEndorsement;
using Iov42sdk.Models.Headers;
using Microsoft.AspNetCore.Http;

namespace Iov42sdk.Support
{
    internal class IovClient : IDisposable
    {
        private const string RetryAfter = "Retry-After";
        private const string JsonMediaType = "application/json";

        private readonly Uri _baseAddress;
        private readonly JsonConversion _json;
        private IdentityDetails _identity;
        private NodeInfo _nodeInfo;
        private string _requestIdRoot;
        private readonly HttpClient _client;
        private string _delegatorId;
        private readonly EventualConsistency _eventualConsistency = new EventualConsistency();

        public IovClient(string baseAddress)
        {
            _baseAddress = new Uri(baseAddress);
            _json = new JsonConversion();
            var httpClientHandler = new HttpClientHandler
            {
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            };
            _client = new HttpClient(httpClientHandler) { BaseAddress = _baseAddress };
        }

        internal IovClient Init(IdentityDetails identity, NodeInfo nodeInfo, string requestIdRoot)
        {
            _identity = identity;
            _nodeInfo = nodeInfo;
            _requestIdRoot = requestIdRoot;
            return this;
        }

        public virtual void Dispose()
        {
            _client?.Dispose();
        }

        internal void WithDelegator(string delegatorId)
        {
            _delegatorId = delegatorId;
        }

        internal async Task<ResponseResult<T>> ProcessSimpleGetRequest<T>(string endPoint)
        {
            var response = await _client.GetAsync(endPoint);
            var result = await response.Content.ReadAsStringAsync();
            var converted = _json.ConvertTo<T>(result);
            return new ResponseResult<T>(converted, response.IsSuccessStatusCode, response.StatusCode, response.ReasonPhrase);
        }

        internal async Task<ResponseResult<TR>> ProcessSignedPutRequest<TB, TR>(TB body, Dictionary<string, string> additionalHeaders = null)
            where TB : PutBody
        {
            return await ProcessSignedPutRequest<TB, TR>(new[] { _identity }, body, additionalHeaders);
        }

        internal async Task<ResponseResult<TR>> ProcessSignedPutRequest<TB, TR>(TB body, Authorisation[] authorisations)
            where TB : PutBody
        {
            return await ProcessSignedPutRequest<TB, TR>(_identity, authorisations, body);
        }

        internal async Task<ResponseResult<TR>> ProcessSignedPutRequest<TB, TR>(IdentityDetails[] identities, TB body, Dictionary<string, string> additionalHeaders = null)
            where TB : PutBody
        {
            var headers = identities.Where(x => x != null).Select(x => GenerateAuthorisationHeader(body, x)).ToArray();
            var authenticationIdentity = identities.FirstOrDefault();
            return await ProcessSignedPutRequest<TB, TR>(authenticationIdentity, headers, body, additionalHeaders);
        }

        internal async Task<ResponseResult<TR>> ProcessSignedPutRequest<TB, TR>(IdentityDetails authenticationIdentity, Authorisation[] authorisations, TB body, Dictionary<string, string> additionalHeaders = null)
            where TB : PutBody
        {
            var minifiedPostBody = _json.ConvertFrom(body, true);
            var request = new HttpRequestMessage(HttpMethod.Put, new Uri($"{NodeConstants.PutEndPoint}/{body.RequestId}", UriKind.Relative))
            {
                Content = new StringContent(minifiedPostBody)
            };
            if (additionalHeaders != null)
            {
                foreach (var additionalHeader in additionalHeaders)
                    request.Headers.Add(additionalHeader.Key, additionalHeader.Value);
            }
            var fullAuthorisation = string.Join(";", authorisations.Select(x => x.Signature));
            if (!request.Headers.Contains(NodeConstants.Iov42Authorisations))
            {
                var authorisationJson = _json.ConvertFrom(authorisations);
                request.Headers.Add(NodeConstants.Iov42Authorisations, authorisationJson.ToBase64Url());
            }

            if (!request.Headers.Contains(NodeConstants.Iov42Authentication))
            {
                var authenticationSignature = authenticationIdentity.Crypto.Sign(fullAuthorisation.ToBytes());
                var encodedAuthenticationSignature = authenticationSignature.ToBase64Url();
                var authentication = new AuthenticationHeader(authenticationIdentity.Crypto.ProtocolId, authenticationIdentity.Id, encodedAuthenticationSignature);
                var authenticationJson = _json.ConvertFrom(authentication);
                request.Headers.Add(NodeConstants.Iov42Authentication, authenticationJson.ToBase64Url());
            }

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(JsonMediaType));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue(JsonMediaType);
            _eventualConsistency.StartWriteOperation();
            var result = await ContactNode<TR>(request);
            _eventualConsistency.EndWriteOperation();
            return result;
        }

        internal async Task<ResponseResult<T>> ProcessSignedGetRequest<T>(string identity, string path, string parameters = null)
        {
            var requestId = CreateUniqueId();
            var url = $"{path}?requestId={requestId}&nodeId={_nodeInfo.NodeId}";
            if (!string.IsNullOrEmpty(parameters))
                url += parameters;
            var encodedGetSignature = _identity.Crypto.Sign(url.ToBytes()).ToBase64Url();
            var authentication = new AuthenticationHeader(_identity.Crypto.ProtocolId, identity, encodedGetSignature);
            var authenticationJson = _json.ConvertFrom(authentication);
            var encodedAuthenticationJsonForGet = authenticationJson.ToBase64Url();
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_client.BaseAddress, url))
            {
                Headers = {{NodeConstants.Iov42Authentication, encodedAuthenticationJsonForGet}}
            };

            await _eventualConsistency.ReadOperation();
            return await ContactNode<T>(request);
        }

        private async Task<ResponseResult<T>> ContactNode<T>(HttpRequestMessage request)
        {
            while (true)
            {
                var response = await _client.SendAsync(request);
                var statusCode = (int)response.StatusCode;
                if (statusCode < StatusCodes.Status300MultipleChoices || statusCode > StatusCodes.Status308PermanentRedirect)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var converted = _json.ConvertTo<T>(result);
                    return new ResponseResult<T>(converted, response.IsSuccessStatusCode, response.StatusCode, response.ReasonPhrase);
                }
                request = await BuildRedirectRequest(request, response);
            }
        }

        private static async Task<HttpRequestMessage> BuildRedirectRequest(HttpRequestMessage request, HttpResponseMessage response)
        {
            var redirectUri = response.Headers.Location;
            if (!redirectUri.IsAbsoluteUri)
                redirectUri = new Uri(request.RequestUri.GetLeftPart(UriPartial.Authority) + redirectUri);
            var redirect = new HttpRequestMessage(HttpMethod.Get, redirectUri);
            if (!response.Headers.TryGetValues(RetryAfter, out var values)) 
                return redirect;
            var retry = values as string[] ?? values.ToArray();
            if (retry.Length != 1) 
                return redirect;
            var millisecondsDelay = Convert.ToInt32(retry.ElementAt(0)) * 1000;
            await Task.Delay(millisecondsDelay);
            return redirect;
        }

        internal Authorisation GenerateAuthorisationHeader<T>(T body, IdentityDetails identity)
        {
            var minifiedPostBody = _json.ConvertFrom(body, true);
            var authorisedSignature = identity.Crypto.Sign(minifiedPostBody.ToBytes());
            var encodedAuthorisedSignature = authorisedSignature.ToBase64Url();
            return _delegatorId != null 
                ? new Authorisation(_identity.Crypto.ProtocolId, _delegatorId, identity.Id, encodedAuthorisedSignature) 
                : new Authorisation(_identity.Crypto.ProtocolId, identity.Id, null, encodedAuthorisedSignature);
        }

        internal string BuildPath(params string[] sections)
        {
            var parts = string.Join("/", sections);
            return $"{_baseAddress.AbsolutePath}{parts}";
        }

        internal static string BuildPagingParameters(int limit, string next)
        {
            var parameters = "";
            if (!string.IsNullOrEmpty(next))
                parameters += $"&next={next}";
            if (limit != -1)
                parameters += $"&limit={limit}";
            return parameters;
        }

        private Dictionary<string, string> GenerateClaimsHeader(Dictionary<string, string> claimMap)
        {
            var json = _json.ConvertFrom(claimMap);
            var encoded = json.ToBase64Url();
            var headers = new Dictionary<string, string> { { NodeConstants.Iov42Claims, encoded } };
            return headers;
        }

        internal async Task<ResponseResult<CreateEndorsementsResult>> CreateClaimsEndorsements(Endorsements endorsements, Func<EndorsementBody> createBody,
            params Authorisation[] authorisations)
        {
            var claimMap = endorsements.GetClaims();
            var claimsHeader = GenerateClaimsHeader(claimMap);
            var body = createBody();
            return await ProcessSignedPutRequest<EndorsementBody, CreateEndorsementsResult>(_identity, authorisations, body, claimsHeader);
        }

        public async Task<ResponseResult<CreateClaimsResult>> CreateClaims(Func<Dictionary<string, string>, CreateClaimsBody> createBody,
            params string[] claims)
        {
            var claimMap = claims.ToDictionary(x => _identity.Crypto.GetHash(x), x => x);
            var headers = GenerateClaimsHeader(claimMap);
            var body = createBody(claimMap);
            return await ProcessSignedPutRequest<CreateClaimsBody, CreateClaimsResult>(body, headers);
        }

        internal string CreateUniqueId()
        {
            var id = Guid.NewGuid().ToString();
            return _requestIdRoot != null ? $"{_requestIdRoot}-{id}" : id;
        }
    }
}