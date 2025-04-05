using BenMakesGames.Webdiatr.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BenMakesGames.Webdiatr;

/// <summary>
/// Extension methods for adding Webdiatr services to the DI container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Webdiatr services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The configuration action to configure Webdiatr.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddWebdiatr(this IServiceCollection services, Action<WebdiatrConfiguration>? configuration = null)
    {
        services.AddScoped<ICommandr, Commandr>();

        var config = new WebdiatrConfiguration();
        configuration?.Invoke(config);

        if (config.AssembliesToScan.Count > 0)
        {
            foreach (var assembly in config.AssembliesToScan)
            {
                RegisterHandlersFromAssembly(services, assembly);
            }
        }
        else
        {
            RegisterHandlersFromAssembly(services, Assembly.GetCallingAssembly());
        }

        return services;
    }

    private static void RegisterHandlersFromAssembly(IServiceCollection services, Assembly assembly)
    {
        var handlerTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType &&
                         (i.GetGenericTypeDefinition() == typeof(IRequestHandler<>) ||
                          i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))));

        foreach (var handlerType in handlerTypes)
        {
            var interfaces = handlerType.GetInterfaces()
                .Where(i => i.IsGenericType &&
                           (i.GetGenericTypeDefinition() == typeof(IRequestHandler<>) ||
                            i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)));

            foreach (var @interface in interfaces)
            {
                Console.WriteLine($"{@interface.Name}<{string.Join(", ", @interface.GenericTypeArguments.Select(a => a.Name))}> -> {handlerType.Name}");
                services.AddScoped(@interface, handlerType);
            }
        }
    }
}

/// <summary>
/// Configuration options for Webdiatr.
/// </summary>
public class WebdiatrConfiguration
{
    /// <summary>
    /// Gets or sets the assemblies to scan for request handlers.
    /// </summary>
    public List<Assembly> AssembliesToScan { get; } = new();
}
