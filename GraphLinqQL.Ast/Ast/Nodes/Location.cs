using System;

namespace GraphLinqQL.Ast.Nodes
{
    public readonly struct Location : IEquatable<Location>
    {
        public int Line { get; }
        public int Column { get; }

        public Location(int line, int column)
        {
            Line = line;
            Column = column;
        }

        public override bool Equals(object obj)
        {
            return obj is Location other && other.Line == Line && other.Column == Column;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + Line.GetHashCode();
            hash = hash * 23 + Column.GetHashCode();
            return hash;
        }

        public bool Equals(Location other)
        {
            return other.Line == Line && other.Column == Column;
        }

        public static bool operator ==(Location left, Location right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Location left, Location right)
        {
            return !(left == right);
        }
    }
}