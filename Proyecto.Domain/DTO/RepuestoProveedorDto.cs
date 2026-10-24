namespace Proyecto.Domain.DTO;

/// <summary>Proveedor dentro del detalle de un repuesto, con sus condiciones de la relación N:N.</summary>
public class RepuestoProveedorDto
{
    [System.Text.Json.Serialization.JsonPropertyName("id")]
    public int Identificador { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("name")]
    public string? Nombre { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("offeredPrice")]
    public decimal PrecioOfrecido { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("estimatedLeadTimeDays")]
    public int DiasEntregaEstimados { get; set; }
}
