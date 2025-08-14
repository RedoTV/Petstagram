using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Petsgram.Domain.Exceptions.PetType;

namespace Petsgram.Infrastructure.ExceptionHandlers;

public class PetTypeExceptionHandler : IExceptionHandler
{
    private readonly ILogger<PetTypeExceptionHandler> _logger;

    public PetTypeExceptionHandler(ILogger<PetTypeExceptionHandler> logger)
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
            PetTypeNotFoundException => (StatusCodes.Status404NotFound, "Pet type not found", LogLevel.Warning),
            PetTypeAlreadyExistsException => (StatusCodes.Status409Conflict, "Pet type already exists", LogLevel.Warning),
            PetTypeValidationException => (StatusCodes.Status400BadRequest, "Validation error", LogLevel.Warning),
            _ => (0, string.Empty, LogLevel.None)
        };

        if (statusCode == 0)
            return false;
        
        _logger.Log(logLevel, exception, "Pet type exception: {Message}", exception.Message);

        var problemDetails = new ProblemDetails()
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