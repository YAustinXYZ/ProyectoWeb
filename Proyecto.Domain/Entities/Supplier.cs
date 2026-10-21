namespace Proyecto.Domain.Entities;

public class Supplier
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty;
    public string? ContactName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? GeneralConditions { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<Asset> Assets { get; set; } = new List<Asset>();
    public ICollection<MaintenanceOrder> MaintenanceOrders { get; set; } = new List<MaintenanceOrder>();
    public ICollection<PartSupplier> PartSuppliers { get; set; } = new List<PartSupplier>();
}