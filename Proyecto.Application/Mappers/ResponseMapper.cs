using Proyecto.Application.DTO;
using Proyecto.Domain.Entities;

namespace Proyecto.Application.Mappers;

public static class ResponseMapper
{
    public static AssetSummaryDto ToAssetSummary(Asset asset, int orderCount, DateTime today) =>
        new(asset.Id, asset.AssetCode, asset.Name, asset.Status, asset.Category.Name, asset.ImageUrl,
            GetMonths(asset.AcquiredOn, today), orderCount);

    public static AssetDetailDto ToAssetDetail(Asset asset, int orderCount, DateTime today) =>
        new(
            asset.Id,
            asset.AssetCode,
            asset.Name,
            asset.Description,
            asset.Brand,
            asset.Model,
            asset.SerialNumber,
            asset.AcquiredOn,
            asset.PurchaseCost,
            asset.UsefulLifeMonths,
            asset.UsageHours,
            asset.Criticality,
            asset.Status,
            asset.IsActive,
            asset.ImageUrl,
            GetMonths(asset.AcquiredOn, today),
            orderCount,
            asset.CategoryId,
            asset.Category.Name,
            asset.Category.PreventiveMaintenanceIntervalDays,
            asset.LocationId,
            asset.Location.Name,
            asset.SupplierId,
            asset.Supplier?.Name,
            asset.MaintenanceOrders
                .OrderByDescending(order => order.RequestedAt)
                .Select(order => new MaintenanceOrderSummaryDto(
                    order.Id,
                    order.WorkOrderCode,
                    order.AssetId,
                    asset.AssetCode,
                    asset.Name,
                    order.MaintenanceType.Name,
                    order.RequestedAt,
                    order.ScheduledFor,
                    order.Priority,
                    order.Status,
                    GetElapsedDays(order.RequestedAt, today)))
                .ToArray());

    public static CategorySummaryDto ToCategorySummary(AssetCategory category, int assetCount) =>
        new(category.Id, category.Name, category.Description, category.PreventiveMaintenanceIntervalDays, assetCount);

    public static CategoryDetailDto ToCategoryDetail(AssetCategory category, int assetCount) =>
        new(
            category.Id,
            category.Name,
            category.Description,
            category.PreventiveMaintenanceIntervalDays,
            assetCount,
            category.Assets.OrderBy(asset => asset.AssetCode)
                .Select(asset => new CategoryAssetDto(asset.Id, asset.AssetCode, asset.Name, asset.Status))
                .ToArray());

    public static TechnicianSummaryDto ToTechnicianSummary(Technician technician, int specialtyCount) =>
        new(technician.Id, technician.FullName, technician.Identification, technician.IsAvailable,
            technician.HourlyRate, specialtyCount);

    public static TechnicianDetailDto ToTechnicianDetail(Technician technician, int specialtyCount, DateTime today) =>
        new(
            technician.Id,
            technician.FullName,
            technician.Identification,
            technician.Email,
            technician.Phone,
            technician.IsAvailable,
            technician.HourlyRate,
            specialtyCount,
            technician.Specialties
                .OrderBy(relation => relation.Specialty.Name)
                .Select(relation => new TechnicianSpecialtyDto(
                    relation.SpecialtyId,
                    relation.Specialty.Name,
                    relation.CertificationLevel,
                    relation.CertifiedSince,
                    GetMonths(relation.CertifiedSince, today)))
                .ToArray());

    public static SpecialtySummaryDto ToSpecialtySummary(Specialty specialty, int technicianCount) =>
        new(specialty.Id, specialty.Name, specialty.Description, technicianCount);

    public static SpecialtyDetailDto ToSpecialtyDetail(Specialty specialty, int technicianCount) =>
        new(
            specialty.Id,
            specialty.Name,
            specialty.Description,
            technicianCount,
            specialty.Technicians
                .OrderBy(relation => relation.Technician.FullName)
                .Select(relation => new SpecialtyTechnicianDto(
                    relation.TechnicianId,
                    relation.Technician.FullName,
                    relation.CertificationLevel,
                    relation.Technician.IsAvailable))
                .ToArray());

    public static SparePartSummaryDto ToSparePartSummary(SparePart part, int supplierCount) =>
        new(
            part.Id,
            part.Code,
            part.Name,
            part.UnitOfMeasure,
            part.CurrentStock,
            part.MinimumStock,
            part.CurrentStock < part.MinimumStock,
            Math.Max(0, part.MinimumStock - part.CurrentStock),
            supplierCount);

    public static SparePartDetailDto ToSparePartDetail(SparePart part, int supplierCount) =>
        new(
            part.Id,
            part.Code,
            part.Name,
            part.UnitOfMeasure,
            part.CurrentStock,
            part.MinimumStock,
            part.Description,
            part.CurrentStock < part.MinimumStock,
            Math.Max(0, part.MinimumStock - part.CurrentStock),
            supplierCount,
            part.Suppliers.OrderBy(relation => relation.Supplier.Name)
                .Select(relation => new SparePartSupplierDto(
                    relation.SupplierId,
                    relation.Supplier.Name,
                    relation.OfferedPrice,
                    relation.EstimatedDeliveryDays))
                .ToArray());

    public static SupplierSummaryDto ToSupplierSummary(Supplier supplier) =>
        new(supplier.Id, supplier.Name, supplier.TaxId, supplier.ContactName ?? string.Empty,
            supplier.Phone, supplier.Email, supplier.Address ?? string.Empty,
            supplier.GeneralConditions ?? string.Empty);

    public static SupplierDetailDto ToSupplierDetail(Supplier supplier) =>
        new(
            supplier.Id,
            supplier.Name,
            supplier.TaxId,
            supplier.ContactName ?? string.Empty,
            supplier.Phone,
            supplier.Email,
            supplier.Address ?? string.Empty,
            supplier.GeneralConditions ?? string.Empty,
            supplier.PartSuppliers.OrderBy(relation => relation.SparePart.Code)
                .Select(relation => new SupplierSparePartDto(
                    relation.SparePartId,
                    relation.SparePart.Code,
                    relation.SparePart.Name,
                    relation.OfferedPrice,
                    relation.EstimatedDeliveryDays))
                .ToArray());

    public static MaintenanceOrderSummaryDto ToMaintenanceOrderSummary(MaintenanceOrder order, DateTime today) =>
        new(
            order.Id,
            order.WorkOrderCode,
            order.AssetId,
            order.Asset.AssetCode,
            order.Asset.Name,
            order.MaintenanceType.Name,
            order.RequestedAt,
            order.ScheduledFor,
            order.Priority,
            order.Status,
            GetElapsedDays(order.RequestedAt, today));

    public static MaintenanceOrderDetailDto ToMaintenanceOrderDetail(MaintenanceOrder order, DateTime today) =>
        new(
            order.Id,
            order.WorkOrderCode,
            order.AssetId,
            order.Asset.AssetCode,
            order.Asset.Name,
            order.Asset.Status,
            order.Asset.Category.Name,
            order.MaintenanceTypeId,
            order.MaintenanceType.Name,
            order.Title,
            order.Description,
            order.RequestedAt,
            order.ScheduledFor,
            order.Priority,
            order.Status,
            GetElapsedDays(order.RequestedAt, today));

    private static int GetMonths(DateTime start, DateTime end)
    {
        if (start.Date > end.Date)
        {
            return 0;
        }

        var months = (end.Year - start.Year) * 12 + end.Month - start.Month;
        return Math.Max(0, months - (end.Day < start.Day ? 1 : 0));
    }

    private static int GetElapsedDays(DateTime start, DateTime end) =>
        Math.Max(0, (end.Date - start.Date).Days);
}
