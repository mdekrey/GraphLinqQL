using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace GraphQlResolver
{
    public static class GraphQlJoin
    {
        internal static readonly MethodInfo FindOriginalInfo = typeof(GraphQlJoin).GetMethods(BindingFlags.Public | BindingFlags.Static).Single(m => m.Name == nameof(FindOriginal));
        internal static readonly MethodInfo BuildPlaceholderInfo = typeof(GraphQlJoin).GetMethods(BindingFlags.Public | BindingFlags.Static).Single(m => m.Name == nameof(BuildPlaceholder));

        public static GraphQlJoin<TInput, TJoined> Join<TInput, TJoined>(Func<IQueryable<JoinPlaceholder<TInput>>, IQueryable<JoinPlaceholder<TInput, TJoined>>> func)
        {
            var root = Enumerable.Empty<JoinPlaceholder<TInput>>().AsQueryable();
            return new GraphQlJoin<TInput, TJoined>(func(root), root);
        }

        public static TInput FindOriginal<TInput>(JoinPlaceholder<TInput> param)
        {
            throw new NotImplementedException("This is not meant to be invoked but to be replaced.");
        }
        public static JoinPlaceholder<TInput, TJoined> BuildPlaceholder<TInput, TJoined>(JoinPlaceholder<TInput> original, TJoined newValue)
        {
            throw new NotImplementedException("This is not meant to be invoked but to be replaced.");
        }
    }

    public class GraphQlJoin<TFromDomain, TToDomain> : IGraphQlJoin
    {
        private static readonly MethodInfo getJoinValue = typeof(JoinPlaceholder<TFromDomain>).GetMethod("Get").MakeGenericMethod(typeof(TToDomain));
        public ParameterExpression Placeholder { get; } = Expression.Variable(typeof(TToDomain), "JoinPlaceholder " + typeof(TToDomain).FullName);

        public Expression Queryable { get; }
        public Expression Root { get; }


        Expression IGraphQlJoin.Queryable => Queryable;
        Expression IGraphQlJoin.Root => Root;

        public GraphQlJoin(IQueryable<JoinPlaceholder<TFromDomain, TToDomain>> queryable, IQueryable<JoinPlaceholder> root)
        {
            this.Queryable = queryable.Expression;
            this.Root = root.Expression;
        }

        public Expression Convert(ParameterExpression joinPlaceholderParameter)
        {
            System.Diagnostics.Debug.Assert(joinPlaceholderParameter.Type == typeof(JoinPlaceholder<TFromDomain>));

            var result = new RefactorExpression(joinPlaceholderParameter, this).Visit(this.Queryable);
            return Expression.Convert(result, typeof(IQueryable<JoinPlaceholder<TFromDomain>>));
        }

        public Expression GetAccessor(ParameterExpression joinPlaceholderParameter)
        {
            System.Diagnostics.Debug.Assert(joinPlaceholderParameter.Type == typeof(JoinPlaceholder<TFromDomain>));
         
            return Expression.Call(joinPlaceholderParameter, getJoinValue, Expression.Constant(this));
        }


        private class RefactorExpression : ExpressionVisitor
        {
            private readonly ParameterExpression joinPlaceholderParameter;
            private readonly GraphQlJoin<TFromDomain, TToDomain> join;
            private static readonly MethodInfo findOriginalInfo = GraphQlJoin.FindOriginalInfo.MakeGenericMethod(typeof(TFromDomain));
            private static readonly MethodInfo buildPlaceholderInfo = GraphQlJoin.BuildPlaceholderInfo.MakeGenericMethod(typeof(TFromDomain), typeof(TToDomain));
            private static readonly MethodInfo addJoin = typeof(JoinPlaceholder<TFromDomain>).GetMethod("Add").MakeGenericMethod(typeof(TToDomain));

            public RefactorExpression(ParameterExpression joinPlaceholderParameter, GraphQlJoin<TFromDomain, TToDomain> join)
            {
                this.joinPlaceholderParameter = joinPlaceholderParameter;
                this.join = join;
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (node.Method == findOriginalInfo)
                {
                    return base.Visit(Expression.Property(node.Arguments[0], nameof(JoinPlaceholder<TFromDomain>.Original)));
                }
                else if (node.Method == buildPlaceholderInfo)
                {
                    return base.Visit(Expression.Call(node.Arguments[0], addJoin, Expression.Constant(join), node.Arguments[1]));
                }
                return base.VisitMethodCall(node);
            }

        }
    }
}
