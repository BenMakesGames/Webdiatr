namespace BenMakesGames.Webdiatr.Core;

/// <summary>
/// Defines a handler for a request with no response
/// </summary>
/// <typeparam name="TRequest">The type of request being handled</typeparam>
public interface IRequestHandler<in TRequest>
{
    /// <summary>
    /// Handles a request
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A task that represents the handle operation</returns>
    Task Handle(TRequest request, CancellationToken cancellationToken);
}

/// <summary>
/// Defines a handler for a request with a response
/// </summary>
/// <typeparam name="TRequest">The type of request being handled</typeparam>
/// <typeparam name="TResponse">The type of response from the handler</typeparam>
public interface IRequestHandler<in TRequest, TResponse>
{
    /// <summary>
    /// Handles a request
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A task that represents the handle operation. The task result contains the handler response</returns>
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
