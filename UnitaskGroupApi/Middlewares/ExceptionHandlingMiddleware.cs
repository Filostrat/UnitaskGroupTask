using Application.Exceptions;

using System.Net;
using System.Text.Json;

using UnitaskGroupApi.Errors;


namespace UnitaskGroupApi.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env)
    {
        _logger = logger;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception caught by middleware");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        int statusCode;
        ApiError payload;

        switch (exception)
        {
            case NotFoundException nf:
                statusCode = (int)HttpStatusCode.NotFound;
                payload = new ApiError
                {
                    Title = "Not Found",
                    Detail = nf.Message
                };
                break;

            default:
                statusCode = (int)HttpStatusCode.InternalServerError;
                payload = new ApiError
                {
                    Title = "Server Error",
                    Detail = exception.Message
                };
                break;
        }

        context.Response.StatusCode = statusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(payload, options));
    }
}