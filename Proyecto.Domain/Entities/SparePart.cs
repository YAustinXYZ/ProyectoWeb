namespace Proyecto.Domain.Entities;

public class SparePart
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string UnitOfMeasure { get; set; } = string.Empty;
    public decimal CurrentStock { get; set; }
    public decimal MinimumStock { get; set; }
    public string? Description { get; set; }
    public ICollection<PartSupplier> Suppliers { get; set; } = new List<PartSupplier>();
}
