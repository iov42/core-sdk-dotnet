using Iov42sdk.Models.Permissions;

namespace Iov42sdk.Support.Permissions
{
    public class CreateQuantifiableAssetTypePermissionBuilder
    {
        private InstancePermission _read;
        private InstancePermission _createClaim;
        private InstancePermission _readClaim;
        private InstancePermission _endorseClaim;
        private InstancePermission _readEndorsement;
        private QuantifiableAssetTypeInstancesPermissions _instances;

        public CreateQuantifiableAssetTypePermissionBuilder WithReadForEveryone(bool grant)
        {
            _read ??= new InstancePermission();
            _read.SetEveryone(grant);
            return this;
        }

        public CreateQuantifiableAssetTypePermissionBuilder WithReadForTypeOwner(bool grant)
        {
            _read ??= new InstancePermission();
            _read.SetTypeOwner(grant);
            return this;
        }

        public CreateQuantifiableAssetTypePermissionBuilder WithReadForIdentity(string identity, bool grant)
        {
            _read ??= new InstancePermission();
            _read.SetIdentity(identity, grant);
            return this;
        }

        public CreateQuantifiableAssetTypePermissionBuilder WithCreateClaimForEveryone(bool grant)
        {
            _createClaim ??= new InstancePermission();
            _createClaim.SetEveryone(grant);
            return this;
        }

        public CreateQuantifiableAssetTypePermissionBuilder WithCreateClaimForInstanceOwner(bool grant)
        {
            _createClaim ??= new InstancePermission();
            _createClaim.SetInstanceOwner(grant);
            return this;
        }

        public CreateQuantifiableAssetTypePermissionBuilder WithCreateClaimForIdentity(string identity, bool grant)
        {
            _createClaim ??= new InstancePermission();
            _createClaim.SetIdentity(identity, grant);
            return this;
        }

        public CreateQuantifiableAssetTypePermissionBuilder WithReadClaimForEveryone(bool grant)
        {
            _readClaim ??= new InstancePermission();
            _readClaim.SetEveryone(grant);
            return this;
        }

        public CreateQuantifiableAssetTypePermissionBuilder WithReadClaimForInstanceOwner(bool grant)
        {
            _readClaim ??= new InstancePermission();
            _readClaim.SetInstanceOwner(grant);
            return this;
        }

        public CreateQuantifiableAssetTypePermissionBuilder WithReadClaimForIdentity(string identity, bool grant)
        {
            _readClaim ??= new InstancePermission();
            _readClaim.SetIdentity(identity, grant);
            return this;
        }

        public CreateQuantifiableAssetTypePermissionBuilder WithEndorseClaimForEveryone(bool grant)
        {
            _endorseClaim ??= new InstancePermission();
            _endorseClaim.SetEveryone(grant);
            return this;
        }

        public CreateQuantifiableAssetTypePermissionBuilder WithEndorseClaimForInstanceOwner(bool grant)
        {
            _endorseClaim ??= new InstancePermission();
            _endorseClaim.SetInstanceOwner(grant);
            return this;
        }

        public CreateQuantifiableAssetTypePermissionBuilder WithEndorseClaimForIdentity(string identity, bool grant)
        {
            _endorseClaim ??= new InstancePermission();
            _endorseClaim.SetIdentity(identity, grant);
            return this;
        }

        public CreateQuantifiableAssetTypePermissionBuilder WithReadEndorsementForEveryone(bool grant)
        {
            _readEndorsement ??= new InstancePermission();
            _readEndorsement.SetEveryone(grant);
            return this;
        }

        public CreateQuantifiableAssetTypePermissionBuilder WithReadEndorsementForInstanceOwner(bool grant)
        {
            _readEndorsement ??= new InstancePermission();
            _readEndorsement.SetInstanceOwner(grant);
            return this;
        }

        public CreateQuantifiableAssetTypePermissionBuilder WithReadEndorsementForIdentity(string identity, bool grant)
        {
            _readEndorsement ??= new InstancePermission();
            _readEndorsement.SetIdentity(identity, grant);
            return this;
        }

        public CreateQuantifiableAssetTypePermissionBuilder WithInstancePermission(QuantifiableAssetTypeInstancesPermissions permissions)
        {
            _instances = permissions;
            return this;
        }

        public QuantifiableAssetTypePermissions Build()
        {
            var permission = new QuantifiableAssetTypePermissions
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