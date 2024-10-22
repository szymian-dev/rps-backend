using RpsApi.Models.Exceptions;

namespace RpsApi.Models.Middlewares;
using System.Net;

public class ExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandler> _logger;
    public record ExceptionResponse(HttpStatusCode StatusCode, string Description);
    
    public ExceptionHandler(RequestDelegate next, ILogger<ExceptionHandler> logger)
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
        ExceptionResponse response = exception switch
        {
            InvalidTokenException _ => new ExceptionResponse(HttpStatusCode.Unauthorized, exception.Message),
            UserNotFoundException _ => new ExceptionResponse(HttpStatusCode.BadRequest, exception.Message),
            UserAlreadyExistsException _ => new ExceptionResponse(HttpStatusCode.BadRequest, exception.Message),
            InvalidPasswordException _ => new ExceptionResponse(HttpStatusCode.BadRequest, exception.Message),
            _ => new ExceptionResponse(HttpStatusCode.InternalServerError, exception.Message)
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)response.StatusCode;
        await context.Response.WriteAsJsonAsync(response);
    }
}