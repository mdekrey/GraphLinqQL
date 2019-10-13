using System.Collections.Generic;

namespace GraphLinqQL
{
    public interface IContract
    {
        IReadOnlyList<ContractMappingCondition> ContractMappingCondition { get; }
    }
}