using System;

namespace GraphLinqQL
{
    public readonly struct QueryLocation : IEquatable<QueryLocation>
    {
        public int Line { get; }
        public int Column { get; }

        public QueryLocation(int line, int column)
        {
            Line = line;
            Column = column;
        }

        public override bool Equals(object obj)
        {
            return obj is QueryLocation other && other.Line == Line && other.Column == Column;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + Line.GetHashCode();
            hash = hash * 23 + Column.GetHashCode();
            return hash;
        }

        public bool Equals(QueryLocation other)
        {
            return other.Line == Line && other.Column == Column;
        }

        public static bool operator ==(QueryLocation left, QueryLocation right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(QueryLocation left, QueryLocation right)
        {
            return !(left == right);
        }
    }
}