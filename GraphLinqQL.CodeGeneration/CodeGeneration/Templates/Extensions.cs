using System;
using System.Collections.Generic;
using System.Text;

namespace GraphLinqQL.CodeGeneration.Templates
{
    static class Extensions
    {
        public static string Docblock(this string input, string indentation)
        {
            return string.Join($"\n{indentation}/// ", input.Split('\n'));
        }
    }
}
