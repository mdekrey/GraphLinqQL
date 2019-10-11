using System;
using System.Collections.Generic;

namespace GraphLinqQL
{
    public class UnionContractBuilder<T> where T : IGraphQlResolvable
    {
        internal readonly struct Condition
        {
            public Condition(Type domainType, Type contractType)
            {
                DomainType = domainType;
                ContractType = contractType;
            }

            public Type DomainType { get; }
            public Type ContractType { get; }
        }

        private readonly List<Condition> conditions = new List<Condition>();

        public UnionContractBuilder<T> Add<TDomain, TContract>()
            where TContract : T, IGraphQlAccepts<TDomain>
        {
            conditions.Add(new Condition(typeof(TDomain), typeof(TContract)));
            return this;
        }

    }
}