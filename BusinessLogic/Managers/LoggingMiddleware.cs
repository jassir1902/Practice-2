using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using UPB.BusinessLogic.Exceptions;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            _logger.LogInformation("Incoming request: {Method} {Path}", context.Request.Method, context.Request.Path);
            await _next(context);
            _logger.LogInformation("Outgoing response: {StatusCode}", context.Response.StatusCode);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "text/plain";

        string errorMessage;
        int statusCode;

        switch (exception)
        {
            case NotFoundException notFoundEx:
                errorMessage = notFoundEx.Message;
                statusCode = StatusCodes.Status404NotFound;
                _logger.LogError(exception, "Resource not found: {ErrorMessage}", errorMessage);
                break;
            case ValidationException validationEx:
                errorMessage = validationEx.Message;
                statusCode = StatusCodes.Status400BadRequest;
                _logger.LogError(exception, "Validation failed: {ErrorMessage}", errorMessage);
                break;
            default:
                errorMessage = "Internal server error";
                statusCode = StatusCodes.Status500InternalServerError;
                _logger.LogError(exception, "Internal server error: {ErrorMessage}", errorMessage);
                break;
        }

        context.Response.StatusCode = statusCode;
        return context.Response.WriteAsync(errorMessage);
    }
}

public static class LoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseLoggingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<LoggingMiddleware>();
    }
}
