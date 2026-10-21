using Proyecto.Application.DTO;

namespace Proyecto.Application.Interfaces;

public interface IAssetManagementService
{
    Task<IReadOnlyList<AssetSummaryDto>> GetAssetsAsync(int? categoryId, string? status, CancellationToken cancellationToken);
    Task<AssetDetailDto> GetAssetAsync(int id, CancellationToken cancellationToken);
    Task<AssetDetailDto> CreateAssetAsync(AssetWriteRequest request, CancellationToken cancellationToken);
    Task<AssetDetailDto> UpdateAssetAsync(int id, AssetWriteRequest request, CancellationToken cancellationToken);
    Task<AssetDetailDto> ChangeAssetStatusAsync(int id, string status, CancellationToken cancellationToken);
    Task DeactivateAssetAsync(int id, CancellationToken cancellationToken);
    Task<AssetDetailDto> UpdateAssetImageAsync(int id, string imageUrl, CancellationToken cancellationToken);

    Task<IReadOnlyList<CategorySummaryDto>> GetCategoriesAsync(CancellationToken cancellationToken);
    Task<CategoryDetailDto> GetCategoryAsync(int id, CancellationToken cancellationToken);
    Task<CategoryDetailDto> CreateCategoryAsync(CategoryWriteRequest request, CancellationToken cancellationToken);
    Task<CategoryDetailDto> UpdateCategoryAsync(int id, CategoryWriteRequest request, CancellationToken cancellationToken);
    Task DeleteCategoryAsync(int id, CancellationToken cancellationToken);

    Task<IReadOnlyList<TechnicianSummaryDto>> GetTechniciansAsync(int? specialtyId, bool? isAvailable, CancellationToken cancellationToken);
    Task<TechnicianDetailDto> GetTechnicianAsync(int id, CancellationToken cancellationToken);
    Task<TechnicianDetailDto> CreateTechnicianAsync(TechnicianWriteRequest request, CancellationToken cancellationToken);
    Task<TechnicianDetailDto> UpdateTechnicianAsync(int id, TechnicianWriteRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyList<SpecialtySummaryDto>> GetSpecialtiesAsync(CancellationToken cancellationToken);
    Task<SpecialtyDetailDto> GetSpecialtyAsync(int id, CancellationToken cancellationToken);
    Task<SpecialtyDetailDto> CreateSpecialtyAsync(SpecialtyWriteRequest request, CancellationToken cancellationToken);
    Task<SpecialtyDetailDto> UpdateSpecialtyAsync(int id, SpecialtyWriteRequest request, CancellationToken cancellationToken);
    Task<TechnicianDetailDto> AddSpecialtyAsync(int technicianId, int specialtyId, TechnicianSpecialtyRequest request, CancellationToken cancellationToken);
    Task RemoveSpecialtyAsync(int technicianId, int specialtyId, CancellationToken cancellationToken);

    Task<IReadOnlyList<SparePartSummaryDto>> GetSparePartsAsync(bool belowMinimum, CancellationToken cancellationToken);
    Task<SparePartDetailDto> GetSparePartAsync(int id, CancellationToken cancellationToken);
    Task<SparePartDetailDto> CreateSparePartAsync(SparePartWriteRequest request, CancellationToken cancellationToken);
    Task<SparePartDetailDto> UpdateSparePartAsync(int id, SparePartWriteRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyList<SupplierSummaryDto>> GetSuppliersAsync(CancellationToken cancellationToken);
    Task<SupplierDetailDto> GetSupplierAsync(int id, CancellationToken cancellationToken);
    Task<SupplierDetailDto> CreateSupplierAsync(SupplierWriteRequest request, CancellationToken cancellationToken);
    Task<SupplierDetailDto> UpdateSupplierAsync(int id, SupplierWriteRequest request, CancellationToken cancellationToken);
    Task<SparePartDetailDto> AddSupplierToPartAsync(int partId, int supplierId, PartSupplierRequest request, CancellationToken cancellationToken);
    Task<SparePartDetailDto> UpdateSupplierConditionsAsync(int partId, int supplierId, PartSupplierRequest request, CancellationToken cancellationToken);

    Task<IReadOnlyList<MaintenanceOrderSummaryDto>> GetMaintenanceOrdersAsync(int? assetId, CancellationToken cancellationToken);
    Task<MaintenanceOrderDetailDto> GetMaintenanceOrderAsync(int id, CancellationToken cancellationToken);
    Task<MaintenanceOrderDetailDto> CreateMaintenanceOrderAsync(MaintenanceOrderRequest request, CancellationToken cancellationToken);

    Task<IReadOnlyList<LookupDto>> GetLocationsAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<LookupDto>> GetMaintenanceTypesAsync(CancellationToken cancellationToken);
}
