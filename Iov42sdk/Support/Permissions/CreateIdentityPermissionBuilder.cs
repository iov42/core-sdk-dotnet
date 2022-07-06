using Iov42sdk.Models.Permissions;

namespace Iov42sdk.Support.Permissions
{
    public class CreateIdentityPermissionBuilder
    {
        private string _createIdentity;
        private InstancePermission _createClaim;
        private InstancePermission _readClaim;
        private InstancePermission _endorseClaim;
        private InstancePermission _readEndorsement;

        public CreateIdentityPermissionBuilder WithCreateIdentity(bool grant)
        {
            _createIdentity = GrantOrDeny.Access(grant);
            return this;
        }

        public CreateIdentityPermissionBuilder WithCreateClaimForEveryone(bool grant)
        {
            _createClaim ??= new InstancePermission();
            _createClaim.SetEveryone(grant);
            return this;
        }

        public CreateIdentityPermissionBuilder WithCreateClaimForIdentity(string identity, bool grant)
        {
            _createClaim ??= new InstancePermission();
            _createClaim.SetIdentity(identity, grant);
            return this;
        }

        public CreateIdentityPermissionBuilder WithReadClaimForEveryone(bool grant)
        {
            _readClaim ??= new InstancePermission();
            _readClaim.SetEveryone(grant);
            return this;
        }

        public CreateIdentityPermissionBuilder WithReadClaimForIdentity(string identity, bool grant)
        {
            _readClaim ??= new InstancePermission();
            _readClaim.SetIdentity(identity, grant);
            return this;
        }

        public CreateIdentityPermissionBuilder WithEndorseClaimForEveryone(bool grant)
        {
            _endorseClaim ??= new InstancePermission();
            _endorseClaim.SetEveryone(grant);
            return this;
        }

        public CreateIdentityPermissionBuilder WithEndorseClaimForIdentity(string identity, bool grant)
        {
            _endorseClaim ??= new InstancePermission();
            _endorseClaim.SetIdentity(identity, grant);
            return this;
        }

        public CreateIdentityPermissionBuilder WithReadEndorsementForEveryone(bool grant)
        {
            _readEndorsement ??= new InstancePermission();
            _readEndorsement.SetEveryone(grant);
            return this;
        }

        public CreateIdentityPermissionBuilder WithReadEndorsementForIdentity(string identity, bool grant)
        {
            _readEndorsement ??= new InstancePermission();
            _readEndorsement.SetIdentity(identity, grant);
            return this;
        }

        public IdentityPermissions Build()
        {
            var permission = new IdentityPermissions
            {
                CreateIdentity = _createIdentity,
                CreateClaim = _createClaim,
                ReadClaim = _readClaim,
                EndorseClaim = _endorseClaim,
                ReadEndorsement = _readEndorsement
            };
            return permission;
        }
    }
}