using System.ComponentModel.DataAnnotations;

namespace Proyecto.Application.DTO;

public sealed class AssetWriteRequest
{
    [Required, RegularExpression(@"^ACT-\d{4,}$", ErrorMessage = "Use el formato ACT-0001.")]
    [StringLength(30)]
    public string AssetCode { get; set; } = string.Empty;

    [Required, StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(80)]
    public string? Brand { get; set; }

    [StringLength(80)]
    public string? Model { get; set; }

    [StringLength(100)]
    public string? SerialNumber { get; set; }

    public DateTime AcquiredOn { get; set; }

    [Range(typeof(decimal), "0", "9999999999999999")]
    public decimal PurchaseCost { get; set; }

    [Range(1, 1200)]
    public int UsefulLifeMonths { get; set; }

    [Range(typeof(decimal), "0", "999999999")]
    public decimal UsageHours { get; set; }

    [Required, RegularExpression("^(Baja|Media|Alta|Crítica)$")]
    public string Criticality { get; set; } = "Media";

    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }

    [Range(1, int.MaxValue)]
    public int LocationId { get; set; }

    [Range(1, int.MaxValue)]
    public int? SupplierId { get; set; }
}

public sealed class AssetStatusRequest
{
    [Required, RegularExpression("^(Fuera de Servicio|Dado de Baja)$")]
    public string Status { get; set; } = string.Empty;
}

public sealed class AssetImageRequest
{
    [Required, Url, StringLength(1000)]
    public string ImageUrl { get; set; } = string.Empty;
}

public sealed class CategoryWriteRequest
{
    [Required, StringLength(80)]
    public string Name { get; set; } = string.Empty;

    [StringLength(250)]
    public string? Description { get; set; }

    [Range(1, 3650)]
    public int? PreventiveMaintenanceIntervalDays { get; set; }
}

public sealed class TechnicianWriteRequest
{
    [Required, StringLength(120)]
    public string FullName { get; set; } = string.Empty;

    [Required, StringLength(30)]
    public string Identification { get; set; } = string.Empty;

    [EmailAddress, StringLength(254)]
    public string? Email { get; set; }

    [Phone, StringLength(30)]
    public string? Phone { get; set; }

    public bool IsAvailable { get; set; } = true;

    [Range(typeof(decimal), "0", "9999999999999999")]
    public decimal HourlyRate { get; set; }
}

public sealed class SpecialtyWriteRequest
{
    [Required, StringLength(80)]
    public string Name { get; set; } = string.Empty;

    [StringLength(250)]
    public string? Description { get; set; }
}

public sealed class TechnicianSpecialtyRequest
{
    [Required, StringLength(40)]
    public string CertificationLevel { get; set; } = string.Empty;

    public DateTime CertifiedSince { get; set; }
}

public sealed class SparePartWriteRequest
{
    [Required, RegularExpression(@"^REP-\d{4,}$", ErrorMessage = "Use el formato REP-0001.")]
    [StringLength(30)]
    public string Code { get; set; } = string.Empty;

    [Required, StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(30)]
    public string UnitOfMeasure { get; set; } = string.Empty;

    [Range(typeof(decimal), "0", "999999999")]
    public decimal CurrentStock { get; set; }

    [Range(typeof(decimal), "0", "999999999")]
    public decimal MinimumStock { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }
}

public sealed class SupplierWriteRequest
{
    [Required, StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(30)]
    public string TaxId { get; set; } = string.Empty;

    [Required, StringLength(120)]
    public string ContactName { get; set; } = string.Empty;

    [Phone, StringLength(30)]
    public string? Phone { get; set; }

    [EmailAddress, StringLength(254)]
    public string? Email { get; set; }

    [Required, StringLength(250)]
    public string Address { get; set; } = string.Empty;

    [Required, StringLength(1000)]
    public string GeneralConditions { get; set; } = string.Empty;
}

public sealed class PartSupplierRequest
{
    [Range(typeof(decimal), "0.01", "9999999999999999")]
    public decimal OfferedPrice { get; set; }

    [Range(1, 365)]
    public int EstimatedDeliveryDays { get; set; }
}

public sealed class MaintenanceOrderRequest
{
    [Required, RegularExpression(@"^OT-\d{4}-\d{4,}$", ErrorMessage = "Use el formato OT-2026-0001.")]
    [StringLength(30)]
    public string WorkOrderCode { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int AssetId { get; set; }

    [Range(1, int.MaxValue)]
    public int MaintenanceTypeId { get; set; }

    [Required, StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required, RegularExpression("^(Low|Medium|High|Urgent)$")]
    public string Priority { get; set; } = "Medium";

    public DateTime? ScheduledFor { get; set; }
}
