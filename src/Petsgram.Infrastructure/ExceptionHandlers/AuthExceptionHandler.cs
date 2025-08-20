using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Petsgram.Domain.Exceptions.Auth;

namespace Petsgram.Infrastructure.ExceptionHandlers;

public class AuthExceptionHandler : IExceptionHandler
{
    private readonly ILogger<AuthExceptionHandler> _logger;

    public AuthExceptionHandler(ILogger<AuthExceptionHandler> logger)
    {
        _logger = logger;
    }
    
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        var (statusCode, title, logLevel) = exception switch
        {
            AuthenticationException => (StatusCodes.Status401Unauthorized, "Authentication failed", LogLevel.Warning),
            TokenValidationException => (StatusCodes.Status401Unauthorized, "Token validation failed", LogLevel.Warning),
            _ => (0, string.Empty, LogLevel.None)
        };

        if (statusCode == 0)
            return false;
        
        _logger.Log(logLevel, exception, "Authentication exception: {Message}", exception.Message);

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = exception.Message,
            Type = exception.GetType().Name
        };
        
        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}