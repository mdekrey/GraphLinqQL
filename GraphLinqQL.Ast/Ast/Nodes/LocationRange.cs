using Antlr4.Runtime;
using System;

namespace GraphLinqQL.Ast.Nodes
{
    public readonly struct LocationRange : IEquatable<LocationRange>
    {
        public Location Start { get; }
        public Location Stop { get; }

        public LocationRange(Location start, Location stop)
        {
            this.Start = start;
            this.Stop = stop;
        }

        public override bool Equals(object obj)
        {
            return obj is LocationRange other && other.Start == Start && other.Stop == Stop;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + Start.GetHashCode();
            hash = hash * 23 + Stop.GetHashCode();
            return hash;
        }

        public bool Equals(LocationRange other)
        {
            return other.Start == Start && other.Stop == Stop;
        }

        public static bool operator ==(LocationRange left, LocationRange right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(LocationRange left, LocationRange right)
        {
            return !(left == right);
        }
    }

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