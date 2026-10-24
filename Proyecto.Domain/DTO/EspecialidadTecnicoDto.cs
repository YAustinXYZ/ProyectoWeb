namespace Proyecto.Domain.DTO;

/// <summary>Técnico que posee una especialidad, con su nivel y disponibilidad.</summary>
public class EspecialidadTecnicoDto
{
    [System.Text.Json.Serialization.JsonPropertyName("id")]
    public int Identificador { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("fullName")]
    public string? NombreCompleto { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("certificationLevel")]
    public string? NivelCertificacion { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("certifiedSince")]
    public DateOnly CertificadoDesde { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("isAvailable")]
    public bool EstaDisponible { get; set; }
}
