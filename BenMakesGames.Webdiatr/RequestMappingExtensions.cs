using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BenMakesGames.Webdiatr;

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
