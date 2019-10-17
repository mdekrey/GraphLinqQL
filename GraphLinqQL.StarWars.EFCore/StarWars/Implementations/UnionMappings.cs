namespace GraphLinqQL.StarWars.Implementations
{
    internal static class UnionMappings
    {
        public static IGraphQlObjectResult<Interfaces.Character> AsCharacterUnion(IGraphQlScalarResult<Domain.Character> characterResult)
        {
            return characterResult.AsUnion<Interfaces.Character>(CharacterTypeMapping);
        }

        public static UnionContractBuilder<Interfaces.Character> CharacterTypeMapping(UnionContractBuilder<Interfaces.Character> builder)
        {
            return builder.Add<Domain.Droid, Droid>().Add<Domain.Human, Human>();
        }

    }
}