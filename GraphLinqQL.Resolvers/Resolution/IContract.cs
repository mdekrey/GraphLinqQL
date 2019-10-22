using System;
using System.Collections.Generic;

namespace GraphLinqQL.Resolution
{
    public interface IContract
    {
        IReadOnlyList<ContractEntry> Resolvables { get; }
    }
}