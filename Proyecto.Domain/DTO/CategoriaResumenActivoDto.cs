namespace Proyecto.Domain.DTO;

/// <summary>Categoría dentro del detalle de un activo, con el umbral que decide el mantenimiento preventivo.</summary>
public class CategoriaResumenActivoDto
{
    [System.Text.Json.Serialization.JsonPropertyName("id")]
    public int Identificador { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("name")]
    public string? Nombre { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("preventiveThresholdHours")]
    public int UmbralPreventivoHoras { get; set; }
}
