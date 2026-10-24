using System.ComponentModel.DataAnnotations;

namespace Proyecto.Domain.DTO;

public class ActualizarEstadoActivoDto
{
    // UnderMaintenance no se asigna a mano: depende de las órdenes de mantenimiento.
    // Decommissioned también se alcanza con DELETE /api/Asset/{id}; ambos caminos aplican la misma regla.
    [Required(ErrorMessage = "El estado es obligatorio.")]
    [RegularExpression("^(Operational|OutOfService|Decommissioned)$", ErrorMessage = "El estado solo puede ser Operational, OutOfService o Decommissioned.")]
    [System.Text.Json.Serialization.JsonPropertyName("status")]
    public string? Estado { get; set; }
}
