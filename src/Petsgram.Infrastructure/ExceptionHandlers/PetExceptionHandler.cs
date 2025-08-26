using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Petsgram.Domain.Exceptions.Pet;

namespace Petsgram.Infrastructure.ExceptionHandlers;

public class PetExceptionHandler : IExceptionHandler
{
    private readonly ILogger<PetExceptionHandler> _logger;

    public PetExceptionHandler(ILogger<PetExceptionHandler> logger)
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
            PetNotFoundException => (StatusCodes.Status404NotFound, "Pet not found", LogLevel.Warning),
            PetUnauthorizedException => (StatusCodes.Status401Unauthorized, "Unauthorized", LogLevel.Warning),
            PetValidationException => (StatusCodes.Status400BadRequest, "Validation error", LogLevel.Warning),
            _ => (0, string.Empty, LogLevel.None)
        };

        if (statusCode == 0)
            return false;
        
        _logger.Log(logLevel, exception, "Pet exception: {Message}", exception.Message);

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