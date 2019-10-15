using System;
using System.Collections.Generic;
using System.Text;

namespace GraphLinqQL.StarWars.Domain
{
#nullable disable warnings
    public class Film
    {
        public Episode EpisodeId { get; set; }

        public string Title { get; set; }

        public int HeroId { get; set; }
        public Character? Hero { get; set; }
    }
}
