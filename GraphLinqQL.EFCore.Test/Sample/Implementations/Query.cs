using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphLinqQL.Sample.Domain;
using GraphLinqQL.Sample.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GraphLinqQL.Sample.Implementations
{
    class Query : Interfaces.Query.GraphQlContract<GraphQlRoot>
    {
        private readonly StarWarsContext dbContext;

        public Query(StarWarsContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public override IGraphQlResult<Interfaces.Character?> character(FieldContext fieldContext, string id)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<Interfaces.Droid?> droid(FieldContext fieldContext, string id) =>
            Original.ResolveTask(_ => dbContext.Droids.FindAsync(id).AsTask(), droidResult => droidResult.Nullable(_ => _.AsContract<Implementations.Droid>()));

        public override IGraphQlResult<Interfaces.Character?> hero(FieldContext fieldContext, Interfaces.Episode? episode)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<Interfaces.Human?> human(FieldContext fieldContext, string id) =>
            // This intentionally has a different implementation from the droid for various implementations
            Original.Resolve(_ => dbContext.Humans.Where(human => human.Id == id)).Nullable(_ => _.List(_ => _.AsContract<Implementations.Human>()).Only());

        public override IGraphQlResult<IEnumerable<Interfaces.Review?>?> reviews(FieldContext fieldContext, Interfaces.Episode episode)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<IEnumerable?> search(FieldContext fieldContext, string? text)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<Interfaces.Starship?> starship(FieldContext fieldContext, string id)
        {
            throw new NotImplementedException();
        }
    }
}
