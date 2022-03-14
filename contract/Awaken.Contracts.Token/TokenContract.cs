using AElf.Types;
using Google.Protobuf.WellKnownTypes;

namespace Awaken.Contracts.Token
{
    public partial class TokenContract : TokenContractContainer.TokenContractBase
    {
        public override Empty Initialize(InitializeInput input)
        {
            Assert(State.Owner.Value == null, "Already initialized.");
            State.Owner.Value = input.Owner;
            return new Empty();
        }

        public override Empty SetOwner(Address input)
        {
            Assert(State.Owner.Value == Context.Sender, "Invalid");
            State.Owner.Value = input;
            return new Empty();
        }
    }
}