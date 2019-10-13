using System.Collections.Generic;
using System.Linq;

namespace GraphLinqQL.Sample.Domain
{
#nullable disable warnings
    class Character
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Friendship>? Friendships { get; set; }
    }
}