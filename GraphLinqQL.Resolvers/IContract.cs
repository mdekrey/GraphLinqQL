using System;
using System.Collections.Generic;

namespace GraphLinqQL
{
    public interface IContract
    {
        string TypeName { get; }
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