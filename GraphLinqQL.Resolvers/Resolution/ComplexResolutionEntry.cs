using System;
using System.Collections.Generic;

namespace GraphLinqQL.Resolution
{
    class ComplexResolutionEntry
    {
        public IGraphQlResolvable GraphQlResolvable { get; }
        public Type DomainType { get; }
        public Action<FieldContext> FieldContextSetup { get; }
        public Dictionary<string, IGraphQlScalarResult> Results { get; } = new Dictionary<string, IGraphQlScalarResult>();

        public ComplexResolutionEntry(IGraphQlResolvable graphQlResolvable, Type domainType, Action<FieldContext> fieldContextSetup)
        {
            this.GraphQlResolvable = graphQlResolvable;
            DomainType = domainType;
            FieldContextSetup = fieldContextSetup;
        }
    }
}
