using System;

namespace Iov42sdk.Support
{
    public class PlatformGetRequest
    {
        private readonly string _nodeId;
        private readonly string _path;
        private string _parameters;
        private readonly string _requestId;

        internal PlatformGetRequest(string nodeId, Uri baseAddress, params string[] sections)
        {
            _nodeId = nodeId;
            var parts = string.Join("/", sections);
            _path = $"{baseAddress.AbsolutePath}{parts}";
            _requestId = IovClient.CreateUniqueId();
        }

        internal PlatformGetRequest(Uri baseAddress, params string[] sections)
        {
            var parts = string.Join("/", sections);
            _path = $"{baseAddress.AbsolutePath}{parts}";
            _requestId = IovClient.CreateUniqueId();
        }

        public PlatformGetRequest WithPagingParameters(int limit, string next)
        {
            var parameters = "";
            if (!string.IsNullOrEmpty(next))
                parameters += $"&next={next}";
            if (limit != -1)
                parameters += $"&limit={limit}";
            _parameters = parameters;
            return this;
        }

        public string Path 
        {
            get
            {
                var url = !string.IsNullOrEmpty(_nodeId) ? $"{_path}?requestId={_requestId}&nodeId={_nodeId}" : _path;
                if (!string.IsNullOrEmpty(_parameters))
                    url += _parameters;
                return url;
            }
        }

    }
}