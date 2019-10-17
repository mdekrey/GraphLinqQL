namespace GraphLinqQL.StarWarsV3.Resolvers
{
    class PageInfo : Interfaces.PageInfo.GraphQlContract<PageInfo.Data>
    {
        public override IGraphQlScalarResult<string?> endCursor() =>
            Original.Resolve(d => d.endCursor);

        public override IGraphQlScalarResult<bool> hasNextPage() =>
            Original.Resolve(d => d.hasNextPage);

        public override IGraphQlScalarResult<string?> startCursor() =>
            Original.Resolve(d => d.startCursor);

        public class Data
        {
            public string startCursor;
            public string endCursor;
            public bool hasNextPage;

            public Data(string startCursor, string endCursor, bool hasNextPage)
            {
                this.startCursor = startCursor;
                this.endCursor = endCursor;
                this.hasNextPage = hasNextPage;
            }
        }
    }

}
