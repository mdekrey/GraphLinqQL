using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphLinqQL.StarWarsV3.Interfaces;

namespace GraphLinqQL.StarWarsV3.Resolvers
{
    public class Query : Interfaces.Query.GraphQlContract<GraphQlRoot>
    {
        public override IGraphQlResult<Character?> character(string id) =>
            Domain.Data.humanLookup.TryGetValue(id, out var human) ? Original.Resolve(_ => human).AsContract<Human>()
            : Domain.Data.droidLookup.TryGetValue(id, out var droid) ? Original.Resolve(_ => droid).AsContract<Droid>()
            : Original.Resolve(_ => (Character?)null);

        public override IGraphQlResult<Interfaces.Droid?> droid(string id) =>
            Original.Resolve(_ => Domain.Data.droidLookup[id]).AsContract<Droid>();

        public override IGraphQlResult<Character?> hero(Episode? episode) =>
            episode == Episode.EMPIRE
                ? character("1000")
                : character("2001");

        public override IGraphQlResult<Interfaces.Human?> human(string id) =>
            Original.Resolve(_ => Domain.Data.humanLookup[id]).AsContract<Human>();

        public override IGraphQlResult<IEnumerable<Interfaces.Review?>?> reviews(Interfaces.Episode episode)
        {
            var domainEpisode = InterfaceToDomain.ConvertEpisode(episode);
            return Original.Resolve(_ => Domain.Data.reviews[domainEpisode]).List(_ => _.AsContract<Review>());
        }

        public override IGraphQlResult<IEnumerable?> search(string? text)
        {
            return Original.Resolve(_ => Domain.Data.humans.Where(v => v.Name.Contains(text))).List(_ => _.AsContract<Human>())
                .Union<IEnumerable<IGraphQlResolvable>?>(Original.Resolve(_ => Domain.Data.droids.Where(v => v.Name.Contains(text))).List(_ => _.AsContract<Droid>()))
                .Union(Original.Resolve(_ => Domain.Data.starships.Where(v => v.Name.Contains(text))).List(_ => _.AsContract<Starship>()));
        }

        public override IGraphQlResult<Interfaces.Starship?> starship(string id) =>
            Original.Resolve(_ => Domain.Data.starshipLookup[id]).AsContract<Starship>();
    }
}
