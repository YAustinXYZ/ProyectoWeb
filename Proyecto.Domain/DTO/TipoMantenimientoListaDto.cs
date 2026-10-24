namespace Proyecto.Domain.DTO;

public class TipoMantenimientoListaDto
{
    [System.Text.Json.Serialization.JsonPropertyName("id")]
    public int Identificador { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("name")]
    public string? Nombre { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("description")]
    public string? Descripcion { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("defaultIntervalDays")]
    public int? IntervaloPredeterminadoDias { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("isEmergency")]
    public bool EsEmergencia { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("requiredSpecialtyId")]
    public int? EspecialidadRequeridaId { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("requiredSpecialtyName")]
    public string? NombreEspecialidadRequerida { get; set; }
}
