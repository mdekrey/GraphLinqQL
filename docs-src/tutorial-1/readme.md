# Getting Started with GraphLinqQL

For our initial tutorial, we're building out a simple .Net Core-based GraphQL
server. So that it stays simple, we'll be binding to in-memory objects, but it
works just as well with EF Core - in fact, most of this code stays the same
for the EF Core tutorial!

Make sure you have the [dotnet CLI](https://aka.ms/dotnetcoregs) installed;
we're using 3.0. You also should have Visual Studio with .Net Core support or
another editor (such as VS Code with OmniSharp.) We'll move forward assuming you
know the basics of C# development.

Once you have your machine, run the following commands from your preferred
terminal.

```powershell
dotnet new web -o GqlLinqGetStarted
cd GqlLinqGetStarted
```

From here, you can launch the project (here we used `GqlLinqGetStarted`) in
Visual Studio or any other C# editor. You'll have a basic new website with a
`Program.cs` and a `Startup.cs` and not much else. Let's add a data model to the
`Data` namespace underneath our root.

```csharp
`[Model](src/GqlLinqGetStarted/Data/Model.cs#DataNamespace)
```

As you can see, this is just using plain .Net objects and lists. GraphLinqQL
works with Plain Old CLR Objects (POCOs) so that you don't need to rewrite your
existing Domain Model to get started. Once this is in place, we should start by
defining a GraphQL API.

> API-Driven development is important for quality APIs. Thinking about how your
> end-user developers will invoke the API is much more important than making the
> implementation easy, because your end-users are often customers! Even when
> they aren't directly paying for your API, having a schema separate from the
> code makes it easier to maintain, easier to prevent breaking changes, and more
> straightforward to document - or even rewrite if you want to change your
> underlying implementation, such as changes from domain objects to accessing EF
> Core directly!

In this case, we'll use the following simple schema. Place it in a `.graphql`
file within the Api folder. You can learn more about [GraphQL schemas from the
official documentation](https://graphql.org/learn/schema/).

```GraphQL
`[GraphQL Schema](src/GqlLinqGetStarted/Api/blogging-schema.graphql)
```

Once you have the file in place, run the following additional commands from your
project directory. This will add the reference to the `GraphLinqQL` project with
the basic classes and code generation and run the initial code generation. The
folder to which we added the `.graphql` file (Api) is used for the namespace of
the generated code, as is common for C# projects. If your project's language
mode is set to C# 8 or higher, GraphLinqQL will even generate nullability
indicators on your API classes!

```powershell
dotnet add package GraphLinqQL.Resolvers
dotnet build
```

Each new `type` provided in your GraphQL schema now has a
`<TypeName>.GraphQlContract<T>` class that should be implemented. It makes sense
to start with the type that is specified as the `query` in the schema. In this
case, it is conveniently known as the `Query` type.

> If you're using Visual Studio, you can use the Object Browser (View -> Object
> Browser) and see that under the Api namespace, more classes are now available.

Make a new class called `QueryResolver` in the `API` folder, and add the
following base class to it:

```csharp
`[QueryResolverDeclaration](src/GqlLinqGetStarted/Api/QueryResolver.cs#Declaration)
```

We used `GraphQlRoot` for the generic parameter because this is used at the root
of the GraphQL Query.

At this point, you can use your editor to implement the class. You should get
the methods that correspond to the GraphQL fields - in this case, `Blogs`, with
a return type of `IGraphQlObjectResult<IEnumerable<Blog>?>`. The `Blog` class
returned from our API is actually the class generated from the schema, and we
know that we need to return a list. We can implement this as follows:

```csharp
`[QueryResolverDeclaration](src/GqlLinqGetStarted/Api/QueryResolver.cs#Declaration)
```

We "resolve" from the root to our list of blogs first. Next, we need to set up
the Nullability preferences here - this will capture errors and handle nulls per
the GraphQL specification, so be sure to include it even if you aren't using
strict null checks in your project. Within that lambda, we specify that we have
a list and tell it that each item is resolved via a contract of our
`BlogResolver`, which we haven't implemented yet. This will map from our
`Data.Blog` to the GraphQL schema's `Api.Blog`. Create a `BlogResolver` class
and implement it as follows:

```csharp
`[BlogResolver](src/GqlLinqGetStarted/Api/BlogResolver.cs#BlogResolver)
```

This time, we used `Data.Blog` in our generic parameter of the base class - the
`BlogResolver` expects a type of `Data.Blog` from which to base queries. You may
implement `Blog.GraphQlContract<T>` multiple times for different `T` types to
allow for different domain models to be resolved as a single GraphQL type!

Here, we have a few simple data properties. Note that `Url` can be returned
as-is, and `Id` requires a bit of manipulation. Also, while the GraphQL Schema
specifies our `Id` as an `ID` field, GraphLinqQL translated this directly to a
`string` field for us!

The Posts field is a bit more complex, but looks similar to the `Blogs` field
above. Once again, this needs a new resolver for the `Post` class, so create a
new `PostResolver`.

```csharp
`[PostResolver](src/GqlLinqGetStarted/Api/PostResolver.cs#PostResolver)
```

Our final type is the `Mutation`, though not all schemas provide them. We could
only partially implement our schema by not implementing this base class, but
that would be poor form. Again, we'll use the `GraphQlRoot` for the generic
parameter, resulting in this declaration:

```csharp
`[MutationResolverDeclaration](src/GqlLinqGetStarted/Api/MutationResolver.cs#Declaration)
```

Implementing this class gives us two additional methods, one for each field in
our GraphQL schema. We'll start with `AddBlog`.

```csharp
`[MutationResolverDeclaration](src/GqlLinqGetStarted/Api/MutationResolver.cs#AddBlogImplementation)
```

Here, we use `ResolveTask` to allow us to use .Net's Task framework, since most
times you want to persist values, you'll need async handles. You can also see
that AddBlog recieves the GraphQL field parameters as method parameters
automatically. The `_` represents the root object, which is unused in this
example. Because we're using the default `System.Collections.Generic.List`
class, adding the blog is straightforward.

Similarly, we can implement `AddPost`.

```csharp
`[MutationResolverDeclaration](src/GqlLinqGetStarted/Api/MutationResolver.cs#AddPostImplementation)
```

At this point, we've implemented our full GraphQL schema! However, we still
don't have a server. Going back to your command line, add a new package, which
includes GraphLinqQL's ASP.Net Core server implementation.

```powershell
dotnet add package GraphLinqQL.AspNetCore
```

Next, add and configure the services that GraphLinqQL needs. Add this to your
`ConfigureServices` function in `Startup.cs`.

```csharp
`[GraphLinqQL Services](src/GqlLinqGetStarted/Startup.cs#GraphLinqQL Services)
```

This specifies the `TypeResolver`, as generated from your schema file, which
informs query parsing. It also specifies your root query object, in this case
the `QueryResolver` we implemented earlier. Because mutations are optional, they
are specified on the options by providing the `MutationResolver` we just
implemented. Finally, GraphQL introspection is standard, but can be removed for
certain production environments if you choose.

Add the following within your ASP.Net Core 3 `UseEndpoints` declaration within
`Configure` in the same file:

```csharp
`[GraphLinqQL Services](src/GqlLinqGetStarted/Startup.cs#GraphLinqQL Endpoint)
```

This specifies the endpoint at which GraphQL will run.

Congratulations, you've done it! Now, you can run your application, use your
favorite GraphQL client tool or library, and start executing mutations and
queries against your objects!
