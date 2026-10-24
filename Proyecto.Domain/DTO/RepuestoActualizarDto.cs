using System.ComponentModel.DataAnnotations;

namespace Proyecto.Domain.DTO;

public class RepuestoActualizarDto
{
    [Required(ErrorMessage = "El código del repuesto es obligatorio.")]
    [RegularExpression("^[A-Z0-9-]{3,30}$", ErrorMessage = "El código debe tener entre 3 y 30 caracteres: letras mayúsculas, números o guiones.")]
    [System.Text.Json.Serialization.JsonPropertyName("code")]
    public string? Codigo { get; set; }

    [Required(ErrorMessage = "El nombre del repuesto es obligatorio.")]
    [StringLength(120, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 120 caracteres.")]
    [System.Text.Json.Serialization.JsonPropertyName("name")]
    public string? Nombre { get; set; }

    [StringLength(500, ErrorMessage = "La descripción no puede superar 500 caracteres.")]
    [System.Text.Json.Serialization.JsonPropertyName("description")]
    public string? Descripcion { get; set; }

    [Required(ErrorMessage = "La unidad de medida es obligatoria.")]
    [RegularExpression("^(Unit|Box|Pack|Meter|Roll|Kit)$", ErrorMessage = "La unidad de medida debe ser Unit, Box, Pack, Meter, Roll o Kit.")]
    [System.Text.Json.Serialization.JsonPropertyName("unitOfMeasure")]
    public string? UnidadMedida { get; set; }

    // El stock nunca puede ser negativo (también lo garantiza CK_SpareParts_Stock)
    [Range(0, int.MaxValue, ErrorMessage = "El stock actual no puede ser negativo.")]
    [System.Text.Json.Serialization.JsonPropertyName("currentStock")]
    public int StockActual { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo no puede ser negativo.")]
    [System.Text.Json.Serialization.JsonPropertyName("minimumStock")]
    public int StockMinimo { get; set; }
}
