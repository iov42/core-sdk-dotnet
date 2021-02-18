using System;
using System.Linq;
using Iov42sdk.Models.Headers;

namespace Iov42sdk.Models.Transfers
{
    public class TransferRequest
    {
        private Authorisation[] _authorisations = new Authorisation[0];
        private readonly TransfersBody _body;

        public TransfersBody Body => _body;
        public Authorisation[] Authorisations => _authorisations;

        public TransferRequest(params SingleTransfer[] transfers)
        {
            _body = new TransfersBody(transfers);
        }

        public TransferRequest(TransfersBody body)
        {
            _body = body;
        }

        public TransferRequest AddAuthorisation(Func<TransfersBody, Authorisation> authorisation)
        {
            _authorisations = _authorisations.Concat(new[] {authorisation(_body)}).ToArray();
            return this;
        }

        public TransferRequest AddAuthorisations(params Authorisation[] authorisations)
        {
            _authorisations = _authorisations.Concat(authorisations).ToArray();
            return this;
        }
    }
}
