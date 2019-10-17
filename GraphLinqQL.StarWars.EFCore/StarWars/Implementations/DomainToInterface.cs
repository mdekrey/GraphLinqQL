﻿using System;

namespace GraphLinqQL.StarWars.Implementations
{
    internal static class DomainToInterface
    {
        internal static Interfaces.Episode ConvertEpisode(Domain.Episode arg) =>
            arg switch
            {
                Domain.Episode.NewHope => Interfaces.Episode.NEWHOPE,
                Domain.Episode.Empire => Interfaces.Episode.EMPIRE,
                Domain.Episode.Jedi => Interfaces.Episode.JEDI,
                _ => throw new NotSupportedException()
            };
    }
}