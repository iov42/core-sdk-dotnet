using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Iov42sdk.Identity;
using Iov42sdk.Models;
using Iov42sdk.Models.CreateClaims;
using Iov42sdk.Models.GetEndorsement;
using Iov42sdk.Models.Headers;
using Microsoft.AspNetCore.Http;

namespace Iov42sdk.Support
{
    internal class IovClient : IDisposable
    {
        private readonly ClientSettings _clientSettings;
        private const string JsonMediaType = "application/json";

        private readonly JsonConversion _json;
        private IdentityDetails _identity;
        private readonly HttpClient _client;
        private string _delegatorId;
        private readonly EventualConsistency _eventualConsistency = new EventualConsistency();

        public IovClient(ClientSettings clientSettings)
        {
            _clientSettings = clientSettings;
            _json = new JsonConversion();
            var httpClientHandler = new HttpClientHandler
            {
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            };
            _client = new HttpClient(httpClientHandler) { BaseAddress = _clientSettings.BaseAddress };
        }

        internal IovClient Init(IdentityDetails identity)
        {
            _identity = identity;
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

        internal async Task<ResponseResult<WriteResult>> ProcessSignedPutRequest(PlatformWriteRequest platformWriteRequest)
        {
            var minifiedPostBody = platformWriteRequest.Body;
            var request = new HttpRequestMessage(HttpMethod.Put, new Uri($"{NodeConstants.PutEndPoint}/{platformWriteRequest.RequestId}", UriKind.Relative))
            {
                Content = new StringContent(minifiedPostBody)
            };
            AddWriteHeaders(platformWriteRequest, request);
            _eventualConsistency.StartWriteOperation();
            var result = await ContactNode<WriteResult>(request);
            _eventualConsistency.EndWriteOperation();
            return result;
        }
        
        internal async Task<ResponseResult<T>> ProcessSignedGetRequest<T>(string url)
        {
            var encodedAuthenticationJsonForGet = BuildSignedGetAuthentication(url);
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_client.BaseAddress, url))
            {
                Headers = {{NodeConstants.Iov42Authentication, encodedAuthenticationJsonForGet}}
            };
            if (_clientSettings.DelayForConsistency)
                await _eventualConsistency.ReadOperation();
            return await ContactNode<T>(request);
        }

        private string BuildSignedGetAuthentication(string url)
        {
            var encodedGetSignature = _identity.Crypto.Sign(url.ToBytes()).ToBase64Url();
            var authentication = _delegatorId != null
                ? new AuthenticationHeader(_identity.Crypto.ProtocolId, _delegatorId, encodedGetSignature, _identity.Id)
                : new AuthenticationHeader(_identity.Crypto.ProtocolId, _identity.Id, encodedGetSignature);

            var authenticationJson = _json.ConvertFrom(authentication);
            var encodedAuthenticationJsonForGet = authenticationJson.ToBase64Url();
            return encodedAuthenticationJsonForGet;
        }

        private void AddWriteHeaders(PlatformWriteRequest platformWriteRequest, HttpRequestMessage request)
        {
            if (platformWriteRequest.AdditionalHeaders != null)
            {
                foreach (var additionalHeader in platformWriteRequest.AdditionalHeaders)
                    request.Headers.Add(additionalHeader.Key, additionalHeader.Value);
            }

            var authorisations = platformWriteRequest.Authorisations ?? new[] { GenerateAuthorisationHeader(platformWriteRequest.Body, _identity) };
            var authorisationJson = _json.ConvertFrom(authorisations);
            request.Headers.Add(NodeConstants.Iov42Authorisations, authorisationJson.ToBase64Url());

            var authentication = platformWriteRequest.Authentication ?? GenerateAuthenticationHeader(_identity, authorisations);
            var authenticationJson = _json.ConvertFrom(authentication);
            request.Headers.Add(NodeConstants.Iov42Authentication, authenticationJson.ToBase64Url());

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(JsonMediaType));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue(JsonMediaType);
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
                request = await BuildRedirectRequest(request);
            }
        }

        private async Task<HttpRequestMessage> BuildRedirectRequest(HttpRequestMessage request)
        {
            // Need to delay the retry - hence this code. Have to clone the request as we can't just resubmit it
            var copy = await CloneHttpRequestMessageAsync(request);
            await Task.Delay(_clientSettings.RedirectDelay);
            return copy;
        }

        public static async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage req)
        {
            var clone = new HttpRequestMessage(req.Method, req.RequestUri);
            var memoryStream = new MemoryStream();
            if (req.Content != null)
            {
                await req.Content.CopyToAsync(memoryStream).ConfigureAwait(false);
                memoryStream.Position = 0;
                clone.Content = new StreamContent(memoryStream);
                foreach (var h in req.Content.Headers)
                    clone.Content.Headers.Add(h.Key, h.Value);
            }
            clone.Version = req.Version;
            foreach (var prop in req.Properties)
                clone.Properties.Add(prop);
            foreach (var header in req.Headers)
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            return clone;
        }

        internal Authorisation GenerateAuthorisationHeader(string body, IdentityDetails identity)
        {
            var authorisedSignature = identity.Crypto.Sign(body.ToBytes());
            var encodedAuthorisedSignature = authorisedSignature.ToBase64Url();
            return _delegatorId != null 
                ? new Authorisation(_identity.Crypto.ProtocolId, _delegatorId, identity.Id, encodedAuthorisedSignature) 
                : new Authorisation(_identity.Crypto.ProtocolId, identity.Id, null, encodedAuthorisedSignature);
        }

        internal AuthenticationHeader GenerateAuthenticationHeader(IdentityDetails authenticationIdentity, Authorisation[] authorisations)
        {
            var fullAuthorisation = string.Join(";", authorisations.Select(x => x.Signature));
            var authenticationSignature = authenticationIdentity.Crypto.Sign(fullAuthorisation.ToBytes());
            var encodedAuthenticationSignature = authenticationSignature.ToBase64Url();
            var authentication = new AuthenticationHeader(authenticationIdentity.Crypto.ProtocolId, authenticationIdentity.Id, encodedAuthenticationSignature);
            return authentication;
        }
        
        internal Dictionary<string, string> GenerateClaimsHeader(Dictionary<string, string> claimMap)
        {
            var json = _json.ConvertFrom(claimMap);
            var encoded = json.ToBase64Url();
            var headers = new Dictionary<string, string> { { NodeConstants.Iov42Claims, encoded } };
            return headers;
        }

        internal async Task<ResponseResult<WriteResult>> CreateClaimsEndorsements(Endorsements endorsements, string requestId, string body,
            params Authorisation[] authorisations)
        {
            var request = new PlatformWriteRequest(requestId, body, authorisations);
            var claimMap = endorsements.CreateClaims ? endorsements.GetClaims() : new Dictionary<string, string>();
            var claimsHeader = GenerateClaimsHeader(claimMap);
            request.WithAdditionalHeaders(claimsHeader);
            return await ProcessSignedPutRequest(request);
        }
        
        internal async Task<ResponseResult<WriteResult>> CreateClaims(Func<Dictionary<string, string>, CreateClaimsBody> createBody,
            string[] claims, Authorisation[] authorisations = null, AuthenticationHeader authentication = null)
        {
            var claimMap = claims.ToDictionary(x => _identity.Crypto.GetHash(x), x => x);
            var headers = GenerateClaimsHeader(claimMap);
            var body = createBody(claimMap);
            var request = new PlatformWriteRequest(body.RequestId, body.Serialize(), authorisations, authentication).WithAdditionalHeaders(headers);
            return await ProcessSignedPutRequest(request);
        }

        internal static string CreateUniqueId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}