using System;
using System.Collections.Generic;
using System.Text;

namespace GraphLinqQL
{
    public class ContractMapping : IContract
    {
        public IReadOnlyList<ContractEntry> Resolvables { get; }

        public string TypeName { get; }

        public ContractMapping(string typeName, Type contractType, Type domainType)
            : this(typeName, new[] { new ContractEntry(contractType, domainType) })
        {

        }

        public ContractMapping(string typeName, ContractEntry[] resolvables)
        {
            this.TypeName = typeName;
            this.Resolvables = resolvables;
        }

        internal static string GetTypeName(Type contractType)
        {
            if (typeof(IGraphQlResolvable).IsAssignableFrom(contractType.BaseType))
            {
                return GetTypeName(contractType.BaseType);
            }
            // TODO - this isn't good
            return contractType.Name;
        }
    }
}
