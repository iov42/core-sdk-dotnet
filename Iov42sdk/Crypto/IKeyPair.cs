using Iov42sdk.Support;

namespace Iov42sdk.Crypto
{
    public interface IKeyPair
    {
        /// <summary>
        /// A base 64 representation of the private key
        /// </summary>
        string PrivateKeyBase64String { get; }
        /// <summary>
        /// A base 64 representation of the public key
        /// </summary>
        string PublicKeyBase64String { get; }
        /// <summary>
        /// Get the serialized representation of the two keys
        /// </summary>
        /// <returns>The serialized representation of the two keys</returns>
        SerializedKeys Serialize();
        /// <summary>
        /// Convert to Pem representation
        /// </summary>
        /// <returns></returns>
        string ToPem();
    }
}