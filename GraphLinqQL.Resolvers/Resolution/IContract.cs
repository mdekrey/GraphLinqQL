using System;
using System.Collections.Generic;

namespace GraphLinqQL.Resolution
{
    public interface IContract
    {
        IReadOnlyList<ContractEntry> Resolvables { get; }
    }


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