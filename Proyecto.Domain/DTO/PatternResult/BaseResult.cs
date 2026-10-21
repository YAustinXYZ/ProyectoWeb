namespace Proyecto.Domain.DTO.PatternResult;

/// <summary>
/// Status repite el código HTTP dentro del cuerpo, para que el frontend lo lea del JSON
/// sin tener que revisar la respuesta HTTP por aparte.
/// </summary>
public abstract class BaseResult
{
    public bool IsSuccess { get; set; }
    public int Status { get; set; }
    public string Title { get; set; } = "";
    public string Detail { get; set; } = "";
}
