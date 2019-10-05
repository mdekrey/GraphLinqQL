using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using GraphLinqQL.CodeGeneration;

// The following warnings are caused due to the msbuild framework
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
#pragma warning disable CA1819 // Properties should not return arrays

namespace GraphLinqQL
{
#if DEBUG 
    [LoadInSeparateAppDomain]
#endif
#if NET45 && DEBUG
    public class CompileGraphLinqQLSchema : AppDomainIsolatedTask
#else
    public class CompileGraphLinqQLSchema : Task
#endif
    {
        /// <summary>
        /// Gets or sets the filenames containing grammars in PEG-format.
        /// </summary>
        [Required]
        public string InputFile { get; set; }

        /// <summary>
        /// Gets or sets the output filenames that will contain the resulting code.
        /// </summary>
        /// <remarks>
        /// Set to null to use the default, which is the input filenames with ".g.cs" appended.
        /// </remarks>
        public string OutputFile { get; set; }

        [Required]
        public string Namespace { get; set; }

        public string Options { get; set; }

        public float LanguageVersion { get; set; }

        /// <summary>
        /// Reads and compiles the specified grammars.
        /// </summary>
        /// <returns>true, if the compilation was successful; false, otherwise.</returns>
        public override bool Execute()
        {
            var inputs = this.InputFile;
            CompileManager.CompileFile(this.InputFile, this.OutputFile, this.LogError, new GraphQLGenerationOptions { Namespace = Namespace, LanguageVersion = LanguageVersion });

            return !this.Log.HasLoggedErrors;
        }

        private void LogError(CodeGeneration.CompilerError error)
        {
            if (error.IsWarning)
            {
                this.Log.LogWarning(null, error.ErrorNumber, null, error.FileName, error.Line, error.Column, 0, 0, error.ErrorText);
            }
            else
            {
                this.Log.LogError(null, error.ErrorNumber, null, error.FileName, error.Line, error.Column, 0, 0, error.ErrorText);
            }
        }
    }
}
