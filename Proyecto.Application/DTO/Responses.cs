namespace Proyecto.Application.DTO;

public sealed record AssetSummaryDto(
    int Id,
    string AssetCode,
    string Name,
    string Status,
    string CategoryName,
    string? ImageUrl,
    int AgeInMonths,
    int MaintenanceOrderCount);

public sealed record AssetDetailDto(
    int Id,
    string AssetCode,
    string Name,
    string? Description,
    string? Brand,
    string? Model,
    string? SerialNumber,
    DateTime AcquiredOn,
    decimal PurchaseCost,
    int UsefulLifeMonths,
    decimal UsageHours,
    string Criticality,
    string Status,
    bool IsActive,
    string? ImageUrl,
    int AgeInMonths,
    int MaintenanceOrderCount,
    int CategoryId,
    string CategoryName,
    int? PreventiveMaintenanceIntervalDays,
    int LocationId,
    string LocationName,
    int? SupplierId,
    string? SupplierName,
    IReadOnlyList<MaintenanceOrderSummaryDto> MaintenanceOrders);

public sealed record CategorySummaryDto(
    int Id,
    string Name,
    string? Description,
    int? PreventiveMaintenanceIntervalDays,
    int AssetCount);

public sealed record CategoryAssetDto(int Id, string AssetCode, string Name, string Status);

public sealed record CategoryDetailDto(
    int Id,
    string Name,
    string? Description,
    int? PreventiveMaintenanceIntervalDays,
    int AssetCount,
    IReadOnlyList<CategoryAssetDto> Assets);

public sealed record TechnicianSummaryDto(
    int Id,
    string FullName,
    string Identification,
    bool IsAvailable,
    decimal HourlyRate,
    int SpecialtyCount);

public sealed record TechnicianSpecialtyDto(
    int SpecialtyId,
    string Name,
    string CertificationLevel,
    DateTime CertifiedSince,
    int SeniorityInMonths);

public sealed record TechnicianDetailDto(
    int Id,
    string FullName,
    string Identification,
    string? Email,
    string? Phone,
    bool IsAvailable,
    decimal HourlyRate,
    int SpecialtyCount,
    IReadOnlyList<TechnicianSpecialtyDto> Specialties);

public sealed record SpecialtySummaryDto(int Id, string Name, string? Description, int TechnicianCount);

public sealed record SpecialtyTechnicianDto(
    int TechnicianId,
    string FullName,
    string CertificationLevel,
    bool IsAvailable);

public sealed record SpecialtyDetailDto(
    int Id,
    string Name,
    string? Description,
    int TechnicianCount,
    IReadOnlyList<SpecialtyTechnicianDto> Technicians);

public sealed record SparePartSummaryDto(
    int Id,
    string Code,
    string Name,
    string UnitOfMeasure,
    decimal CurrentStock,
    decimal MinimumStock,
    bool IsBelowMinimum,
    decimal StockShortage,
    int SupplierCount);

public sealed record SparePartSupplierDto(
    int SupplierId,
    string SupplierName,
    decimal OfferedPrice,
    int EstimatedDeliveryDays);

public sealed record SparePartDetailDto(
    int Id,
    string Code,
    string Name,
    string UnitOfMeasure,
    decimal CurrentStock,
    decimal MinimumStock,
    string? Description,
    bool IsBelowMinimum,
    decimal StockShortage,
    int SupplierCount,
    IReadOnlyList<SparePartSupplierDto> Suppliers);

public sealed record SupplierSummaryDto(
    int Id,
    string Name,
    string TaxId,
    string ContactName,
    string? Phone,
    string? Email,
    string Address,
    string GeneralConditions);

public sealed record SupplierSparePartDto(
    int SparePartId,
    string Code,
    string Name,
    decimal OfferedPrice,
    int EstimatedDeliveryDays);

public sealed record SupplierDetailDto(
    int Id,
    string Name,
    string TaxId,
    string ContactName,
    string? Phone,
    string? Email,
    string Address,
    string GeneralConditions,
    IReadOnlyList<SupplierSparePartDto> SpareParts);

public sealed record MaintenanceOrderSummaryDto(
    int Id,
    string WorkOrderCode,
    int AssetId,
    string AssetCode,
    string AssetName,
    string MaintenanceTypeName,
    DateTime RequestedAt,
    DateTime? ScheduledFor,
    string Priority,
    string Status,
    int DaysSinceRequested);

public sealed record MaintenanceOrderDetailDto(
    int Id,
    string WorkOrderCode,
    int AssetId,
    string AssetCode,
    string AssetName,
    string AssetStatus,
    string AssetCategoryName,
    int MaintenanceTypeId,
    string MaintenanceTypeName,
    string Title,
    string? Description,
    DateTime RequestedAt,
    DateTime? ScheduledFor,
    string Priority,
    string Status,
    int DaysSinceRequested);
