using System;
using System.Collections.Generic;
using System.Text;

namespace GraphLinqQL.CodeGeneration
{
    public static class LanguageFeatureExtensions
    {
        public static bool ShowNullabilityIndicators(this GraphQLGenerationOptions options) => options.LanguageVersion >= 8;

    }
}
