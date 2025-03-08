using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace BenMakesGames.Webdiatr;

/// <summary>
/// Provides extension methods for mapping exceptions to HTTP responses.
/// Allows for easy configuration of exception handling middleware that converts exceptions to appropriate HTTP results.
/// </summary>
public static class ExceptionMappingExtensions
{
    private static readonly Dictionary<Type, Func<Exception, IResult>> ExceptionHandlers = new();
    private static bool MiddlewareRegistered;

    /// <summary>
    /// Maps an exception type to a specific HTTP response handler.
    /// When an exception of the specified type (or a derived type) is thrown, the handler will be invoked to generate the HTTP response.
    /// </summary>
    /// <typeparam name="TException">The type of exception to handle</typeparam>
    /// <param name="app">The WebApplication to add the exception mapping to</param>
    /// <param name="handler">A function that takes the exception and returns an IResult representing the HTTP response</param>
    /// <returns>The WebApplication for method chaining</returns>
    public static WebApplication MapException<TException>(this WebApplication app, Func<TException, IResult> handler) where TException : Exception
    {
        if (!MiddlewareRegistered)
        {
            UseExceptionMapping(app);
            MiddlewareRegistered = true;
        }

        ExceptionHandlers[typeof(TException)] = ex => handler((TException)ex);
        return app;
    }

    /// <summary>
    /// Configures the exception handling middleware that processes mapped exceptions.
    /// This is automatically called by MapException when the first exception mapping is registered.
    /// </summary>
    /// <param name="app">The WebApplication to configure the middleware for</param>
    private static void UseExceptionMapping(WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                var actualException = ex.InnerException ?? ex;
                var exceptionType = actualException.GetType();

                // Find the most specific matching handler by walking up the type hierarchy
                var currentType = exceptionType;
                while (currentType != null)
                {
                    if (ExceptionHandlers.TryGetValue(currentType, out var exceptionHandler))
                    {
                        var result = exceptionHandler(actualException);
                        await result.ExecuteAsync(context);
                        return;
                    }
                    currentType = currentType.BaseType;
                }
                
                throw;
            }
        });
    }
}
