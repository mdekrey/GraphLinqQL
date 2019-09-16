﻿#nullable disable
using System.Collections.Generic;

namespace GraphQlResolver.StarWarsV4.Domain
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<FilmCharacter> FilmCharacters { get; set; }
    }
}