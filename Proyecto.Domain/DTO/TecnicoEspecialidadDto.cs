namespace Proyecto.Domain.DTO;

/// <summary>Especialidad que posee un técnico, con los atributos de la relación N:N.</summary>
public class TecnicoEspecialidadDto
{
    [System.Text.Json.Serialization.JsonPropertyName("id")]
    public int Identificador { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("name")]
    public string? Nombre { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("certificationLevel")]
    public string? NivelCertificacion { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("certifiedSince")]
    public DateOnly CertificadoDesde { get; set; }
    // Calculado en el mapper: meses completos desde CertificadoDesde
    [System.Text.Json.Serialization.JsonPropertyName("seniorityInMonths")]
    public int MesesAntiguedad { get; set; }
}
