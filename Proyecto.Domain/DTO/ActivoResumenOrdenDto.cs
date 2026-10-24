namespace Proyecto.Domain.DTO;

/// <summary>Activo relacionado dentro del detalle de una orden: datos suficientes para ubicarlo sin otra consulta.</summary>
public class ActivoResumenOrdenDto
{
    [System.Text.Json.Serialization.JsonPropertyName("id")]
    public int Identificador { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("assetCode")]
    public string? CodigoActivo { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("name")]
    public string? Nombre { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("status")]
    public string? Estado { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("categoryId")]
    public int CategoriaActivoId { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("categoryName")]
    public string? NombreCategoria { get; set; }
}
