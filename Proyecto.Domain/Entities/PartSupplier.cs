namespace Proyecto.Domain.Entities;

public class PartSupplier
{
    public int SparePartId { get; set; }
    public SparePart SparePart { get; set; } = null!;
    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;
    public decimal OfferedPrice { get; set; }
    public int EstimatedDeliveryDays { get; set; }
}
