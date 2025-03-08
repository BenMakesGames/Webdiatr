using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace BenMakesGames.Webdiatr;

public static class ExceptionMappingExtensions
{
    private static readonly Dictionary<Type, Func<Exception, IResult>> ExceptionHandlers = new();
    private static bool MiddlewareRegistered;

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
