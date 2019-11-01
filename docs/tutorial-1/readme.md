# Getting Started with GraphLinqQL and EF Core

https://docs.microsoft.com/en-us/ef/core/get-started/

```powershell
dotnet new console -o GqlLinqGetStarted
cd GqlLinqGetStarted
```

All the EFCore stuff... Data folder and Startup...

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

```powershell
dotnet add package GraphLinqQL.Resolvers
```

Each resolver...

```csharp
internal class BlogResolver : Blog.GraphQlContract<Data.Blog>
{
    public override IGraphQlScalarResult<string> Id() =>
        this.Resolve(blog => blog.BlogId.ToString());

    public override IGraphQlObjectResult<IEnumerable<Post>> Posts() =>
        this.Resolve(blog => blog.Posts).Nullable(_ => _.List(post => post.AsContract<PostResolver>()));

    public override IGraphQlScalarResult<string> Url() =>
        this.Resolve(blog => blog.Url);
}
```


```powershell
dotnet add package GraphLinqQL.AspNetCore
```

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

