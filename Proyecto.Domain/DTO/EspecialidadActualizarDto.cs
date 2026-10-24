using System.ComponentModel.DataAnnotations;

namespace Proyecto.Domain.DTO;

public class EspecialidadActualizarDto
{
    [Required(ErrorMessage = "El nombre de la especialidad es obligatorio.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
    [System.Text.Json.Serialization.JsonPropertyName("name")]
    public string? Nombre { get; set; }

    [StringLength(250, ErrorMessage = "La descripción no puede superar 250 caracteres.")]
    [System.Text.Json.Serialization.JsonPropertyName("description")]
    public string? Descripcion { get; set; }
}
