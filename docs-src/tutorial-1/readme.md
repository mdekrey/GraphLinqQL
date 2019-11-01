# Getting Started with GraphLinqQL and EF Core

https://docs.microsoft.com/en-us/ef/core/get-started/

```powershell
dotnet new console -o GqlLinqGetStarted
cd GqlLinqGetStarted
```

All the EFCore stuff... Data folder and Startup...

```GraphQL
`[GraphQL Schema](src/GqlLinqGetStarted/Api/blogging-schema.graphql)
```

```powershell
dotnet add package GraphLinqQL.Resolvers
```

Each resolver...

```csharp
`[BlogResolver](src/GqlLinqGetStarted/Api/BlogResolver.cs#BlogResolver)
```


```powershell
dotnet add package GraphLinqQL.AspNetCore
```

```csharp
`[GraphLinqQL Services](src/GqlLinqGetStarted/Startup.cs#GraphLinqQL Services)
```

```csharp
`[GraphLinqQL Services](src/GqlLinqGetStarted/Startup.cs#GraphLinqQL Endpoint)
```
