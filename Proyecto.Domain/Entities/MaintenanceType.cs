namespace Proyecto.Domain.Entities;

public class MaintenanceType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? DefaultIntervalDays { get; set; }
    public ICollection<MaintenanceOrder> MaintenanceOrders { get; set; } = new List<MaintenanceOrder>();
}