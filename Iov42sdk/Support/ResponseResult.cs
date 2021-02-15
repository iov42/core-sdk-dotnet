using System.Net;

namespace Iov42sdk.Support
{
    public class ResponseResult<T>
    {
        public HttpStatusCode ResponseCode { get; }
        public string Reason { get; }
        public T Value { get; }
        public bool Success { get; }

        public ResponseResult(T value, bool success, HttpStatusCode responseCode, string reason)
        {
            ResponseCode = responseCode;
            Reason = reason;
            Success = success;
            Value = value;
        }
    }
}
