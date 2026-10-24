namespace Proyecto.Domain.DTO;

public class OrdenMantenimientoListaDto
{
    [System.Text.Json.Serialization.JsonPropertyName("id")]
    public int Identificador { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("workOrderCode")]
    public string? CodigoOrdenTrabajo { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("assetCode")]
    public string? CodigoActivo { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("assetName")]
    public string? NombreActivo { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("maintenanceTypeName")]
    public string? NombreTipoMantenimiento { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("supplierName")]
    public string? NombreProveedor { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("title")]
    public string? Titulo { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("priority")]
    public string? Prioridad { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("status")]
    public string? Estado { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("requestedAt")]
    public DateTime SolicitadoEn { get; set; }
    // Calculado en el mapper a partir de SolicitadoEn; no existe como columna
    [System.Text.Json.Serialization.JsonPropertyName("daysSinceRequest")]
    public int DiasDesdeSolicitud { get; set; }
}
