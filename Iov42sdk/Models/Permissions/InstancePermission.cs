using System.Collections.Generic;

namespace Iov42sdk.Models.Permissions
{
    public class InstancePermission : Dictionary<string, string>
    {
        public const string InstanceOwner = "InstanceOwner";
        public const string TypeOwner = "TypeOwner";
        public const string Everyone = "Everyone";

        public void SetIdentity(string identityId, bool grant)
        {
            this[Identity(identityId)] = GrantOrDeny.Access(grant);
        }

        public void SetEveryone(bool grant)
        {
            this[Everyone] = GrantOrDeny.Access(grant);
        }

        public void SetTypeOwner(bool grant)
        {
            this[TypeOwner] = GrantOrDeny.Access(grant);
        }

        public void SetInstanceOwner(bool grant)
        {
            this[InstanceOwner] = GrantOrDeny.Access(grant);
        }

        public static string Identity(string identityId)
        {
            return $"Identity({identityId})";
        }
    }
}