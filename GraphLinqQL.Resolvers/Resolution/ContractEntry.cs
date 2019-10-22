using System;

namespace GraphLinqQL.Resolution
{
    public class ContractEntry
    {
        public Type Contract { get; }
        public Type DomainType { get; }

        public ContractEntry(Type contract, Type domainType)
        {
            Contract = contract;
            DomainType = domainType;
        }
    }
}