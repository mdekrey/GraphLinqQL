# Building the solution

Because this creates a custom build task (and consumes it itself), the best way
to build this is with node reuse turned off. If you aren't working on the
CodeGeneration or Ast portions, this won't be a problem; you may use Visual
Studio or other development process as you see fit. However, if you are working
on either of those two packages, either only load those two in Visual Studio or
work directly via the command line. The following commands work well:

    dotnet build /nodeReuse:false
    dotnet clean /nodeReuse:false
    msbuild /t:Build
    msbuild /t:Rebuild

## Issues Building

* If you use the `dotnet` cli, `dotnet build /nodeReuse:false` needs to run
twice due to the order it launches msbuild nodes. Building the CodeGeneration
project first solves the issue.
* `docker` is required to download the GraphQL grammar and run Antlr. This
seems like a more reliable path than installing the JDK and managing jar files
during the msbuild process.