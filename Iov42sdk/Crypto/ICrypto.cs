using Iov42sdk.Support;

namespace Iov42sdk.Crypto
{
    public interface ICrypto
    {
        /// <summary>
        /// Returns the protocol id used by this crypto instance
        /// </summary>
        string ProtocolId { get; }
        /// <summary>
        /// Returns the keypair for the instance
        /// </summary>
        IKeyPair Pair { get; }
        /// <summary>
        /// Sign the data using the private key
        /// </summary>
        /// <param name="data">The data to sign</param>
        /// <returns></returns>
        byte[] Sign(byte[] data);
        /// <summary>
        /// Verify the signature matches the data
        /// </summary>
        /// <param name="pair">The key pair that signed it - it will use the public key</param>
        /// <param name="base64Signature">The base64 signature string</param>
        /// <param name="original">The original bytes</param>
        /// <returns></returns>
        bool VerifySignature(IKeyPair pair, string base64Signature, byte[] original);
        /// <summary>
        /// Get the hash of the original string
        /// </summary>
        /// <param name="original">The string to hash</param>
        /// <returns></returns>
        string GetHash(string original);
        /// <summary>
        /// Deserialize the key pair
        /// </summary>
        /// <param name="keys">The serialized keys</param>
        /// <returns>The deserialized keys</returns>
        IKeyPair FromSerialized(SerializedKeys keys);
    }
}