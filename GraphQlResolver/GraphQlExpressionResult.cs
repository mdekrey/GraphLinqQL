﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace GraphQlResolver
{
    class GraphQlExpressionResult<TReturnType> : IGraphQlResult<TReturnType>
    {
        public IServiceProvider ServiceProvider { get; }

        public LambdaExpression UntypedResolver { get; }

        public ImmutableHashSet<IGraphQlJoin> Joins { get; }

        IReadOnlyCollection<IGraphQlJoin> IGraphQlResult.Joins => Joins;

        public GraphQlExpressionResult(LambdaExpression func, IServiceProvider serviceProvider)
            : this(func, serviceProvider, ImmutableHashSet<IGraphQlJoin>.Empty) { }

        public GraphQlExpressionResult(LambdaExpression func, IServiceProvider serviceProvider, ImmutableHashSet<IGraphQlJoin> joins)
        {
            this.UntypedResolver = func;
            this.ServiceProvider = serviceProvider;
            this.Joins = joins;
        }

        public static GraphQlExpressionResult<TReturnType> Construct<TInput>(Expression<Func<TInput, TReturnType>> func, IServiceProvider serviceProvider)
        {
            return new GraphQlExpressionResult<TReturnType>(func, serviceProvider);
        }
    }

}