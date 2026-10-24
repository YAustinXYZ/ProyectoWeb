namespace Proyecto.Domain.DTO;

public class ActivoListaDto
{
    [System.Text.Json.Serialization.JsonPropertyName("id")]
    public int Identificador { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("assetCode")]
    public string? CodigoActivo { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("name")]
    public string? Nombre { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("status")]
    public string? Estado { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("criticality")]
    public string? Criticidad { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("categoryId")]
    public int CategoriaActivoId { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("categoryName")]
    public string? NombreCategoria { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("locationName")]
    public string? NombreUbicacion { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("imageUrl")]
    public string? UrlImagen { get; set; }

    // Campos calculados: no existen como columnas
    // Meses completos desde la fecha de adquisición (mapper)
    [System.Text.Json.Serialization.JsonPropertyName("ageInMonths")]
    public int MesesAntiguedad { get; set; }
    // Conteo hecho por SQL Server con GROUP BY, sin cargar las órdenes (servicio + repositorio)
    [System.Text.Json.Serialization.JsonPropertyName("maintenanceOrderCount")]
    public int CantidadOrdenes { get; set; }
}
