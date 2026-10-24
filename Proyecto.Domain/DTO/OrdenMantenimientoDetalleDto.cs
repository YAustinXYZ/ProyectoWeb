namespace Proyecto.Domain.DTO;

public class OrdenMantenimientoDetalleDto
{
    [System.Text.Json.Serialization.JsonPropertyName("id")]
    public int Identificador { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("workOrderCode")]
    public string? CodigoOrdenTrabajo { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("assetId")]
    public int ActivoId { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("assetCode")]
    public string? CodigoActivo { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("assetName")]
    public string? NombreActivo { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("asset")]
    public ActivoResumenOrdenDto? Activo { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("maintenanceTypeId")]
    public int TipoMantenimientoId { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("maintenanceTypeName")]
    public string? NombreTipoMantenimiento { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("isEmergency")]
    public bool EsEmergencia { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("supplierId")]
    public int? ProveedorId { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("supplierName")]
    public string? NombreProveedor { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("title")]
    public string? Titulo { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("description")]
    public string? Descripcion { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("reason")]
    public string? Motivo { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("priority")]
    public string? Prioridad { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("status")]
    public string? Estado { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("requestedAt")]
    public DateTime SolicitadoEn { get; set; }
    // Calculado en el mapper a partir de SolicitadoEn; no existe como columna
    [System.Text.Json.Serialization.JsonPropertyName("daysSinceRequest")]
    public int DiasDesdeSolicitud { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("scheduledFor")]
    public DateTime? ProgramadoPara { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("startedAt")]
    public DateTime? IniciadoEn { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("closedAt")]
    public DateTime? CerradoEn { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("totalCost")]
    public decimal? CostoTotal { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("findings")]
    public string? Hallazgos { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("createdAt")]
    public DateTime CreadoEn { get; set; }
}
