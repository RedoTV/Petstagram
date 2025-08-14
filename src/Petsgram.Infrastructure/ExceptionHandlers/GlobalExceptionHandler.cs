using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Petsgram.Infrastructure.ExceptionHandlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger, 
        IWebHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }
    
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        var (statusCode, title, logLevel) = exception switch
        {
            ArgumentException => (StatusCodes.Status400BadRequest, "Bad Request", LogLevel.Warning),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized", LogLevel.Warning),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error", LogLevel.Error)
        };

        _logger.Log(logLevel, exception, 
            "Unhandled exception: {ExceptionType} at {RequestPath} - {ExceptionMessage}",
            exception.GetType().Name,
            httpContext.Request.Path,
            exception.Message);

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Type = exception.GetType().Name
        };
        
        if (_environment.IsDevelopment())
            problemDetails.Detail = exception.Message;
        else
            problemDetails.Detail = "An unexpected error occurred";

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}