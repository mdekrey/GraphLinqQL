using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphLinqQL.StarWarsV3.Interfaces;

namespace GraphLinqQL.StarWarsV3.Resolvers
{
    public class Query : Interfaces.Query.GraphQlContract<GraphQlRoot>
    {
        public override IGraphQlObjectResult<Character?> character(FieldContext fieldContext, string id) => 
            Original.Union(
                _ => _.Resolve(_ => Domain.Data.humans.Where(human => human.Id == id)).List(_ => _.AsContract<Human>() as IGraphQlObjectResult<Character?>),
                _ => _.Resolve(_ => Domain.Data.droids.Where(droid => droid.Id == id)).List(_ => _.AsContract<Droid>())
            ).Only();

        public override IGraphQlObjectResult<Interfaces.Droid?> droid(FieldContext fieldContext, string id) =>
            Original.Resolve(_ => Domain.Data.droidLookup[id]).AsContract<Droid>();

        public override IGraphQlObjectResult<Character?> hero(FieldContext fieldContext, Episode? episode) =>
            episode == Episode.EMPIRE
                ? character(fieldContext, "1000")
                : character(fieldContext, "2001");

        public override IGraphQlObjectResult<Interfaces.Human?> human(FieldContext fieldContext, string id) =>
            Original.Resolve(_ => Domain.Data.humanLookup[id]).AsContract<Human>();

        public override IGraphQlObjectResult<IEnumerable<Interfaces.Review?>?> reviews(FieldContext fieldContext, Interfaces.Episode episode)
        {
            var domainEpisode = InterfaceToDomain.ConvertEpisode(episode);
            return Original.Resolve(_ => Domain.Data.reviews[domainEpisode]).List(_ => _.AsContract<Review>());
        }

        public override IGraphQlObjectResult<IEnumerable<SearchResult?>?> search(FieldContext fieldContext, string? text)
        {
            return Original.Union(
                _ => _.Resolve(from human in Domain.Data.humans where human.Name.Contains(text!) select human).List(_ => _.AsContract<Human>() as IGraphQlObjectResult<SearchResult?>),
                _ => _.Resolve(from droid in Domain.Data.droids where droid.Name.Contains(text!) select droid).List(_ => _.AsContract<Droid>()),
                _ => _.Resolve(from starship in Domain.Data.starships where starship.Name.Contains(text!) select starship).List(_ => _.AsContract<Starship>())
            );
        }

        public override IGraphQlObjectResult<Interfaces.Starship?> starship(FieldContext fieldContext, string id) =>
            Original.Resolve(_ => Domain.Data.starshipLookup[id]).AsContract<Starship>();
    }
}
