using System.ComponentModel.DataAnnotations;
using Proyecto.Domain.Validaciones;

namespace Proyecto.Domain.DTO;

public class ActivoActualizarDto
{
    [Required(ErrorMessage = "El nombre del activo es obligatorio.")]
    [StringLength(120, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 120 caracteres.")]
    [System.Text.Json.Serialization.JsonPropertyName("name")]
    public string? Nombre { get; set; }

    [StringLength(500, ErrorMessage = "La descripción no puede superar 500 caracteres.")]
    [System.Text.Json.Serialization.JsonPropertyName("description")]
    public string? Descripcion { get; set; }

    [StringLength(80, ErrorMessage = "La marca no puede superar 80 caracteres.")]
    [System.Text.Json.Serialization.JsonPropertyName("brand")]
    public string? Marca { get; set; }

    [StringLength(80, ErrorMessage = "El modelo no puede superar 80 caracteres.")]
    [System.Text.Json.Serialization.JsonPropertyName("model")]
    public string? Modelo { get; set; }

    [StringLength(100, ErrorMessage = "El número de serie no puede superar 100 caracteres.")]
    [System.Text.Json.Serialization.JsonPropertyName("serialNumber")]
    public string? NumeroSerie { get; set; }

    [Required(ErrorMessage = "La fecha de adquisición es obligatoria.")]
    [System.Text.Json.Serialization.JsonPropertyName("acquiredOn")]
    [FechaNoFutura(ErrorMessage = "La fecha de adquisición no puede estar en el futuro.")]
    public DateTime? FechaAdquisicion { get; set; }

    [Range(0, 999999999.99, ErrorMessage = "El costo de compra debe estar entre 0 y 999999999.99.")]
    [System.Text.Json.Serialization.JsonPropertyName("purchaseCost")]
    public decimal CostoCompra { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La vida útil debe ser de al menos 1 mes.")]
    [System.Text.Json.Serialization.JsonPropertyName("usefulLifeMonths")]
    public int MesesVidaUtil { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Las horas de uso no pueden ser negativas.")]
    [System.Text.Json.Serialization.JsonPropertyName("usageHours")]
    public int HorasUso { get; set; }

    [Required(ErrorMessage = "La criticidad es obligatoria.")]
    [RegularExpression("^(High|Medium|Low)$", ErrorMessage = "La criticidad debe ser High, Medium o Low.")]
    [System.Text.Json.Serialization.JsonPropertyName("criticality")]
    public string? Criticidad { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La categoría es obligatoria.")]
    [System.Text.Json.Serialization.JsonPropertyName("categoryId")]
    public int CategoriaActivoId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La ubicación es obligatoria.")]
    [System.Text.Json.Serialization.JsonPropertyName("locationId")]
    public int UbicacionId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "El proveedor no es válido.")]
    [System.Text.Json.Serialization.JsonPropertyName("supplierId")]
    public int? ProveedorId { get; set; }
}
