using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BenMakesGames.Webdiatr;

/// <summary>
/// Provides extension methods for mapping MediatR requests to HTTP endpoints without controllers.
/// </summary>
public static class RequestMappingExtensions
{
    /// <summary>
    /// Maps a GET endpoint to a MediatR query that returns a response.
    /// Query parameters are automatically bound from the URL query string to the request object properties.
    /// </summary>
    /// <typeparam name="TRequest">The MediatR request type that implements IRequest{TResponse}</typeparam>
    /// <typeparam name="TResponse">The response type that will be returned from the handler</typeparam>
    /// <param name="app">The WebApplication to add the endpoint to</param>
    /// <param name="path">The URL path to map the endpoint to</param>
    /// <returns>The WebApplication for method chaining</returns>
    public static WebApplication MapGetToQuery<TRequest, TResponse>(this WebApplication app, string path) where TRequest: IRequest<TResponse>
    {
        app.MapGet(path, (IMediator mediatr, [AsParameters] TRequest request) => mediatr.Send(request));

        return app;
    }

    /// <summary>
    /// Maps a POST endpoint to a MediatR command that does not return a response.
    /// The request object is deserialized from the request body.
    /// </summary>
    /// <typeparam name="TRequest">The MediatR request type that implements IRequest</typeparam>
    /// <param name="app">The WebApplication to add the endpoint to</param>
    /// <param name="path">The URL path to map the endpoint to</param>
    /// <returns>The WebApplication for method chaining</returns>
    public static WebApplication MapPostToCommand<TRequest>(this WebApplication app, string path) where TRequest: IRequest
    {
        app.MapPost(path, (IMediator mediatr, [FromBody] TRequest request) => mediatr.Send(request));

        return app;
    }

    /// <summary>
    /// Maps a POST endpoint to a MediatR command that returns a response.
    /// The request object is deserialized from the request body.
    /// </summary>
    /// <typeparam name="TRequest">The MediatR request type that implements IRequest{TResponse}</typeparam>
    /// <typeparam name="TResponse">The response type that will be returned from the handler</typeparam>
    /// <param name="app">The WebApplication to add the endpoint to</param>
    /// <param name="path">The URL path to map the endpoint to</param>
    /// <returns>The WebApplication for method chaining</returns>
    public static WebApplication MapPostToCommand<TRequest, TResponse>(this WebApplication app, string path) where TRequest: IRequest<TResponse>
    {
        app.MapPost(path, (IMediator mediatr, [FromBody] TRequest request) => mediatr.Send(request));

        return app;
    }
}
