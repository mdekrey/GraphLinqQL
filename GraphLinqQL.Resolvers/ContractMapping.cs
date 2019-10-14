using System;
using System.Collections.Generic;
using System.Text;

namespace GraphLinqQL
{
    public class ContractMapping : IContract
    {
        public IReadOnlyList<ContractEntry> Resolvables { get; }

        public ContractMapping(Type contractType, Type domainType)
            : this(new[] { new ContractEntry(contractType, domainType) })
        {

        }

        public ContractMapping(ContractEntry[] resolvables)
        {
            this.Resolvables = resolvables;
        }
    }
}
