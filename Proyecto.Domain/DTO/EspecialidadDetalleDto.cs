namespace Proyecto.Domain.DTO;

public class EspecialidadDetalleDto
{
    [System.Text.Json.Serialization.JsonPropertyName("id")]
    public int Identificador { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("name")]
    public string? Nombre { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("description")]
    public string? Descripcion { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("technicianCount")]
    public int CantidadTecnicos { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("technicians")]
    public List<EspecialidadTecnicoDto> Tecnicos { get; set; } = [];
}
