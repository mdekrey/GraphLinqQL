# GraphLinqQL

GraphLinqQL is designed to allow the API client to build a query plan for
custom data. In .NET, `IQueryable` is designed to do the same sort of thing.
This library is designed to combine the two by allowing the developer to easily
map an arbitrary GraphQL query into the `IQueryable` expression by using lambda
expressions and code generation from a schema to create 

# How it works

1. Create your GraphQL schema.

    This should be done in a file to be versioned with your repository.

2. Set up code generation for the interfaces.

    This generates C# classes for you to implement.

3. Implement those interfaces.

    Connect them to your Domain model using the expressions that will be
	converted into the IQueryable.

# Remaining work

- [ ] Fix naming/reorganization
- [ ] Support versions of C# prior to 8
- [ ] Determine dependencies to generate code
    - Currently, we depend on Node, but would Docker be better?
	- See how [Pegasus](https://github.com/otac0n/Pegasus) adds build tools
- [ ] Publish to NuGet, etc.
- [ ] More documentation
