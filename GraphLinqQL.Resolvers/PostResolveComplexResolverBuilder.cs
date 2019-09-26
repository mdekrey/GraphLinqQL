using System;
using System.Collections.Generic;

namespace GraphLinqQL
{
    internal class PostResolveComplexResolverBuilder : IComplexResolverBuilder
    {
        private IComplexResolverBuilder original;
        private readonly Func<IGraphQlResult, IGraphQlResult> postResolve;

        public PostResolveComplexResolverBuilder(IComplexResolverBuilder complexResolverBuilder, Func<IGraphQlResult, IGraphQlResult> postResolve)
        {
            this.original = complexResolverBuilder;
            this.postResolve = postResolve;
        }

        public IComplexResolverBuilder Add(string displayName, Func<IGraphQlResolvable, IGraphQlResult> resolve) =>
            new PostResolveComplexResolverBuilder(original.Add(displayName, resolve), postResolve);

        public IComplexResolverBuilder Add(string propertyName, IDictionary<string, string>? parameters = null) =>
            new PostResolveComplexResolverBuilder(original.Add(propertyName, parameters), postResolve);

        public IComplexResolverBuilder Add(string displayName, string propertyName, IDictionary<string, string>? parameters = null) =>
            new PostResolveComplexResolverBuilder(original.Add(displayName, propertyName, parameters), postResolve);

        public IGraphQlResult Build() => this.postResolve(original.Build());

        public IComplexResolverBuilder IfType(string value, Func<IComplexResolverBuilder, IComplexResolverBuilder> typedBuilder) =>
            new PostResolveComplexResolverBuilder(original.IfType(value, typedBuilder), postResolve);
    }
}