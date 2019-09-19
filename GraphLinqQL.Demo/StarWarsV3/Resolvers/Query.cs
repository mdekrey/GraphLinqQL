using System;
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
            Domain.Data.humanLookup.TryGetValue(id, out var human) ? Original.Resolve(_ => human).As<Human>()
            : Domain.Data.droidLookup.TryGetValue(id, out var droid) ? Original.Resolve(_ => droid).As<Droid>()
            : Original.Resolve(_ => (Character?)null);

        public override IGraphQlResult<Interfaces.Droid?> droid(string id) =>
            Original.Resolve(_ => Domain.Data.droidLookup[id]).As<Droid>();

        public override IGraphQlResult<Character?> hero(Episode? episode) =>
            episode == Episode.EMPIRE
                ? character("1000")
                : character("2001");

        public override IGraphQlResult<Interfaces.Human?> human(string id) =>
            Original.Resolve(_ => Domain.Data.humanLookup[id]).As<Human>();

        public override IGraphQlResult<IEnumerable<Interfaces.Review?>?> reviews(Interfaces.Episode episode)
        {
            var domainEpisode = InterfaceToDomain.ConvertEpisode(episode);
            return Original.Resolve(_ => Domain.Data.reviews[domainEpisode]).List(_ => _.As<Review>());
        }

        public override IGraphQlResult<IEnumerable?> search(string? text)
        {
            return Original.Resolve(_ => Domain.Data.humans.Where(v => v.Name.Contains(text))).List(_ => _.As<Human>())
                .Union<IEnumerable<IGraphQlResolvable>?>(Original.Resolve(_ => Domain.Data.droids.Where(v => v.Name.Contains(text))).List(_ => _.As<Droid>()))
                .Union(Original.Resolve(_ => Domain.Data.starships.Where(v => v.Name.Contains(text))).List(_ => _.As<Starship>()));
        }

        public override IGraphQlResult<Interfaces.Starship?> starship(string id) =>
            Original.Resolve(_ => Domain.Data.starshipLookup[id]).As<Starship>();
    }

}
