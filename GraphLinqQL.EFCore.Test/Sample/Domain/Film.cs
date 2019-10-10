using System;
using System.Collections.Generic;
using System.Text;

namespace GraphLinqQL.Sample.Domain
{
#nullable disable warnings
    class Film
    {
        public Episode EpisodeId { get; set; }

        public string Title { get; set; }

        public int HeroId { get; set; }
        public Character? Hero { get; set; }
    }
}
