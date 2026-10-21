namespace Proyecto.Domain.Entities;

public class Asset
{
    public int Id { get; set; }
    public string AssetCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public string? SerialNumber { get; set; }
    public DateTime AcquiredOn { get; set; }
    public decimal PurchaseCost { get; set; }
    public int UsefulLifeMonths { get; set; }
    public decimal UsageHours { get; set; }
    public string Criticality { get; set; } = "Media";
    public string? ImageUrl { get; set; }
    public string Status { get; set; } = "Operativo";
    public bool IsActive { get; set; } = true;
    public int CategoryId { get; set; }
    public AssetCategory Category { get; set; } = null!;
    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;
    public int? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<MaintenanceOrder> MaintenanceOrders { get; set; } = new List<MaintenanceOrder>();
}