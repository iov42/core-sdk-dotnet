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
using Iov42sdk.Models.GetEndorsement;
using Iov42sdk.Models.Headers;
using Microsoft.AspNetCore.Http;

namespace Iov42sdk.Support
{
    internal class IovClient : IDisposable
    {
        private const string RetryAfter = "Retry-After";
        private const string JsonMediaType = "application/json";

        private readonly JsonConversion _json;
        private IdentityDetails _identity;
        private readonly HttpClient _client;
        private string _delegatorId;
        private readonly EventualConsistency _eventualConsistency = new EventualConsistency();

        public IovClient(Uri baseAddress)
        {
            _json = new JsonConversion();
            var httpClientHandler = new HttpClientHandler
            {
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            };
            _client = new HttpClient(httpClientHandler) { BaseAddress = baseAddress };
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
            await _eventualConsistency.ReadOperation();
            return await ContactNode<T>(request);
        }

        private string BuildSignedGetAuthentication(string url)
        {
            var encodedGetSignature = _identity.Crypto.Sign(url.ToBytes()).ToBase64Url();
            var authentication = new AuthenticationHeader(_identity.Crypto.ProtocolId, _identity.Id, encodedGetSignature);
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
            var claimMap = endorsements.GetClaims();
            var claimsHeader = GenerateClaimsHeader(claimMap);
            var request = new PlatformWriteRequest(requestId, body, authorisations).WithAdditionalHeaders(claimsHeader);
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