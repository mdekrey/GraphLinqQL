using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace GraphLinqQL
{
    public class JoinPlaceholder<TOriginal>
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

        public JoinPlaceholder<TOriginal> Add<TNewValue>(GraphQlJoin<TOriginal, TNewValue> join, TNewValue newValue)
        {
            return new JoinPlaceholder<TOriginal>(Original, Joins.Add(join, newValue));
        }
    }
}
