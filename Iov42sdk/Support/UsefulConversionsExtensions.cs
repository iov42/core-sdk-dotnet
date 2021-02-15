using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace Iov42sdk.Support
{
    internal static class UsefulConversionsExtensions
    {
        public static byte[] ToBytes(this string original)
        {
            return Encoding.UTF8.GetBytes(original);
        }

        public static string FromBytes(this byte[] original)
        {
            return Encoding.UTF8.GetString(original);
        }

        public static string ToBase64Url(this byte[] original)
        {
            return WebEncoders.Base64UrlEncode(original);
        }

        public static string ToBase64Url(this string original)
        {
            return original.ToBytes().ToBase64Url();
        }

        public static string FromBase64Url(this string original)
        {
            return original.FromBase64UrlToBytes().FromBytes();
        }

        public static byte[] FromBase64UrlToBytes(this string original)
        {
            return WebEncoders.Base64UrlDecode(original);
        }
    }
}