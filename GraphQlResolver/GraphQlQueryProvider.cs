using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GraphQlResolver
{
    public class GraphQlQueryProvider : QueryProvider
    {
        public static GraphQlQueryProvider Instance = new GraphQlQueryProvider();

        public override object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        public static IQueryable<T> CreatePlaceholder<T>(Expression? expression = null)
        {
            return new Query<T>(Instance, expression ?? Expression.Constant(null, typeof(IQueryable<T>)));
        }
    }

    public abstract class QueryProvider : IQueryProvider
    {
        IQueryable<S> IQueryProvider.CreateQuery<S>(Expression expression)
        {
            return new Query<S>(this, expression);
        }

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);
            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(Query<>).MakeGenericType(elementType), new object[] { this, expression });
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        S IQueryProvider.Execute<S>(Expression expression)
        {
            return (S)this.Execute(expression);
        }

        object IQueryProvider.Execute(Expression expression)
        {
            return this.Execute(expression);
        }

        public abstract object Execute(Expression expression);
    }
}