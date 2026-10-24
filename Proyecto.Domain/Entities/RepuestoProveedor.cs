namespace Proyecto.Domain.Entities;

// Tabla puente N:N entre Repuesto y Proveedor con atributos propios de la relación
public class RepuestoProveedor
{
    public int RepuestoId { get; set; }
    public Repuesto Repuesto { get; set; } = null!;
    public int ProveedorId { get; set; }
    public Proveedor Proveedor { get; set; } = null!;
    public decimal PrecioOfrecido { get; set; }
    public int DiasEntregaEstimados { get; set; }
}
