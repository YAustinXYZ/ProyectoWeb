using Microsoft.AspNetCore.Mvc;

namespace Proyecto.WebAPI.Controllers;

public abstract class LoggedApiController<TController>(ILogger<TController> logger) : ControllerBase
{
    protected void LogStarted(string operation) =>
        logger.LogInformation(
            "Starting {Operation}. TraceId: {TraceId}",
            operation, HttpContext.TraceIdentifier);

    protected void LogCompleted(string operation, int? resourceId = null) =>
        logger.LogInformation(
            "Completed {Operation} for resource {ResourceId}. TraceId: {TraceId}",
            operation, resourceId, HttpContext.TraceIdentifier);
}
