# Getting Started with GraphLinqQL and EF Core

Using GraphLinqQL with EF Core is fairly straightforward, but if you're
unfamiliar with either, you should first go through both [EF Core Getting
Started Tutorial](https://docs.microsoft.com/en-us/ef/core/get-started/) or
[GraphLinqQL Getting Started Tutorial](../tutorial-1), but we'll cover most of
the same material here, just not quite as in-depth.

We'll start by creating the project and adding EFCore and GraphLinqQL.

```powershell
dotnet new web -o GqlLinqGetStarted
cd GqlLinqGetStarted
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```

Compared to our previous GraphLinqQL tutorial, we have new contents in our Data
folder. Here's our `Data/Model.cs`:

```csharp
namespace GqlLinqGetStarted.Data
{
    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=blogging.db");
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }

        public List<Post> Posts { get; } = new List<Post>();
    }

    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }
}
```

As you can see, this is a basic DbContext and the corresponding models,
including auto-generated sequential IDs. GraphLinqQL
works with Plain Old CLR Objects (POCOs) so that you don't need to rewrite your
existing Domain Model to get started. Once this is in place, we should start by
defining a GraphQL API.

> API-Driven development is important for quality APIs. Thinking about how your
> end-user developers will invoke the API is much more important than making the
> implementation easy, because your end-users are often customers! Even when API
> consumers are internal to your company, having a schema separate from the code
> makes it easier to maintain, easier to prevent breaking changes, and more
> straightforward to document - or even rewrite if you want to change your
> underlying implementation, such as changes from domain objects to accessing EF
> Core directly!

We'll use the same GraphQL schema definition from the previous tutorial. Place
it in a `.graphql` file within the Api folder. You can learn more about [GraphQL
schemas from the official documentation](https://graphql.org/learn/schema/).

```GraphQL
schema {
	query: Query
	mutation: Mutation
}

type Query {
	blogs: [Blog!]
}

type Blog {
	id: ID!
	url: String
	posts: [Post!]
}

type Post {
	id: ID!
	title: String
	content: String
}

type Mutation {
	addBlog(url: String!): Blog!
	addPost(blogId: ID!, post: NewPost!): Post!
}

input NewPost {
	title: String!
	post_content: String!
}

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

Our resolvers won't be much different than the previous tutorial. Make a new
class called `QueryResolver` in the `API` folder, and add the following base
class to it:

```csharp
public class QueryResolver : Query.GraphQlContract<GraphQlRoot>
```

We used `GraphQlRoot` for the generic parameter because this is used at the root
of the GraphQL Query.

At this point, you can use your editor to implement the class. You should get
the methods that correspond to the GraphQL fields - in this case, `Blogs`, with
a return type of `IGraphQlObjectResult<IEnumerable<Blog>?>`. The `Blog` class
returned from our API is actually the class generated from the schema, and we
know that we need to return a list. However, before we can implement it, we need
our `DbContext`. Fortunately, GraphLinqQL integrates with ASP.NET Core's
dependency injection. Include the following lines for our constructor.

```csharp
private readonly Data.BloggingContext context;

public QueryResolver(Data.BloggingContext context)
{
    this.context = context;
}
```

We can now implement the method as follows:

```csharp
public class QueryResolver : Query.GraphQlContract<GraphQlRoot>
```

Because this returns an `IQueryable<T>`, GraphLinqQL automatically uses this to
build LINQ queries behind the scenes. This results in custom-made SQL
corresponding to your end-users' requests when used in this way.

Because the LINQ context is continued, our resolvers don't need to know anything
about the `DbContext` - those concerns are kept completely separately from these
resolvers. As a result, our next few resolvers are identical to the previous
tutorial; please use the links at the top of this tutorial for more information.

`BlogResolver.cs`:

```csharp
internal class BlogResolver : Blog.GraphQlContract<Data.Blog>
{
    public override IGraphQlScalarResult<string> Id() =>
        this.Resolve(blog => blog.BlogId.ToString());

    public override IGraphQlScalarResult<string> Url() =>
        this.Resolve(blog => blog.Url);

    public override IGraphQlObjectResult<IEnumerable<Post>?> Posts() =>
        this.Resolve(blog => blog.Posts).Nullable(_ => _.List(post => post.AsContract<PostResolver>()));
}
```

`PostResolver.cs`:

```csharp
internal class PostResolver : Post.GraphQlContract<Data.Post>
{
    public override IGraphQlScalarResult<string> Content() =>
        this.Resolve(post => post.Content);

    public override IGraphQlScalarResult<string> Id() =>
        this.Resolve(post => post.PostId.ToString());

    public override IGraphQlScalarResult<string> Title() =>
        this.Resolve(post => post.Title);
}
```

Our final type is the `Mutation`, though not all schemas provide them. We could
only partially implement our schema by not implementing this base class, but
that would be contrary to our Schema document. Again, we'll use the
`GraphQlRoot` for the generic parameter, resulting in this declaration, and
we'll include the dependency injection of the EF Core `DbContext`.

```csharp
public class MutationResolver : Mutation.GraphQlContract<GraphLinqQL.GraphQlRoot>
{
    private readonly Data.BloggingContext context;

    public MutationResolver(Data.BloggingContext context)
    {
        this.context = context;
    }
```

Implementing this class using our IDE gives us two additional methods, one for
each field in our GraphQL schema. We'll start with `AddBlog`.

```csharp
public override IGraphQlObjectResult<Blog?> AddBlog(string url) =>
    this.ResolveTask(async _ =>
    {
        var blog = new Data.Blog { Url = url };
        context.Add(blog);
        await context.SaveChangesAsync();
        return blog;
    }).Nullable(blog => blog.AsContract<BlogResolver>());
```

Here, we use `ResolveTask` to allow us to use .Net's Task framework to write
imperative code, since we'll be using the `BloggingContext` we injected earlier.
You can see that AddBlog recieves the GraphQL field parameters as method
parameters automatically. The `_` represents the root object, which is unused in
this example. Other than that, we use the normal EF Core process for adding an
object to the database. We can then return that object for further use with
GraphQL.

Similarly, we can implement `AddPost`.

```csharp
public override IGraphQlObjectResult<Post?> AddPost(string blogId, NewPost newPost) =>
    this.ResolveTask(async _ =>
    {
        var post = new Data.Post { BlogId = int.Parse(blogId), Title = newPost.Title, Content = newPost.PostContent };
        context.Add(post);
        await context.SaveChangesAsync();
        return post;
    }).Nullable(post => post.AsContract<PostResolver>());
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
services.AddGraphQl<Api.TypeResolver>(typeof(Api.QueryResolver), options =>
{
    options.Mutation = typeof(Api.MutationResolver);
    options.AddIntrospection();
});
```

This specifies the `TypeResolver`, as generated from your schema file, which
informs query parsing. It also specifies your root query object, in this case
the `QueryResolver` we implemented earlier. Because mutations are optional, they
are specified on the options by providing the `MutationResolver` we just
implemented. Finally, GraphQL introspection is standard, but can be removed for
certain production environments if you choose.

EF Core also needs to be initialized for our demo. For that, we're going to add
a `DbContextInitializingHostedService`. `HostedService`s are a .NET Core
capability that allows a task to be run at application startup, potentially so
that it may continue running; ours will end. This wouldn't necessarily be used
for production, but it gets the job done for our test environment.

```csharp
internal class DbContextInitializingHostedService : IHostedService
{
    private readonly IServiceProvider serviceProvider;

    public DbContextInitializingHostedService(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<Data.BloggingContext>();
        await dbContext.Database.EnsureCreatedAsync().ConfigureAwait(false);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
```

Going back to our `Startup.cs`, we'll add these additional services to the
`ConfigureServices` function in `Startup.cs`.

```csharp
services.AddDbContext<Data.BloggingContext>();
services.AddHostedService<Data.DbContextInitializingHostedService>();
```

In our case, this is all we need to use a SQLite EF Core database and have it
created for us!

Finally, we need to add our actual GraphQL endpoint. Add the following within
your ASP.Net Core 3 `UseEndpoints` declaration within `Configure` in the same
file:

```csharp
endpoints.UseGraphQl("/graphql");
```

Congratulations, you've done it! Now, you can run your application, use your
favorite GraphQL client tool or library, and start executing mutations and
queries against your objects!


```csharp
services.AddGraphQl<Api.TypeResolver>(typeof(Api.QueryResolver), options =>
{
    options.Mutation = typeof(Api.MutationResolver);
    options.AddIntrospection();
});
```

```csharp
endpoints.UseGraphQl("/graphql");
```


