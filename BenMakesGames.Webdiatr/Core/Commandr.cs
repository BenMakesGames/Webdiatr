using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace BenMakesGames.Webdiatr.Core;

/// <summary>
/// Default implementation of ICommandr that sends requests to their handlers.
/// </summary>
public class Commandr : ICommandr
{
    private readonly IServiceProvider _serviceProvider;
    private static readonly Dictionary<Type, Type> _handlerTypes = new();

    public Commandr(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var handler = GetHandler<IRequestHandler<IRequest<TResponse>, TResponse>>(request.GetType());
        return handler.Handle((dynamic)request, cancellationToken);
    }

    public Task Send(IRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var handler = GetHandler<IRequestHandler<IRequest>>(request.GetType());
        return handler.Handle((dynamic)request, cancellationToken);
    }

    private THandler GetHandler<THandler>(Type requestType)
    {
        var handlerType = GetHandlerType(requestType);
        var handler = (THandler)_serviceProvider.GetRequiredService(handlerType);
        return handler;
    }

    private static Type GetHandlerType(Type requestType)
    {
        if (_handlerTypes.TryGetValue(requestType, out var handlerType))
            return handlerType;

        var requestInterface = requestType.GetInterfaces()
            .FirstOrDefault(i =>
                i.IsGenericType && (
                    i.GetGenericTypeDefinition() == typeof(IRequest<>) ||
                    i.GetGenericTypeDefinition() == typeof(IRequest)
                )
            )
        ;

        if (requestInterface == null)
            throw new ArgumentException($"Type {requestType.Name} does not implement IRequest or IRequest<TResponse>");

        var responseType = requestInterface.GetGenericArguments().FirstOrDefault();
        var handlerInterface = responseType != null
            ? typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType)
            : typeof(IRequestHandler<>).MakeGenericType(requestType);

        _handlerTypes[requestType] = handlerInterface;
        return handlerInterface;
    }
}
