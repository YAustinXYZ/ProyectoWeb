using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Proyecto.Domain.Exceptions;
using Proyecto.WebAPI.Models;

namespace Proyecto.WebAPI.Middleware;

public sealed class GlobalExceptionMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (BusinessException exception)
        {
            var status = exception switch
            {
                BusinessValidationException => StatusCodes.Status400BadRequest,
                EntityNotFoundException => StatusCodes.Status404NotFound,
                BusinessConflictException => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status400BadRequest
            };

            logger.LogWarning(
                "Business request failure {Code} for {Method} {Path}. TraceId: {TraceId}",
                exception.Code, context.Request.Method, context.Request.Path, context.TraceIdentifier);

            var errors = exception is BusinessValidationException validation
                ? new Dictionary<string, string[]> { [validation.Field] = [exception.Message] }
                : null;
            await WriteErrorAsync(context, status, exception.Code, exception.Message, errors);
        }
        catch (DbUpdateException exception) when (exception.InnerException is SqlException { Number: 2601 or 2627 })
        {
            logger.LogWarning(
                "Unique constraint conflict for {Method} {Path}. TraceId: {TraceId}",
                context.Request.Method, context.Request.Path, context.TraceIdentifier);

            await WriteErrorAsync(
                context,
                StatusCodes.Status409Conflict,
                "UNIQUE_CONSTRAINT_VIOLATION",
                "El registro entra en conflicto con un valor que debe ser único.");
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Unhandled request failure for {Method} {Path}. TraceId: {TraceId}",
                context.Request.Method, context.Request.Path, context.TraceIdentifier);

            await WriteErrorAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "INTERNAL_SERVER_ERROR",
                "Ocurrió un error inesperado. Use el identificador de seguimiento para solicitar asistencia.");
        }
    }

    private async Task WriteErrorAsync(
        HttpContext context,
        int status,
        string code,
        string message,
        IReadOnlyDictionary<string, string[]>? errors = null)
    {
        if (context.Response.HasStarted)
        {
            logger.LogError(
                "Cannot write error response because the response has started. TraceId: {TraceId}",
                context.TraceIdentifier);
            return;
        }

        context.Response.Clear();
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new ApiErrorResponse(
            status,
            code,
            message,
            DateTimeOffset.UtcNow,
            context.TraceIdentifier,
            errors));
    }
}
