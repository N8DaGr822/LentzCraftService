using System.Net;
using System.Text.Json;

namespace LentzCraftServices.Middleware;

/// <summary>
/// Global exception handler middleware to catch and handle unhandled exceptions.
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred. RequestId: {RequestId}", 
                context.TraceIdentifier);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = new
        {
            error = new
            {
                message = _environment.IsDevelopment() 
                    ? exception.Message 
                    : "An error occurred while processing your request.",
                requestId = context.TraceIdentifier,
                statusCode = context.Response.StatusCode
            }
        };

        // If it's a Blazor page request, redirect to error page
        if (context.Request.Path.StartsWithSegments("/") && 
            !context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.Redirect("/500");
            return;
        }

        var jsonResponse = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(jsonResponse);
    }
}
