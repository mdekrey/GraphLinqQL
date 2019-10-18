using System;
using System.Collections.Generic;
using System.Text;

namespace GraphLinqQL.CodeGeneration.Templates
{
    public static class Generator
    {
        public static string Version()
        {
            return typeof(Generator).Assembly.GetName().Version.ToString(3);
        }
    }
}
