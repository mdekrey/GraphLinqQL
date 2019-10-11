using System;
using System.Collections.Generic;
using System.Text;

namespace GraphLinqQL
{
    public class ContractMappingCondition
    {
        public ContractMappingCondition(Type domainType, Type contractType)
        {
            DomainType = domainType;
            ContractType = contractType;
        }

        public Type DomainType { get; }
        public Type ContractType { get; }
    }

    class ContractMapping : IContract
    {
        public IReadOnlyList<ContractMappingCondition> ContractMappingCondition { get; }

        public ContractMapping(Type contractType)
            : this(new[] { new ContractMappingCondition(typeof(void), contractType) })
        {

        }

        public ContractMapping(ContractMappingCondition[] contractMappingCondition)
        {
            this.ContractMappingCondition = contractMappingCondition;
        }
    }
}
