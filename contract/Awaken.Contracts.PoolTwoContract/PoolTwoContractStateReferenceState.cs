using AElf.Contracts.MultiToken;

namespace Awaken.Contracts.PoolTwoContract
{
    public partial class PoolTwoContractState
    {
        internal TokenContractContainer.TokenContractReferenceState TokenContract { get; set; }
        
        internal Token.TokenContractContainer.TokenContractReferenceState LpTokenContract { get; set; }
    }
}