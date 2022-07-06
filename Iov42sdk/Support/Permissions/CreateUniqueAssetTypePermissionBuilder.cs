using Iov42sdk.Models.Permissions;

namespace Iov42sdk.Support.Permissions
{
    public class CreateUniqueAssetTypePermissionBuilder
    {
        private InstancePermission _read;
        private InstancePermission _createClaim;
        private InstancePermission _readClaim;
        private InstancePermission _endorseClaim;
        private InstancePermission _readEndorsement;
        private UniqueAssetTypeInstancesPermissions _instances;

        public CreateUniqueAssetTypePermissionBuilder WithReadForEveryone(bool grant)
        {
            _read ??= new InstancePermission();
            _read.SetEveryone(grant);
            return this;
        }

        public CreateUniqueAssetTypePermissionBuilder WithReadForTypeOwner(bool grant)
        {
            _read ??= new InstancePermission();
            _read.SetTypeOwner(grant);
            return this;
        }

        public CreateUniqueAssetTypePermissionBuilder WithReadForIdentity(string identity, bool grant)
        {
            _read ??= new InstancePermission();
            _read.SetIdentity(identity, grant);
            return this;
        }

        public CreateUniqueAssetTypePermissionBuilder WithCreateClaimForEveryone(bool grant)
        {
            _createClaim ??= new InstancePermission();
            _createClaim.SetEveryone(grant);
            return this;
        }

        public CreateUniqueAssetTypePermissionBuilder WithCreateClaimForInstanceOwner(bool grant)
        {
            _createClaim ??= new InstancePermission();
            _createClaim.SetInstanceOwner(grant);
            return this;
        }

        public CreateUniqueAssetTypePermissionBuilder WithCreateClaimForIdentity(string identity, bool grant)
        {
            _createClaim ??= new InstancePermission();
            _createClaim.SetIdentity(identity, grant);
            return this;
        }

        public CreateUniqueAssetTypePermissionBuilder WithReadClaimForEveryone(bool grant)
        {
            _readClaim ??= new InstancePermission();
            _readClaim.SetEveryone(grant);
            return this;
        }

        public CreateUniqueAssetTypePermissionBuilder WithReadClaimForInstanceOwner(bool grant)
        {
            _readClaim ??= new InstancePermission();
            _readClaim.SetInstanceOwner(grant);
            return this;
        }

        public CreateUniqueAssetTypePermissionBuilder WithReadClaimForIdentity(string identity, bool grant)
        {
            _readClaim ??= new InstancePermission();
            _readClaim.SetIdentity(identity, grant);
            return this;
        }

        public CreateUniqueAssetTypePermissionBuilder WithEndorseClaimForEveryone(bool grant)
        {
            _endorseClaim ??= new InstancePermission();
            _endorseClaim.SetEveryone(grant);
            return this;
        }

        public CreateUniqueAssetTypePermissionBuilder WithEndorseClaimForInstanceOwner(bool grant)
        {
            _endorseClaim ??= new InstancePermission();
            _endorseClaim.SetInstanceOwner(grant);
            return this;
        }

        public CreateUniqueAssetTypePermissionBuilder WithEndorseClaimForIdentity(string identity, bool grant)
        {
            _endorseClaim ??= new InstancePermission();
            _endorseClaim.SetIdentity(identity, grant);
            return this;
        }

        public CreateUniqueAssetTypePermissionBuilder WithReadEndorsementForEveryone(bool grant)
        {
            _readEndorsement ??= new InstancePermission();
            _readEndorsement.SetEveryone(grant);
            return this;
        }

        public CreateUniqueAssetTypePermissionBuilder WithReadEndorsementForInstanceOwner(bool grant)
        {
            _readEndorsement ??= new InstancePermission();
            _readEndorsement.SetInstanceOwner(grant);
            return this;
        }

        public CreateUniqueAssetTypePermissionBuilder WithReadEndorsementForIdentity(string identity, bool grant)
        {
            _readEndorsement ??= new InstancePermission();
            _readEndorsement.SetIdentity(identity, grant);
            return this;
        }

        public CreateUniqueAssetTypePermissionBuilder WithInstancePermission(UniqueAssetTypeInstancesPermissions permissions)
        {
            _instances = permissions;
            return this;
        }

        public UniqueAssetTypePermissions Build()
        {
            var permission = new UniqueAssetTypePermissions
            {
                Read = _read,
                CreateClaim = _createClaim,
                ReadClaim = _readClaim,
                EndorseClaim = _endorseClaim,
                ReadEndorsement = _readEndorsement,
                Instances = _instances
            };
            return permission;
        }
    }
}