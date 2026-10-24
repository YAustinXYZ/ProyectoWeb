namespace Proyecto.Domain.Entities;

public class Proveedor
{
    public int Identificador { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string IdentificacionFiscal { get; set; } = string.Empty;
    public string? NombreContacto { get; set; }
    public string? Telefono { get; set; }
    public string? Correo { get; set; }
    public string? Direccion { get; set; }
    public string? CondicionesGenerales { get; set; }
    public bool EstaActivo { get; set; } = true;
    public ICollection<Activo> Activos { get; set; } = new List<Activo>();
    public ICollection<OrdenMantenimiento> OrdenesMantenimiento { get; set; } = new List<OrdenMantenimiento>();
    public ICollection<RepuestoProveedor> RepuestosProveedor { get; set; } = new List<RepuestoProveedor>();
}