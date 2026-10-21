namespace Proyecto.WebAPI.Models;

public sealed record ApiErrorResponse(
    int Status,
    string Code,
    string Message,
    DateTimeOffset Timestamp,
    string TraceId,
    IReadOnlyDictionary<string, string[]>? Errors = null);
