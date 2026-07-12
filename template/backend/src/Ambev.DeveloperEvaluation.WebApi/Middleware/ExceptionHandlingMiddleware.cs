using System.Net;
using System.Text.Json;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, type, error, detail) = exception switch
        {
            KeyNotFoundException ex => (
                HttpStatusCode.NotFound,
                "ResourceNotFound",
                "Resource not found",
                ex.Message),

            DomainException ex => (
                HttpStatusCode.BadRequest,
                "BusinessRuleViolation",
                "Business rule violation",
                ex.Message),

            ValidationException ex => (
                HttpStatusCode.BadRequest,
                "ValidationError",
                "Invalid input data",
                ex.Message),

            UnauthorizedAccessException ex => (
                HttpStatusCode.Unauthorized,
                "AuthenticationError",
                "Authentication failed",
                ex.Message),

            InvalidOperationException ex => (
                HttpStatusCode.Conflict,
                "InvalidOperation",
                "Invalid operation",
                ex.Message),

            _ => (
                HttpStatusCode.InternalServerError,
                "InternalServerError",
                "An unexpected error occurred",
                "An internal error occurred while processing your request")
        };

        _logger.LogError(exception, "Exception handled: {Type} - {Error}", type, error);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            type,
            error,
            detail
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
