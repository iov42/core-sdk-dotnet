namespace Iov42sdk.Models.Permissions
{
    public class GrantOrDeny
    {
        public static readonly string Grant = "Grant";
        public static readonly string Deny = "Deny";

        public static string Access(bool grant)
        {
            return grant ? Grant : Deny;
        }
    }
}