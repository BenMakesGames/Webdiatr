using BenMakesGames.Webdiatr.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace BenMakesGames.Webdiatr;

/// <summary>
/// Provides extension methods for mapping requests to HTTP endpoints without controllers.
/// </summary>
public static class RequestMappingExtensions
{
    /// <summary>
    /// Maps a GET endpoint to a query that returns a response.
    /// Query parameters are automatically bound from the URL query string to the request object properties.
    /// </summary>
    /// <typeparam name="TRequest">The request type that implements IRequest{TResponse}</typeparam>
    /// <typeparam name="TResponse">The response type that will be returned from the handler</typeparam>
    /// <param name="app">The WebApplication to add the endpoint to</param>
    /// <param name="path">The URL path to map the endpoint to</param>
    /// <returns>The WebApplication for method chaining</returns>
    public static WebApplication MapGetToQuery<TRequest, TResponse>(this WebApplication app, string path)
    {
        using (var scope = app.Services.CreateScope())
        {
            if(scope.ServiceProvider.GetService<IRequestHandler<TRequest, TResponse>>() is null) throw new ArgumentException($"No handler of type IRequestHandler<{typeof(TRequest).Name}, {typeof(TResponse).Name}> was registered");
        }

        app.MapGet(path, (ICommandr commandr, [AsParameters] TRequest request, CancellationToken ctx) => commandr.Send<TRequest, TResponse>(request, ctx));

        return app;
    }

    /// <summary>
    /// Maps a POST endpoint to a command that does not return a response.
    /// The request object is deserialized from the request body.
    /// </summary>
    /// <typeparam name="TRequest">The request type that implements IRequest</typeparam>
    /// <param name="app">The WebApplication to add the endpoint to</param>
    /// <param name="path">The URL path to map the endpoint to</param>
    /// <returns>The WebApplication for method chaining</returns>
    public static WebApplication MapPostToCommand<TRequest>(this WebApplication app, string path)
    {
        using (var scope = app.Services.CreateScope())
        {
            if(scope.ServiceProvider.GetService<IRequestHandler<TRequest>>() is null) throw new ArgumentException($"No handler of type IRequestHandler<{typeof(TRequest).Name}> was registered");
        }

        app.MapPost(path, (ICommandr commandr, [FromBody] TRequest request, CancellationToken ctx) => commandr.Send<TRequest>(request, ctx));

        return app;
    }

    /// <summary>
    /// Maps a POST endpoint to a command that returns a response.
    /// The request object is deserialized from the request body.
    /// </summary>
    /// <typeparam name="TRequest">The request type that implements IRequest{TResponse}</typeparam>
    /// <typeparam name="TResponse">The response type that will be returned from the handler</typeparam>
    /// <param name="app">The WebApplication to add the endpoint to</param>
    /// <param name="path">The URL path to map the endpoint to</param>
    /// <returns>The WebApplication for method chaining</returns>
    public static WebApplication MapPostToCommand<TRequest, TResponse>(this WebApplication app, string path)
    {
        using (var scope = app.Services.CreateScope())
        {
            if(scope.ServiceProvider.GetService<IRequestHandler<TRequest, TResponse>>() is null) throw new ArgumentException($"No handler of type IRequestHandler<{typeof(TRequest).Name}, {typeof(TResponse).Name}> was registered");
        }

        app.MapPost(path, (ICommandr commandr, [FromBody] TRequest request, CancellationToken ctx) => commandr.Send<TRequest, TResponse>(request, ctx));

        return app;
    }
}
