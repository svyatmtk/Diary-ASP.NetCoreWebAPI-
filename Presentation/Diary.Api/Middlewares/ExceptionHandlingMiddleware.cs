using System.Net;
using Diary.Domain.Enum;
using Diary.Domain.Result;
using ILogger = Serilog.ILogger;

namespace Diary.Api.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception e)
        {
            await HandleExceptionAsync(e, httpContext);
        }
    }

    private async Task HandleExceptionAsync(Exception exception, HttpContext httpContext)
    {
        _logger.Error(exception, exception.Message);

        var responce = exception switch
        {
            UnauthorizedAccessException _ => new BaseResult()
                { ErrorMessage = exception.Message, ErrorCode = (int)HttpStatusCode.Unauthorized},
            _ => new BaseResult()
            {
                ErrorMessage = "Internal server error. Try again later", ErrorCode = (int)HttpStatusCode.InternalServerError
            }
        };
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = (int)responce.ErrorCode;
        await httpContext.Response.WriteAsJsonAsync(responce);
    }
}