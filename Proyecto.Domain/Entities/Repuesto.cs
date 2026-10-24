namespace Proyecto.Domain.Entities;

public class Repuesto
{
    public int Identificador { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string UnidadMedida { get; set; } = "Unit";
    public int StockActual { get; set; }
    public int StockMinimo { get; set; }
    public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
    public ICollection<RepuestoProveedor> ProveedoresRepuesto { get; set; } = new List<RepuestoProveedor>();
}
