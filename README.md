# Webdiatr

I made this library because if I have to participate in one more discussion about unit testing controller endpoints, I'm going to murder something.

Better put me on a watchlist, though: we primarily use PHP at work, and won't be doing anything like what this library does any time soon.

## What is it?

Webdiatr is an opinionated "framework" (it's ~80 lines of code) to support writing vertical-slice-y, RPC-style web APIs with MediatR.

You will write MediatR handlers that needn't have ever heard of this thing called "the internet", making reuse and testing easier. Best of all: you won't write a single ASP.NET controller endpoint, completing eliminating any discussion about how best to test them.

Less is more. You don't have to test what doesn't exist.

## How to structure your app

```
PROJECT_ROOT/
├── MyApp.SomeFeatureSlice
│   ├── Exceptions (or install OneOf if you hate throwing exceptions)
│   │   ├── MyAppSliceException.cs
│   │   └── MyOtherAppSliceException.cs
│   ├── Handlers (each file here contains a MediatR request, handler, and response)
│   │   ├── CreateThing.cs
│   │   ├── SearchThings.cs
│   │   └── TakeSomeSpecificActionOnAThing.cs
│   └── AssemblyMarker.cs (if you like - some people love these things)
├── MyApp.SomeOtherFeatureSlice
│   └── etc.
├── etc.
└── MyApp.Web (depends on the Webdiatr library, and your slices)
    └── Program.cs
```

### MediatR handlers

Here's an example of what one of your handler `.cs` files might look like. It contains the handler, and request and response DTOs.

```c#
public sealed class GetFruit(
    IFruitRepository repository
): IRequestHandler<GetFruitRequest, GetFruitResponse>
{
    public Task<GetFruitResponse> Handle(GetFruitRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new GetFruitResponse
        {
            Fruit = repository.GetFruit().ToList()
        });
    }
}

// this particular request has no body
public sealed class GetFruitRequest : IRequest<GetFruitResponse>;

public sealed class GetFruitResponse
{
    public required List<string> Fruit { get; init; }
}
```

### Program.cs

```c#
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<MyApp.SomeFeatureSlice.AssemblyMarker>();
});

builder.Services.AddSingleton<IFruitRepository, FruitRepository>();

var app = builder.Build();

app.MapGet("/", () => "Hi! :D"); // 👋

app
    .MapGetToQuery<GetFruitRequest, GetFruitResponse>("/fruit") // example above
    .MapPostToCommand<AddFruitRequest>("/fruit") // you'll have to imagine this one
;

app.Run();
```

`.MapGetToQuery` and `.MapPostToCommand` are extension methods provided by Webdiatr. They are very small, and you could write them yourself if you don't want to depand on this library.

Here they are:

```c#
public static class RequestMappingExtensions
{
    public static WebApplication MapGetToQuery<TRequest, TResponse>(this WebApplication app, string path) where TRequest: IRequest<TResponse>
    {
        app.MapGet(path, (IMediator mediatr, [AsParameters] TRequest request) => mediatr.Send(request));

        return app;
    }

    public static WebApplication MapPostToCommand<TRequest>(this WebApplication app, string path) where TRequest: IRequest
    {
        app.MapPost(path, (IMediator mediatr, [FromBody] TRequest request) => mediatr.Send(request));

        return app;
    }

    public static WebApplication MapPostToCommand<TRequest, TResponse>(this WebApplication app, string path) where TRequest: IRequest<TResponse>
    {
        app.MapPost(path, (IMediator mediatr, [FromBody] TRequest request) => mediatr.Send(request));

        return app;
    }
}
```

### Mapping exceptions (or using OneOf)

If throwing exceptions for things like "entity not found" doesn't bother you, Webdiatr provides extension methods for easily mapping exceptions to HTTP responses.

For example, supposing you created an app-specific base exception class called `AppException`, and an `EntityAlreadyExistsException` which extends it, toss this in your `Program.cs`:

```c#
app
    .MapException<EntityAlreadyExistsException>(ex => Results.Conflict(new { type = ex.EntityType, id = ex.Identifier, error = ex.Message }))
    .MapException<AppException>(ex => Results.BadRequest(new { error = ex.Message }))
;
```

But if you hate throwing exceptions, I get it, and you don't need me to tell you to use [OneOf](https://github.com/mcintyre321/OneOf) - you've already asked Claude or ChatGPT how to write some middleware to convert any and all `OneOf<T, MyBaseErrorClass>` to HTTP responses, or maybe are considering writing your own `MapGetToQuery` to handle it... either way, you're good to go. You don't need me. Heck, you don't even need this library.

### Request validation?

Webdiatr doesn't have opinions about this. You could:

* Use FluentValidation, hoping they don't change their license and start asking for hundreds of dollars per dev per year
* Hook up MediatR pipelines
* Go old-school with `System.ComponentModel.DataAnnotations` and call `Validator.ValidateObject` in your handlers

The choice is yours.
