using System.ComponentModel.DataAnnotations;
using Proyecto.Domain.Validaciones;

namespace Proyecto.Domain.DTO;

public class SolicitudEspecialidadTecnicoDto
{
    [Required(ErrorMessage = "El nivel de certificación es obligatorio.")]
    [RegularExpression("^(Basic|Intermediate|Advanced)$", ErrorMessage = "El nivel de certificación debe ser Basic, Intermediate o Advanced.")]
    [System.Text.Json.Serialization.JsonPropertyName("certificationLevel")]
    public string? NivelCertificacion { get; set; }

    [Required(ErrorMessage = "La fecha de certificación es obligatoria.")]
    [FechaNoFutura(ErrorMessage = "La fecha de certificación no puede estar en el futuro.")]
    [System.Text.Json.Serialization.JsonPropertyName("certifiedSince")]
    public DateOnly? CertificadoDesde { get; set; }
}
