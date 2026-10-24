namespace Proyecto.Domain.DTO;

/// <summary>Orden dentro del detalle de un activo: solo lo necesario para ver su historial de un vistazo.</summary>
public class OrdenResumenActivoDto
{
    [System.Text.Json.Serialization.JsonPropertyName("id")]
    public int Identificador { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("workOrderCode")]
    public string? CodigoOrdenTrabajo { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("requestedAt")]
    public DateTime SolicitadoEn { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("maintenanceTypeName")]
    public string? NombreTipoMantenimiento { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("priority")]
    public string? Prioridad { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("status")]
    public string? Estado { get; set; }
}
