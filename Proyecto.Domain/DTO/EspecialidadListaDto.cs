namespace Proyecto.Domain.DTO;

public class EspecialidadListaDto
{
    [System.Text.Json.Serialization.JsonPropertyName("id")]
    public int Identificador { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("name")]
    public string? Nombre { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("description")]
    public string? Descripcion { get; set; }
    // Calculado: conteo hecho por SQL Server con GROUP BY
    [System.Text.Json.Serialization.JsonPropertyName("technicianCount")]
    public int CantidadTecnicos { get; set; }
}
