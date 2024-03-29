﻿using System.Collections.Generic;
using System.Linq;
using Iov42sdk.Crypto;
using Iov42sdk.Identity;
using Iov42sdk.Models.CreateEndorsements;
using Iov42sdk.Support;

namespace Iov42sdk.Models.GetEndorsement
{
    public class Endorsements
    {
        private readonly List<Endorsement> _endorsements = new List<Endorsement>();
        private readonly IdentityDetails _endorser;

        public Endorsements(string requestId, string subjectTypeId, string subjectId, IdentityDetails endorser)
        {
            SubjectId = subjectId;
            SubjectTypeId = subjectTypeId;
            RequestId = requestId;
            _endorser = endorser;
            EndorserId = endorser.Id;
        }

        public Endorsements(string requestId, string subjectId, string endorserId)
            : this(requestId, null, subjectId, endorserId)
        {
        }

        public Endorsements(string requestId, string subjectTypeId, string subjectId, string endorserIdId)
        {
            EndorserId = endorserIdId;
            SubjectId = subjectId;
            RequestId = requestId;
            SubjectTypeId = subjectTypeId;
        }

        public string SubjectTypeId { get; set; }
        public string SubjectId { get; }
        public bool CreateClaims { get; private set; }
        public string EndorserId { get; }
        public string RequestId { get; }

        public IEnumerable<Endorsement> AllEndorsements => _endorsements.ToArray();

        public Endorsements AndCreateClaims()
        {
            CreateClaims = true;
            return this;
        }

        public Endorsements AddEndorsement(string claim)
        {
            var claimHash = _endorser.Crypto.GetHash(claim);
            var content = BuildClaimContent(_endorser.Crypto, SubjectTypeId, SubjectId, claim);
            var endorsement = _endorser.Crypto.Sign(content.ToBytes()).ToBase64Url();
            _endorsements.Add(new Endorsement(claim, claimHash, endorsement));
            return this;
        }

        internal static string BuildClaimContent(ICrypto crypto, string subjectTypeId, string subjectId, string claim)
        {
            var claimHash = crypto.GetHash(claim);
            return subjectTypeId != null ? $"{subjectId};{subjectTypeId};{claimHash}" : $"{subjectId};{claimHash}";
        }

        public Endorsements AddEndorsement(string claim, string claimHash, string endorsement)
        {
            _endorsements.Add(new Endorsement(claim, claimHash, endorsement));
            return this;
        }

        public EndorsementBody GenerateIdentityEndorsementBody()
        {
            return new EndorsementBody(NodeConstants.CreateIdentityEndorsementsRequestType, RequestId, SubjectId, EndorserId, _endorsements.ToDictionary(x => x.ClaimHash, x => x.EndorsementString));
        }

        public EndorsementBody GenerateAssetTypeEndorsementBody()
        {
            return new EndorsementBody(NodeConstants.CreateAssetTypeEndorsementsRequestType, RequestId, SubjectId, EndorserId, _endorsements.ToDictionary(x => x.ClaimHash, x => x.EndorsementString));
        }

        public EndorsementBody GenerateAssetEndorsementBody(string subjectTypeId)
        {
            return new EndorsementBody(NodeConstants.CreateAssetEndorsementsRequestType, RequestId, subjectTypeId, SubjectId, EndorserId, _endorsements.ToDictionary(x => x.ClaimHash, x => x.EndorsementString));
        }

        public Dictionary<string, string> GetClaims()
        {
            return _endorsements.ToDictionary(x => x.ClaimHash, x => x.Claim);
        }
    }
}