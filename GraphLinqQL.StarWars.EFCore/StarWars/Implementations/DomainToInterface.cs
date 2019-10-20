using System;

namespace GraphLinqQL.StarWars.Implementations
{
    internal static class DomainToInterface
    {
        internal static Interfaces.Episode ConvertEpisode(Domain.Episode arg) =>
            arg switch
            {
                Domain.Episode.NewHope => Interfaces.Episode.Newhope,
                Domain.Episode.Empire => Interfaces.Episode.Empire,
                Domain.Episode.Jedi => Interfaces.Episode.Jedi,
                _ => throw new NotSupportedException()
            };
    }
}