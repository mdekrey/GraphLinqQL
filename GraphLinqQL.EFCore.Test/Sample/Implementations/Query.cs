using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphLinqQL.Sample.Domain;
using GraphLinqQL.Sample.Interfaces;
using Microsoft.EntityFrameworkCore;
#pragma warning disable CA1307 // Specify StringComparison

namespace GraphLinqQL.Sample.Implementations
{
    class Query : Interfaces.Query.GraphQlContract<GraphQlRoot>
    {
        private readonly StarWarsContext dbContext;

        public Query(StarWarsContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public override IGraphQlObjectResult<Interfaces.Character?> character(FieldContext fieldContext, string id)
        {
            return Original.ResolveTask(async _ => await FindCharacterById(id).ConfigureAwait(false)).Nullable(_ => _.AsUnion<Interfaces.Character>(CharacterTypeMapping));
        }

        private async Task<object> FindCharacterById(string id)
        {
            // This could use dbContext.Characters.FindAsync, but this demonstrates using async/await
            var intId = int.Parse(id);
            return (object)await dbContext.Humans.FindAsync(intId) 
                ?? await dbContext.Droids.FindAsync(intId);
        }

        private static UnionContractBuilder<Interfaces.Character> CharacterTypeMapping(UnionContractBuilder<Interfaces.Character> builder)
        {
            return builder.Add<Domain.Droid, Droid>().Add<Domain.Human, Human>();
        }

        public override IGraphQlObjectResult<Interfaces.Droid?> droid(FieldContext fieldContext, string id)
        {
            var intId = int.Parse(id);
            return Original.ResolveTask(_ => dbContext.Droids.FindAsync(intId).AsTask()).Nullable(_ => _.AsContract<Droid>());
        }

        public override IGraphQlObjectResult<Interfaces.Character?> hero(FieldContext fieldContext, Interfaces.Episode? episode) =>
            episode == null
                ? character(fieldContext, "2001")
                : throw new NotImplementedException();

        public override IGraphQlObjectResult<Interfaces.Human?> human(FieldContext fieldContext, string id)
        {
            var intId = int.Parse(id);
            // This intentionally has a different implementation from the droid for various implementations
            return Original.Resolve(dbContext.Humans.Where(human => human.Id == intId)).List(_ => _.AsContract<Human>()).Only();
        }

        public override IGraphQlObjectResult<IEnumerable<Interfaces.Review?>?> reviews(FieldContext fieldContext, Interfaces.Episode episode)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlObjectResult<IEnumerable<SearchResult?>?> search(FieldContext fieldContext, string? text)
        {
            // FIXME - this is a bad example of doing unions due to the .ToList(). Find another way, since this evaluates the list immediately before selection.
            return Original.Resolve(_ =>
                (from human in dbContext.Humans where human.Name.Contains(text!) select (object)human).ToList().Concat
                (from droid in dbContext.Droids where droid.Name.Contains(text!) select droid).ToList()
            ).List(_ => _.AsUnion<SearchResult>(builder => builder.Add<Domain.Droid, Droid>().Add<Domain.Human, Human>()));
        }

        public override IGraphQlObjectResult<Interfaces.Starship?> starship(FieldContext fieldContext, string id)
        {
            throw new NotImplementedException();
        }
    }
}
