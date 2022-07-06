using Iov42sdk.Models.Permissions;

namespace Iov42sdk.Support.Permissions
{
    public class CreateQuantifiableAssetTypeInstancesPermissionsBuilder
    {
        private InstancePermission _create;
        private InstancePermission _read;
        private InstancePermission _createClaim;
        private InstancePermission _endorseClaim;
        private InstancePermission _readClaim;
        private InstancePermission _readEndorsement;
        private InstancePermission _transfer;
        private InstancePermission _addQuantity;

        public CreateQuantifiableAssetTypeInstancesPermissionsBuilder WithInstanceCreateForEveryone(bool grant)
        {
            _create ??= new InstancePermission();
            _create.SetEveryone(grant);
            return this;
        }

        public CreateQuantifiableAssetTypeInstancesPermissionsBuilder WithInstanceCreateForTypeOwner(bool grant)
        {
            _create ??= new InstancePermission();
            _create.SetTypeOwner(grant);
            return this;
        }

        public CreateQuantifiableAssetTypeInstancesPermissionsBuilder WithInstanceReadForEveryone(bool grant)
        {
            _read ??= new InstancePermission();
            _read.SetEveryone(grant);
            return this;
        }

        public CreateQuantifiableAssetTypeInstancesPermissionsBuilder WithInstanceReadForTypeOwner(bool grant)
        {
            _read ??= new InstancePermission();
            _read.SetTypeOwner(grant);
            return this;
        }

        public CreateQuantifiableAssetTypeInstancesPermissionsBuilder WithInstanceReadForInstanceOwner(bool grant)
        {
            _read ??= new InstancePermission();
            _read.SetInstanceOwner(grant);
            return this;
        }

        public CreateQuantifiableAssetTypeInstancesPermissionsBuilder WithInstanceReadForIdentity(string identity, bool grant)
        {
            _read ??= new InstancePermission();
            _read.SetIdentity(identity, grant);
            return this;
        }

        public CreateQuantifiableAssetTypeInstancesPermissionsBuilder WithInstanceCreateClaimForEveryone(bool grant)
        {
            _createClaim ??= new InstancePermission();
            _createClaim.SetEveryone(grant);
            return this;
        }

        public CreateQuantifiableAssetTypeInstancesPermissionsBuilder WithInstanceCreateClaimForInstanceOwner(bool grant)
        {
            _createClaim ??= new InstancePermission();
            _createClaim.SetInstanceOwner(grant);
            return this;
        }

        public CreateQuantifiableAssetTypeInstancesPermissionsBuilder WithInstanceCreateClaimForTypeOwner(bool grant)
        {
            _createClaim ??= new InstancePermission();
            _createClaim.SetTypeOwner(grant);
            return this;
        }

        public CreateQuantifiableAssetTypeInstancesPermissionsBuilder WithInstanceCreateClaimForIdentity(string identity, bool grant)
        {
            _createClaim ??= new InstancePermission();
            _createClaim.SetIdentity(identity, grant);
            return this;
        }

        public CreateQuantifiableAssetTypeInstancesPermissionsBuilder WithInstanceReadClaimForEveryone(bool grant)
        {
            _readClaim ??= new InstancePermission();
            _readClaim.SetEveryone(grant);
            return this;
        }

        public CreateQuantifiableAssetTypeInstancesPermissionsBuilder WithInstanceReadClaimForInstanceOwner(bool grant)
        {
            _readClaim ??= new InstancePermission();
            _readClaim.SetInstanceOwner(grant);
            return this;
        }

        public CreateQuantifiableAssetTypeInstancesPermissionsBuilder WithInstanceReadClaimForTypeOwner(bool grant)
        {
            _readClaim ??= new InstancePermission();
            _readClaim.SetTypeOwner(grant);
            return this;
        }

        public CreateQuantifiableAssetTypeInstancesPermissionsBuilder WithInstanceReadClaimForIdentity(string identity, bool grant)
        {
            _readClaim ??= new InstancePermission();
            _readClaim.SetIdentity(identity, grant);
            return this;
        }

        public CreateQuantifiableAssetTypeInstancesPermissionsBuilder WithInstanceEndorseClaimForEveryone(bool grant)
        {
            _endorseClaim ??= new InstancePermission();
            _endorseClaim.SetEveryone(grant);
            return this;
        }

        public CreateQuantifiableAssetTypeInstancesPermissionsBuilder WithInstanceEndorseClaimForInstanceOwner(bool grant)
        {
            _endorseClaim ??= new InstancePermission();
            _endorseClaim.SetInstanceOwner(grant);
            return this;
        }

        public CreateQuantifiableAssetTypeInstancesPermissionsBuilder WithInstanceEndorseClaimForTypeOwner(bool grant)
        {
            _endorseClaim ??= new InstancePermission();
            _endorseClaim.SetTypeOwner(grant);
            return this;
        }

        public CreateQuantifiableAssetTypeInstancesPermissionsBuilder WithInstanceEndorseClaimForIdentity(string identity, bool grant)
        {
            _endorseClaim ??= new InstancePermission();
            _endorseClaim.SetIdentity(identity, grant);
            return this;
        }

        public CreateQuantifiableAssetTypeInstancesPermissionsBuilder WithInstanceReadEndorsementForEveryone(bool grant)
        {
            _readEndorsement ??= new InstancePermission();
            _readEndorsement.SetEveryone(grant);
            return this;
        }

        public CreateQuantifiableAssetTypeInstancesPermissionsBuilder WithInstanceReadEndorsementForInstanceOwner(bool grant)
        {
            _readEndorsement ??= new InstancePermission();
            _readEndorsement.SetInstanceOwner(grant);
            return this;
        }

        public CreateQuantifiableAssetTypeInstancesPermissionsBuilder WithInstanceReadEndorsementForTypeOwner(bool grant)
        {
            _readEndorsement ??= new InstancePermission();
            _readEndorsement.SetTypeOwner(grant);
            return this;
        }

        public CreateQuantifiableAssetTypeInstancesPermissionsBuilder WithInstanceReadEndorsementForIdentity(string identity, bool grant)
        {
            _readEndorsement ??= new InstancePermission();
            _readEndorsement.SetIdentity(identity, grant);
            return this;
        }

        public CreateQuantifiableAssetTypeInstancesPermissionsBuilder WithInstanceTransferForEveryone(bool grant)
        {
            _transfer ??= new InstancePermission();
            _transfer.SetEveryone(grant);
            return this;
        }

        public CreateQuantifiableAssetTypeInstancesPermissionsBuilder WithInstanceTransferForInstanceOwner(bool grant)
        {
            _transfer ??= new InstancePermission();
            _transfer.SetInstanceOwner(grant);
            return this;
        }

        public CreateQuantifiableAssetTypeInstancesPermissionsBuilder WithInstanceTransferForTypeOwner(bool grant)
        {
            _transfer ??= new InstancePermission();
            _transfer.SetTypeOwner(grant);
            return this;
        }

        public CreateQuantifiableAssetTypeInstancesPermissionsBuilder WithInstanceTransferForIdentity(string identity, bool grant)
        {
            _transfer ??= new InstancePermission();
            _transfer.SetIdentity(identity, grant);
            return this;
        }

        public CreateQuantifiableAssetTypeInstancesPermissionsBuilder WithInstanceAddQuantityForEveryone(bool grant)
        {
            _addQuantity ??= new InstancePermission();
            _addQuantity.SetEveryone(grant);
            return this;
        }

        public CreateQuantifiableAssetTypeInstancesPermissionsBuilder WithInstanceAddQuantityForInstanceOwner(bool grant)
        {
            _addQuantity ??= new InstancePermission();
            _addQuantity.SetInstanceOwner(grant);
            return this;
        }

        public CreateQuantifiableAssetTypeInstancesPermissionsBuilder WithInstanceAddQuantityForTypeOwner(bool grant)
        {
            _addQuantity ??= new InstancePermission();
            _addQuantity.SetTypeOwner(grant);
            return this;
        }

        public CreateQuantifiableAssetTypeInstancesPermissionsBuilder WithInstanceAddQuantityForIdentity(string identity, bool grant)
        {
            _addQuantity ??= new InstancePermission();
            _addQuantity.SetIdentity(identity, grant);
            return this;
        }

        public QuantifiableAssetTypeInstancesPermissions BuildInstances()
        {
            return new QuantifiableAssetTypeInstancesPermissions
            {
                Create = _create,
                Read = _read,
                CreateClaim = _createClaim,
                EndorseClaim = _endorseClaim,
                ReadClaim = _readClaim,
                ReadEndorsement = _readEndorsement,
                Transfer = _transfer,
                AddQuantity = _addQuantity
            };
        }
    }
}