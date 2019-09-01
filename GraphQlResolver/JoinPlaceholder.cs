using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace GraphQlResolver
{
    public abstract class JoinPlaceholder { }
    public class JoinPlaceholder<TOriginal> : JoinPlaceholder
    {
        public TOriginal Original { get; }
        public ImmutableDictionary<IGraphQlJoin, object?> Joins { get; }

        internal JoinPlaceholder(TOriginal original)
            : this(original, ImmutableDictionary<IGraphQlJoin, object?>.Empty)
        {
        }

        internal JoinPlaceholder(TOriginal original, ImmutableDictionary<IGraphQlJoin, object?> joins)
        {
            Original = original;
            Joins = joins;
        }

        public TOutput Get<TOutput>(GraphQlJoin<TOriginal, TOutput> join)
        {
#nullable disable
            return (TOutput)Joins[join];
#nullable restore
        }

        public JoinPlaceholder<TOriginal, TNewValue> Add<TNewValue>(GraphQlJoin<TOriginal, TNewValue> join, TNewValue newValue)
        {
            return new JoinPlaceholder<TOriginal, TNewValue>(Original, Joins.Add(join, newValue));
        }
    }

    public class JoinPlaceholder<TOriginal, TNewValue> : JoinPlaceholder<TOriginal>
    {
        public JoinPlaceholder(TOriginal original, ImmutableDictionary<IGraphQlJoin, object?> immutableDictionary)
            : base(original, immutableDictionary)
        {
        }
    }
}
