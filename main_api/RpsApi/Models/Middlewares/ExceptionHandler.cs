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
            UserNotFoundException _ when context.Request.Method == HttpMethods.Get 
                => new ExceptionResponse(HttpStatusCode.NotFound, exception.Message),
            UserNotFoundException _ => new ExceptionResponse(HttpStatusCode.BadRequest, exception.Message),
            UserAlreadyExistsException _ => new ExceptionResponse(HttpStatusCode.BadRequest, exception.Message),
            InvalidPasswordException _ => new ExceptionResponse(HttpStatusCode.BadRequest, exception.Message),
            GameNotFoundException _ when context.Request.Method == HttpMethods.Get 
                => new ExceptionResponse(HttpStatusCode.NotFound, exception.Message),
            GameNotFoundException _ => new ExceptionResponse(HttpStatusCode.BadRequest, exception.Message),
            InvalidGameStatusException _ => new ExceptionResponse(HttpStatusCode.BadRequest, exception.Message),
            UnauthorizedAccessException _ => new ExceptionResponse(HttpStatusCode.Unauthorized, exception.Message),
            InvalidGameException _ => new ExceptionResponse(HttpStatusCode.BadRequest, exception.Message),
            FileExtensionNotAllowedException _ => new ExceptionResponse(HttpStatusCode.BadRequest, exception.Message),
            InvalidGameStateException _ => new ExceptionResponse(HttpStatusCode.BadRequest, exception.Message),
            NotFoundException _ => new ExceptionResponse(HttpStatusCode.NotFound, exception.Message),
            InvalidFilterStateException _ => new ExceptionResponse(HttpStatusCode.BadRequest, exception.Message),
            ForbiddenAccessException _ => new ExceptionResponse(HttpStatusCode.Forbidden, exception.Message),
            UnprocessableEntityException _ => new ExceptionResponse(HttpStatusCode.UnprocessableEntity, exception.Message),
            BadRequestException _ => new ExceptionResponse(HttpStatusCode.BadRequest, exception.Message),
            _ => new ExceptionResponse(HttpStatusCode.InternalServerError, exception.Message)
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)response.StatusCode;
        await context.Response.WriteAsJsonAsync(response);
    }
}