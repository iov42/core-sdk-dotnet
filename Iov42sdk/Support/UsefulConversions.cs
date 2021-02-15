namespace Iov42sdk.Support
{
    public class UsefulConversions
    {
        public static byte[] ToBytes(string original)
        {
            return original.ToBytes();
        }

        public static string FromBytes(byte[] original)
        {
            return original.FromBytes();
        }

        public static string ToBase64Url(byte[] original)
        {
            return original.ToBase64Url();
        }

        public static string ToBase64Url(string original)
        {
            return original.ToBase64Url();
        }

        public static string FromBase64Url(string original)
        {
            return original.FromBase64Url();
        }

        public static byte[] FromBase64UrlToBytes(string original)
        {
            return original.FromBase64UrlToBytes();
        }
    }
}