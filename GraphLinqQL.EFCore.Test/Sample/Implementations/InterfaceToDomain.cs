﻿using System;

namespace GraphLinqQL.Sample.Implementations
{
    internal static class InterfaceToDomain
    {

        internal static Domain.Episode ConvertEpisode(Interfaces.Episode arg) =>
            arg switch
            {
                Interfaces.Episode.NEWHOPE => Domain.Episode.NewHope,
                Interfaces.Episode.EMPIRE => Domain.Episode.Empire,
                Interfaces.Episode.JEDI => Domain.Episode.Jedi,
                _ => throw new NotSupportedException()
            };
    }
}