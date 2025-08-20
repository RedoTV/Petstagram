using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Petsgram.Domain.Exceptions.User;

namespace Petsgram.Infrastructure.ExceptionHandlers;

public class UserExceptionHandler : IExceptionHandler
{
    private readonly ILogger<UserExceptionHandler> _logger;

    public UserExceptionHandler(ILogger<UserExceptionHandler> logger)
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
            UserNotFoundException => (StatusCodes.Status404NotFound, "User not found", LogLevel.Warning),
            UserAlreadyExistsException => (StatusCodes.Status409Conflict, "User already exists", LogLevel.Warning),
            UserValidationException => (StatusCodes.Status400BadRequest, "Validation error", LogLevel.Warning),
            _ => (0, string.Empty, LogLevel.None)
        };

        if (statusCode == 0)
            return false;
        
        _logger.Log(logLevel, exception, "User exception: {Message}", exception.Message);

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