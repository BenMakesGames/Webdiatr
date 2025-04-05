using System.Threading.Tasks;

namespace BenMakesGames.Webdiatr.Core;

/// <summary>
/// Interface for sending requests to their handlers.
/// </summary>
internal interface ICommandr
{
    /// <summary>
    /// Asynchronously send a request to a single handler
    /// </summary>
    /// <typeparam name="TRequest">Request type</typeparam>
    /// <typeparam name="TResponse">Response type</typeparam>
    /// <param name="request">Request object</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A task that represents the send operation. The task result contains the handler response</returns>
    Task<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously send a request to a single handler that does not return a response
    /// </summary>
    /// <param name="request">Request object</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A task that represents the send operation</returns>
    Task Send<TRequest>(TRequest request, CancellationToken cancellationToken);
}
