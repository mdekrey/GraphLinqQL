using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphQlResolver.StarWarsV3.Interfaces;

namespace GraphQlResolver.StarWarsV3.Resolvers
{
    public class Query : Interfaces.Query.GraphQlContract<GraphQlRoot>
    {
        public override IGraphQlResult<Character?> character(string id) =>
            Domain.Data.humanLookup.TryGetValue(id, out var human) ? Original.Resolve(_ => human).Convertable().As<Human>()
            : Domain.Data.droidLookup.TryGetValue(id, out var droid) ? Original.Resolve(_ => droid).Convertable().As<Droid>()
            : Original.Resolve(_ => (Character?)null);

        public override IGraphQlResult<Interfaces.Droid?> droid(string id) =>
            Original.Resolve(_ => Domain.Data.droidLookup[id]).Convertable().As<Droid>();

        public override IGraphQlResult<Character?> hero(Episode? episode) =>
            episode == Episode.EMPIRE
                ? character("1000")
                : character("2001");

        public override IGraphQlResult<Interfaces.Human?> human(string id) =>
            Original.Resolve(_ => Domain.Data.humanLookup[id]).Convertable().As<Human>();

        public override IGraphQlResult<IEnumerable<Interfaces.Review?>?> reviews(Interfaces.Episode episode)
        {
            var domainEpisode = InterfaceToDomain.ConvertEpisode(episode);
            return Original.Resolve(_ => Domain.Data.reviews[domainEpisode]).ConvertableList().As<Review>();
        }

        public override IGraphQlResult<IEnumerable?> search(string? text)
        {
            return Original.Resolve(_ => Domain.Data.humans.Where(v => v.Name.Contains(text))).ConvertableList().As<Human>()
                .Union<IEnumerable<IGraphQlResolvable>?>(Original.Resolve(_ => Domain.Data.droids.Where(v => v.Name.Contains(text))).ConvertableList().As<Droid>())
                .Union(Original.Resolve(_ => Domain.Data.starships.Where(v => v.Name.Contains(text))).ConvertableList().As<Starship>());
        }

        public override IGraphQlResult<Interfaces.Starship?> starship(string id) =>
            Original.Resolve(_ => Domain.Data.starshipLookup[id]).Convertable().As<Starship>();
    }

}
