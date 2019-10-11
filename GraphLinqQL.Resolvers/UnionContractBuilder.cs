using System;
using System.Collections.Generic;

namespace GraphLinqQL
{
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
            return new ContractMapping(conditions.ToArray());
        }
    }
}