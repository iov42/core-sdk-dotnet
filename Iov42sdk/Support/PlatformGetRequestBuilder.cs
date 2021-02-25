using System;

namespace Iov42sdk.Support
{
    public class PlatformGetRequestBuilder
    {
        private readonly Uri _baseAddress;
        private readonly string _nodeId;

        public PlatformGetRequestBuilder(Uri baseAddress, string nodeId)
        {
            _baseAddress = baseAddress;
            _nodeId = nodeId;
        }

        public PlatformGetRequest Create(params string[] sections)
        {
            return new PlatformGetRequest(_nodeId, _baseAddress, sections);
        }

        public PlatformGetRequest CreateUnsigned(params string[] sections)
        {
            return new PlatformGetRequest(_baseAddress, sections);
        }
    }
}