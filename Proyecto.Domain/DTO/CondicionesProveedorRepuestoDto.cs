using System.ComponentModel.DataAnnotations;

namespace Proyecto.Domain.DTO;

/// <summary>Precio y tiempo de entrega con que un proveedor ofrece un repuesto.</summary>
public class CondicionesProveedorRepuestoDto
{
    [Range(typeof(decimal), "0", "100000000", ErrorMessage = "El precio ofrecido debe estar entre 0 y 100 000 000.")]
    [System.Text.Json.Serialization.JsonPropertyName("offeredPrice")]
    public decimal PrecioOfrecido { get; set; }

    [Range(0, 365, ErrorMessage = "El tiempo de entrega debe estar entre 0 y 365 días.")]
    [System.Text.Json.Serialization.JsonPropertyName("estimatedLeadTimeDays")]
    public int DiasEntregaEstimados { get; set; }
}
