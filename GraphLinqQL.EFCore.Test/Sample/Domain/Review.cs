﻿namespace GraphLinqQL.Sample.Domain
{
#nullable disable warnings
    internal class Review
    {
        public int ReviewId { get; set; }
        public Episode Episode { get; set; }
        public int Stars { get; set; }
        public string Commentary { get; set; }
    }
}