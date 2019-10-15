﻿using System;
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
            var intId = int.Parse(id);
            return Original.Union(
                _ => _.Resolve(from human in dbContext.Humans where human.Id == intId select human).List(_ => _.AsContract<Human>() as IGraphQlObjectResult<Interfaces.Character?>),
                _ => _.Resolve(from droid in dbContext.Droids where droid.Id == intId select droid).List(_ => _.AsContract<Droid>())
            ).Only();
        }

        public override IGraphQlObjectResult<Interfaces.Droid?> droid(FieldContext fieldContext, string id)
        {
            var intId = int.Parse(id);
            // This intentionally has a different implementation from the human/starship for various implementations
            return Original.ResolveTask(_ => FindDroidById(id)).Nullable(_ => _.AsContract<Droid>());
        }

        private async Task<Domain.Droid> FindDroidById(string id)
        {
            // This could use dbContext.Characters.FindAsync, but this demonstrates using async/await
            var intId = int.Parse(id);
            return await dbContext.Droids.FindAsync(intId);
        }

        public override IGraphQlObjectResult<Interfaces.Character?> hero(FieldContext fieldContext, Interfaces.Episode? episode) =>
            episode switch
            {
                null => character(fieldContext, "2001"),
                Interfaces.Episode ep => Original
                    .Resolve(_ => dbContext.Films.Where(f => f.EpisodeId == InterfaceToDomain.ConvertEpisode(ep)).Select(f => f.Hero))
                    .List(_ => _.AsUnion<Interfaces.Character>(builder => builder.Add<Domain.Human, Human>().Add<Domain.Droid, Droid>()))
                    .Only()
            };

        public override IGraphQlObjectResult<Interfaces.Human?> human(FieldContext fieldContext, string id)
        {
            var intId = int.Parse(id);
            return Original.Resolve(dbContext.Humans.Where(human => human.Id == intId)).List(_ => _.AsContract<Human>()).Only();
        }

        public override IGraphQlObjectResult<IEnumerable<Interfaces.Review?>?> reviews(FieldContext fieldContext, Interfaces.Episode episode)
        {
            return Original.Resolve(dbContext.Reviews.Where(review => review.Episode == InterfaceToDomain.ConvertEpisode(episode)))
                .List(_ => _.AsContract<Review>());
        }

        public override IGraphQlObjectResult<IEnumerable<SearchResult?>?> search(FieldContext fieldContext, string? text)
        {
            return Original.Union(
                _ => _.Resolve(from human in dbContext.Humans where human.Name.Contains(text!) select human).List(_ => _.AsContract<Human>() as IGraphQlObjectResult<SearchResult?>),
                _ => _.Resolve(from droid in dbContext.Droids where droid.Name.Contains(text!) select droid).List(_ => _.AsContract<Droid>()),
                _ => _.Resolve(from starship in dbContext.Starships where starship.Name.Contains(text!) select starship).List(_ => _.AsContract<Starship>())
            );
        }

        public override IGraphQlObjectResult<Interfaces.Starship?> starship(FieldContext fieldContext, string id)
        {
            var intId = int.Parse(id);
            return Original.Resolve(dbContext.Starships.Where(starship => starship.Id == intId)).List(_ => _.AsContract<Starship>()).Only();
        }
    }
}
