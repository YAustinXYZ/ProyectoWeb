namespace Proyecto.Domain.Entities;

public class MaintenanceOrder
{
    public int Id { get; set; }
    public string WorkOrderCode { get; set; } = string.Empty;
    public int AssetId { get; set; }
    public Asset Asset { get; set; } = null!;
    public int MaintenanceTypeId { get; set; }
    public MaintenanceType MaintenanceType { get; set; } = null!;
    public int? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Priority { get; set; } = "Medium";
    public string Status { get; set; } = "Solicitada";
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ScheduledFor { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public decimal? Cost { get; set; }
    public string? Findings { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}