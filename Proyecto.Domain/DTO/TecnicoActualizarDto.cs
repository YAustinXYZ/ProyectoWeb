using System.ComponentModel.DataAnnotations;

namespace Proyecto.Domain.DTO;

public class TecnicoActualizarDto
{
    [Required(ErrorMessage = "El nombre completo es obligatorio.")]
    [StringLength(150, MinimumLength = 3, ErrorMessage = "El nombre completo debe tener entre 3 y 150 caracteres.")]
    [System.Text.Json.Serialization.JsonPropertyName("fullName")]
    public string? NombreCompleto { get; set; }

    [Required(ErrorMessage = "La identificación es obligatoria.")]
    [StringLength(30, MinimumLength = 5, ErrorMessage = "La identificación debe tener entre 5 y 30 caracteres.")]
    [System.Text.Json.Serialization.JsonPropertyName("identificationNumber")]
    public string? NumeroIdentificacion { get; set; }

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    [StringLength(254, ErrorMessage = "El correo no puede superar 254 caracteres.")]
    [System.Text.Json.Serialization.JsonPropertyName("email")]
    public string? Correo { get; set; }

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [RegularExpression(@"^[0-9+\-\s]{8,30}$", ErrorMessage = "El teléfono debe tener entre 8 y 30 dígitos, espacios, guiones o +.")]
    [System.Text.Json.Serialization.JsonPropertyName("phone")]
    public string? Telefono { get; set; }

    [Range(0, 1000000, ErrorMessage = "La tarifa por hora debe estar entre 0 y 1 000 000.")]
    [System.Text.Json.Serialization.JsonPropertyName("hourlyRate")]
    public decimal TarifaPorHora { get; set; }

    [System.Text.Json.Serialization.JsonPropertyName("isAvailable")]
    public bool EstaDisponible { get; set; } = true;
}
