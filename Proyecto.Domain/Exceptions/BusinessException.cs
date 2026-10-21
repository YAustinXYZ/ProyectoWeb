namespace Proyecto.Domain.Exceptions;

public abstract class BusinessException(string code, string message) : Exception(message)
{
    public string Code { get; } = code;
}

public sealed class EntityNotFoundException(string message)
    : BusinessException("RESOURCE_NOT_FOUND", message);

public sealed class BusinessConflictException(string message)
    : BusinessException("BUSINESS_CONFLICT", message);

public sealed class BusinessValidationException(string field, string message)
    : BusinessException("BUSINESS_VALIDATION_FAILED", message)
{
    public string Field { get; } = field;
}
