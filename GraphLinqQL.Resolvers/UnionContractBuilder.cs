using System;
using System.Collections.Generic;
using System.Linq;

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

    public class UnionContractBuilder<T> where T : IGraphQlResolvable
    {
        private readonly List<ContractMappingCondition> conditions = new List<ContractMappingCondition>();

        public UnionContractBuilder<T> Add<TDomain, TContract>()
            where TContract : T, IGraphQlAccepts<TDomain>
        {
            conditions.Add(new ContractMappingCondition(typeof(TDomain), typeof(TContract)));
            return this;
        }

        internal ContractMapping CreateContractMapping()
        {
            return new ContractMapping(conditions.Select(c => new ContractEntry(c.ContractType, c.DomainType)).ToArray());
        }

        public IReadOnlyList<ContractMappingCondition> Conditions => conditions.AsReadOnly();
    }
}