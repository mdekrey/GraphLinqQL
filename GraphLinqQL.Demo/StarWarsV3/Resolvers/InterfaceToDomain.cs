﻿using System;

namespace GraphLinqQL.StarWarsV3.Resolvers
{
    internal static class InterfaceToDomain
    {

        internal static Domain.Episode ConvertEpisode(Interfaces.Episode arg) =>
            arg switch
            {
                Interfaces.Episode.Newhope => Domain.Episode.NewHope,
                Interfaces.Episode.Empire => Domain.Episode.Empire,
                Interfaces.Episode.Jedi => Domain.Episode.Jedi,
                _ => throw new NotSupportedException()
            };
    }
}