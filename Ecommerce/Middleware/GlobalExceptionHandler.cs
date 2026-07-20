using Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

internal sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IHostEnvironment env)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context, Exception exception, CancellationToken ct)
    {
        var (status, title) = exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, "Not Found"),
            UnAuthorizedException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            ForbbidenException => (StatusCodes.Status403Forbidden, "Forbidden"),
            BadRequestException => (StatusCodes.Status400BadRequest, "Bad Request"),
            ConflictException => (StatusCodes.Status409Conflict, "Conflict"),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
        };

        if (status == StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Unhandled exception occurred");

        var detail = status == StatusCodes.Status500InternalServerError
            ? env.IsDevelopment()
                ? exception.ToString()        // full stacktrace in dev
                : "An unexpected error occurred."
            : exception.Message;

        context.Response.StatusCode = status;
        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail
        }, ct);

        return true;
    }
}