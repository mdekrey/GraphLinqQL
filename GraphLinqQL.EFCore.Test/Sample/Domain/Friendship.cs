﻿namespace GraphLinqQL.Sample.Domain
{
    class Friendship
    {
#nullable disable warnings
        public int FromId { get; set; }
        public Character? From { get; set; }
        public int ToId { get; set; }
        public Character? To { get; set; }
    }
}