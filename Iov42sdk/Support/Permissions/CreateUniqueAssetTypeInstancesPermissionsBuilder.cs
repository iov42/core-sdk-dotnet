using Iov42sdk.Models.Permissions;

namespace Iov42sdk.Support.Permissions
{
    public class CreateUniqueAssetTypeInstancesPermissionsBuilder
    {
        private InstancePermission _create;
        private InstancePermission _read;
        private InstancePermission _createClaim;
        private InstancePermission _endorseClaim;
        private InstancePermission _readClaim;
        private InstancePermission _readEndorsement;
        private InstancePermission _transfer;

        public CreateUniqueAssetTypeInstancesPermissionsBuilder WithInstanceCreateForEveryone(bool grant)
        {
            _create ??= new InstancePermission();
            _create.SetEveryone(grant);
            return this;
        }

        public CreateUniqueAssetTypeInstancesPermissionsBuilder WithInstanceCreateForTypeOwner(bool grant)
        {
            _create ??= new InstancePermission();
            _create.SetTypeOwner(grant);
            return this;
        }

        public CreateUniqueAssetTypeInstancesPermissionsBuilder WithInstanceCreateForIdentity(string identity, bool grant)
        {
            _create ??= new InstancePermission();
            _create.SetIdentity(identity, grant);
            return this;
        }

        public CreateUniqueAssetTypeInstancesPermissionsBuilder WithInstanceReadForEveryone(bool grant)
        {
            _read ??= new InstancePermission();
            _read.SetEveryone(grant);
            return this;
        }

        public CreateUniqueAssetTypeInstancesPermissionsBuilder WithInstanceReadForTypeOwner(bool grant)
        {
            _read ??= new InstancePermission();
            _read.SetTypeOwner(grant);
            return this;
        }

        public CreateUniqueAssetTypeInstancesPermissionsBuilder WithInstanceReadForInstanceOwner(bool grant)
        {
            _read ??= new InstancePermission();
            _read.SetInstanceOwner(grant);
            return this;
        }

        public CreateUniqueAssetTypeInstancesPermissionsBuilder WithInstanceReadForIdentity(string identity, bool grant)
        {
            _read ??= new InstancePermission();
            _read.SetIdentity(identity, grant);
            return this;
        }

        public CreateUniqueAssetTypeInstancesPermissionsBuilder WithInstanceCreateClaimForEveryone(bool grant)
        {
            _createClaim ??= new InstancePermission();
            _createClaim.SetEveryone(grant);
            return this;
        }

        public CreateUniqueAssetTypeInstancesPermissionsBuilder WithInstanceCreateClaimForInstanceOwner(bool grant)
        {
            _createClaim ??= new InstancePermission();
            _createClaim.SetInstanceOwner(grant);
            return this;
        }

        public CreateUniqueAssetTypeInstancesPermissionsBuilder WithInstanceCreateClaimForTypeOwner(bool grant)
        {
            _createClaim ??= new InstancePermission();
            _createClaim.SetTypeOwner(grant);
            return this;
        }

        public CreateUniqueAssetTypeInstancesPermissionsBuilder WithInstanceCreateClaimForIdentity(string identity, bool grant)
        {
            _createClaim ??= new InstancePermission();
            _createClaim.SetIdentity(identity, grant);
            return this;
        }

        public CreateUniqueAssetTypeInstancesPermissionsBuilder WithInstanceReadClaimForEveryone(bool grant)
        {
            _readClaim ??= new InstancePermission();
            _readClaim.SetEveryone(grant);
            return this;
        }

        public CreateUniqueAssetTypeInstancesPermissionsBuilder WithInstanceReadClaimForInstanceOwner(bool grant)
        {
            _readClaim ??= new InstancePermission();
            _readClaim.SetInstanceOwner(grant);
            return this;
        }

        public CreateUniqueAssetTypeInstancesPermissionsBuilder WithInstanceReadClaimForTypeOwner(bool grant)
        {
            _readClaim ??= new InstancePermission();
            _readClaim.SetTypeOwner(grant);
            return this;
        }

        public CreateUniqueAssetTypeInstancesPermissionsBuilder WithInstanceReadClaimForIdentity(string identity, bool grant)
        {
            _readClaim ??= new InstancePermission();
            _readClaim.SetIdentity(identity, grant);
            return this;
        }

        public CreateUniqueAssetTypeInstancesPermissionsBuilder WithInstanceEndorseClaimForEveryone(bool grant)
        {
            _endorseClaim ??= new InstancePermission();
            _endorseClaim.SetEveryone(grant);
            return this;
        }

        public CreateUniqueAssetTypeInstancesPermissionsBuilder WithInstanceEndorseClaimForInstanceOwner(bool grant)
        {
            _endorseClaim ??= new InstancePermission();
            _endorseClaim.SetInstanceOwner(grant);
            return this;
        }

        public CreateUniqueAssetTypeInstancesPermissionsBuilder WithInstanceEndorseClaimForTypeOwner(bool grant)
        {
            _endorseClaim ??= new InstancePermission();
            _endorseClaim.SetTypeOwner(grant);
            return this;
        }

        public CreateUniqueAssetTypeInstancesPermissionsBuilder WithInstanceEndorseClaimForIdentity(string identity, bool grant)
        {
            _endorseClaim ??= new InstancePermission();
            _endorseClaim.SetIdentity(identity, grant);
            return this;
        }

        public CreateUniqueAssetTypeInstancesPermissionsBuilder WithInstanceReadEndorsementForEveryone(bool grant)
        {
            _readEndorsement ??= new InstancePermission();
            _readEndorsement.SetEveryone(grant);
            return this;
        }

        public CreateUniqueAssetTypeInstancesPermissionsBuilder WithInstanceReadEndorsementForInstanceOwner(bool grant)
        {
            _readEndorsement ??= new InstancePermission();
            _readEndorsement.SetInstanceOwner(grant);
            return this;
        }

        public CreateUniqueAssetTypeInstancesPermissionsBuilder WithInstanceReadEndorsementForTypeOwner(bool grant)
        {
            _readEndorsement ??= new InstancePermission();
            _readEndorsement.SetTypeOwner(grant);
            return this;
        }

        public CreateUniqueAssetTypeInstancesPermissionsBuilder WithInstanceReadEndorsementForIdentity(string identity, bool grant)
        {
            _readEndorsement ??= new InstancePermission();
            _readEndorsement.SetIdentity(identity, grant);
            return this;
        }

        public CreateUniqueAssetTypeInstancesPermissionsBuilder WithInstanceTransferForEveryone(bool grant)
        {
            _transfer ??= new InstancePermission();
            _transfer.SetEveryone(grant);
            return this;
        }

        public CreateUniqueAssetTypeInstancesPermissionsBuilder WithInstanceTransferForInstanceOwner(bool grant)
        {
            _transfer ??= new InstancePermission();
            _transfer.SetInstanceOwner(grant);
            return this;
        }

        public CreateUniqueAssetTypeInstancesPermissionsBuilder WithInstanceTransferForTypeOwner(bool grant)
        {
            _transfer ??= new InstancePermission();
            _transfer.SetTypeOwner(grant);
            return this;
        }

        public CreateUniqueAssetTypeInstancesPermissionsBuilder WithInstanceTransferForIdentity(string identity, bool grant)
        {
            _transfer ??= new InstancePermission();
            _transfer.SetIdentity(identity, grant);
            return this;
        }

        public UniqueAssetTypeInstancesPermissions BuildInstances()
        {
            return new UniqueAssetTypeInstancesPermissions
            {
                Create = _create,
                Read = _read,
                CreateClaim = _createClaim,
                EndorseClaim = _endorseClaim,
                ReadClaim = _readClaim,
                ReadEndorsement = _readEndorsement,
                Transfer = _transfer
            };
        }
    }
}