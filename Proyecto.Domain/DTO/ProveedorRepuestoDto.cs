namespace Proyecto.Domain.DTO;

/// <summary>Repuesto que suministra un proveedor, dentro del detalle del proveedor.</summary>
public class ProveedorRepuestoDto
{
    [System.Text.Json.Serialization.JsonPropertyName("id")]
    public int Identificador { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("code")]
    public string? Codigo { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("name")]
    public string? Nombre { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("offeredPrice")]
    public decimal PrecioOfrecido { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("estimatedLeadTimeDays")]
    public int DiasEntregaEstimados { get; set; }
}
