using System.Collections.Generic;
using System.Linq;

namespace GraphLinqQL.StarWars.Domain
{
#nullable disable warnings
    public class Character
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Friendship>? Friendships { get; set; }
    }
}