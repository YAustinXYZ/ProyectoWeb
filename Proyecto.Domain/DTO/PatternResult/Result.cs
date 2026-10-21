using System.Text.Json.Serialization;

namespace Proyecto.Domain.DTO.PatternResult;

/// <summary>
/// Fuera de Paged, los campos de paginación quedan en null y JsonIgnore los quita del JSON:
/// un GET por id no devuelve PageNumber ni TotalPages vacíos.
/// </summary>
public class Result<T> : BaseResult
{
    [JsonPropertyOrder(1)]
    public T? Data { get; set; }

    [JsonPropertyOrder(2), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? PageNumber { get; set; }

    [JsonPropertyOrder(3), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? PageSize { get; set; }

    [JsonPropertyOrder(4), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? TotalRecords { get; set; }

    [JsonPropertyOrder(5), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? TotalPages { get; set; }

    public static Result<T> Success(T data, int status = 200, string title = "OK", string detail = "") =>
        new() { IsSuccess = true, Status = status, Title = title, Detail = detail, Data = data };

    public static Result<T> Failure(int status, string title, string detail) =>
        new() { IsSuccess = false, Status = status, Title = title, Detail = detail };

    public static Result<T> Paged(T data, int pageNumber, int pageSize, int totalRecords) =>
        new()
        {
            IsSuccess = true,
            Status = 200,
            Title = "OK",
            Data = data,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords,
            // Ceiling: 23 registros de 10 en 10 son 3 páginas, no 2
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize)
        };
}
