namespace Proyecto.Domain.DTO;

public class CategoriaActivoListaDto
{
    [System.Text.Json.Serialization.JsonPropertyName("id")]
    public int Identificador { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("name")]
    public string? Nombre { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("codePrefix")]
    public string? PrefijoCodigo { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("preventiveThresholdHours")]
    public int UmbralPreventivoHoras { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("isActive")]
    public bool EstaActivo { get; set; }
    // Calculado: conteo hecho por SQL Server con GROUP BY, sin cargar los activos
    [System.Text.Json.Serialization.JsonPropertyName("assetCount")]
    public int CantidadActivos { get; set; }
}
