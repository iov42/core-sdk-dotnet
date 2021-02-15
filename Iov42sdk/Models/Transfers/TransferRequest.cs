using System;
using System.Collections.Generic;
using System.Linq;
using Iov42sdk.Models.Headers;

namespace Iov42sdk.Models.Transfers
{
    public class TransferRequest
    {
        private Authorisation[] _authorisations = new Authorisation[0];
        private readonly List<SingleTransfer> _transfers = new List<SingleTransfer>();
        private readonly TransfersBody _body;

        public TransfersBody Body => _body;
        public Authorisation[] Authorisations => _authorisations;

        public TransferRequest(params SingleTransfer[] transfers)
        {
            _transfers.AddRange(transfers);
            _body = new TransfersBody(_transfers.ToArray());
        }

        public TransferRequest AddAuthorisation(Func<TransfersBody, Authorisation> authorisation)
        {
            _authorisations = _authorisations.Concat(new[] {authorisation(_body)}).ToArray();
            return this;
        }
    }
}
