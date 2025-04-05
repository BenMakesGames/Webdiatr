using Microsoft.Extensions.DependencyInjection;

namespace BenMakesGames.Webdiatr.Core;

/// <summary>
/// Default implementation of ICommandr that sends requests to their handlers.
/// </summary>
internal sealed class Commandr : ICommandr
{
    private readonly IServiceProvider _serviceProvider;

    public Commandr(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var handler = _serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
        return handler.Handle(request, cancellationToken);
    }

    public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var handler = _serviceProvider.GetRequiredService<IRequestHandler<TRequest>>();
        return handler.Handle(request, cancellationToken);
    }
}
