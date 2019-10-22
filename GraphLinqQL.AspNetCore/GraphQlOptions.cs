using GraphLinqQL.Directives;
using GraphLinqQL.Resolution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphLinqQL
{
    public class GraphQlOptions
    {
        private readonly List<Type> directives = new List<Type>()
        {
            typeof(IncludeDirective),
            typeof(SkipDirective)
        };
        private Type? query;
        private Type? mutation;
        private Type? subscription;
        private Type? typeResolver;

        public IReadOnlyList<Type> Directives => directives.AsReadOnly();

        public Type? Query
        {
            get { return query; }
            set
            {
                if (!typeof(IGraphQlResolvable).IsAssignableFrom(value))
                {
                    throw new ArgumentException(nameof(value) + " must be assignable to " + typeof(IGraphQlResolvable), nameof(value));
                }
                query = value;
            }
        }

        public Type? Mutation
        {
            get { return mutation; }
            set
            {
                if (!typeof(IGraphQlResolvable).IsAssignableFrom(value))
                {
                    throw new ArgumentException(nameof(value) + " must be assignable to " + typeof(IGraphQlResolvable), nameof(value));
                }
                mutation = value;
            }
        }

        public Type? Subscription
        {
            get { return subscription; }
            set
            {
                if (!typeof(IGraphQlResolvable).IsAssignableFrom(value))
                {
                    throw new ArgumentException(nameof(value) + " must be assignable to " + typeof(IGraphQlResolvable), nameof(value));
                }
                subscription = value;
            }
        }

        public Type? TypeResolver
        {
            get { return typeResolver; }
            set
            {
                if (!typeof(IGraphQlTypeResolver).IsAssignableFrom(value))
                {
                    throw new ArgumentException(nameof(value) + " must be assignable to " + typeof(IGraphQlTypeResolver), nameof(value));
                }
                typeResolver = value;
            }
        }

        public void AddDirective(Type directiveType)
        {
            if (typeof(IGraphQlDirective).IsAssignableFrom(directiveType))
            {
                directives.Add(directiveType);
            }
        }

        public void RemoveDirective(Type directiveType)
        {
            directives.Remove(directiveType);
        }

        public void ClearDirectives()
        {
            directives.Clear();
        }
    }
}
