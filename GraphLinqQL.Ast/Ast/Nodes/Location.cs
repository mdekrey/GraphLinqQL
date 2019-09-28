using Antlr4.Runtime;
using System;

namespace GraphLinqQL.Ast.Nodes
{
    public readonly struct Location : IEquatable<Location>
    {
        public IToken Start { get; }
        public IToken Stop { get; }

        public Location(IToken start, IToken stop)
        {
            this.Start = start;
            this.Stop = stop;
        }

        public override bool Equals(object obj)
        {
            return obj is Location other && other.Start == Start && other.Stop == Stop;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + Start.GetHashCode();
            hash = hash * 23 + Stop.GetHashCode();
            return hash;
        }

        public bool Equals(Location other)
        {
            return other.Start == Start && other.Stop == Stop;
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