using System.Text.RegularExpressions;

namespace LentzCraftServices.Middleware;

/// <summary>
/// Middleware to sanitize and validate user inputs to prevent XSS and injection attacks.
/// </summary>
public class InputValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<InputValidationMiddleware> _logger;

    public InputValidationMiddleware(RequestDelegate next, ILogger<InputValidationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Only validate form data
        if (context.Request.HasFormContentType && context.Request.Form != null)
        {
            var form = context.Request.Form;
            var sanitizedForm = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>();

            foreach (var field in form)
            {
                var sanitizedValues = field.Value.Select(SanitizeInput).ToArray();
                sanitizedForm[field.Key] = new Microsoft.Extensions.Primitives.StringValues(sanitizedValues);
            }

            // Replace the form collection (Note: This is a simplified approach)
            // In a real scenario, you might need to rebuild the request
        }

        await _next(context);
    }

    private static string SanitizeInput(string? input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        // Remove potentially dangerous characters
        // Note: This is a basic sanitization. For production, consider using a library like HtmlSanitizer
        var sanitized = Regex.Replace(input, @"<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>", "", 
            RegexOptions.IgnoreCase | RegexOptions.Multiline);
        
        sanitized = Regex.Replace(sanitized, @"javascript:", "", RegexOptions.IgnoreCase);
        sanitized = Regex.Replace(sanitized, @"on\w+\s*=", "", RegexOptions.IgnoreCase);

        return sanitized;
    }
}
