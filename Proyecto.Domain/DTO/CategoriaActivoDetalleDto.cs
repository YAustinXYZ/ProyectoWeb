namespace Proyecto.Domain.DTO;

public class CategoriaActivoDetalleDto
{
    [System.Text.Json.Serialization.JsonPropertyName("id")]
    public int Identificador { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("name")]
    public string? Nombre { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("description")]
    public string? Descripcion { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("codePrefix")]
    public string? PrefijoCodigo { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("preventiveThresholdHours")]
    public int UmbralPreventivoHoras { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("isActive")]
    public bool EstaActivo { get; set; }
    // Calculado: conteo hecho por SQL Server con GROUP BY, sin cargar los activos
    [System.Text.Json.Serialization.JsonPropertyName("assetCount")]
    public int CantidadActivos { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("assets")]
    public List<ActivoResumenCategoriaDto> Activos { get; set; } = [];
}
