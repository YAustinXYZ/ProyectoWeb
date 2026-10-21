using Microsoft.EntityFrameworkCore;
using Proyecto.Application.DTO;
using Proyecto.Application.Mappers;
using Proyecto.Domain.Entities;
using Proyecto.Domain.Exceptions;

namespace Proyecto.Infraestructure.Services;

public sealed partial class AssetManagementService
{
    public async Task<IReadOnlyList<SparePartSummaryDto>> GetSparePartsAsync(
        bool belowMinimum, CancellationToken cancellationToken)
    {
        var query = _context.SpareParts.AsNoTracking().AsQueryable();
        if (belowMinimum)
        {
            query = query.Where(part => part.CurrentStock < part.MinimumStock);
        }

        var records = await query.OrderBy(part => part.Code)
            .Select(part => new { Part = part, SupplierCount = part.Suppliers.Count })
            .ToListAsync(cancellationToken);
        return records.Select(record => ResponseMapper.ToSparePartSummary(record.Part, record.SupplierCount)).ToArray();
    }

    public async Task<SparePartDetailDto> GetSparePartAsync(int id, CancellationToken cancellationToken)
    {
        var part = await GetSparePartEntityAsync(id, cancellationToken);
        var supplierCount = await _context.PartSuppliers.CountAsync(
            relation => relation.SparePartId == id, cancellationToken);
        return ResponseMapper.ToSparePartDetail(part, supplierCount);
    }

    public async Task<SparePartDetailDto> CreateSparePartAsync(
        SparePartWriteRequest request, CancellationToken cancellationToken)
    {
        var code = request.Code.Trim().ToUpperInvariant();
        if (await _context.SpareParts.AnyAsync(part => part.Code == code, cancellationToken))
        {
            throw new BusinessConflictException($"Ya existe un repuesto con el código {code}.");
        }

        var part = new SparePart();
        ApplySparePartRequest(part, request);
        _context.SpareParts.Add(part);
        await _context.SaveChangesAsync(cancellationToken);
        return await GetSparePartAsync(part.Id, cancellationToken);
    }

    public async Task<SparePartDetailDto> UpdateSparePartAsync(
        int id, SparePartWriteRequest request, CancellationToken cancellationToken)
    {
        var part = await _context.SpareParts.FirstOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new EntityNotFoundException($"No existe el repuesto con id {id}.");
        var code = request.Code.Trim().ToUpperInvariant();
        if (await _context.SpareParts.AnyAsync(
            item => item.Id != id && item.Code == code, cancellationToken))
        {
            throw new BusinessConflictException($"Ya existe otro repuesto con el código {code}.");
        }

        ApplySparePartRequest(part, request);
        await _context.SaveChangesAsync(cancellationToken);
        return await GetSparePartAsync(id, cancellationToken);
    }

    public async Task<IReadOnlyList<SupplierSummaryDto>> GetSuppliersAsync(CancellationToken cancellationToken)
    {
        var suppliers = await _context.Suppliers.AsNoTracking()
            .Where(supplier => supplier.IsActive)
            .OrderBy(supplier => supplier.Name)
            .ToListAsync(cancellationToken);
        return suppliers.Select(ResponseMapper.ToSupplierSummary).ToArray();
    }

    public async Task<SupplierDetailDto> GetSupplierAsync(int id, CancellationToken cancellationToken)
    {
        var supplier = await GetSupplierEntityAsync(id, cancellationToken);
        return ResponseMapper.ToSupplierDetail(supplier);
    }

    public async Task<SupplierDetailDto> CreateSupplierAsync(
        SupplierWriteRequest request, CancellationToken cancellationToken)
    {
        var taxId = request.TaxId.Trim();
        if (await _context.Suppliers.AnyAsync(supplier => supplier.TaxId == taxId, cancellationToken))
        {
            throw new BusinessConflictException("Ya existe un proveedor con esa identificación tributaria.");
        }

        var supplier = new Supplier();
        ApplySupplierRequest(supplier, request);
        supplier.IsActive = true;
        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync(cancellationToken);
        return await GetSupplierAsync(supplier.Id, cancellationToken);
    }

    public async Task<SupplierDetailDto> UpdateSupplierAsync(
        int id, SupplierWriteRequest request, CancellationToken cancellationToken)
    {
        var supplier = await _context.Suppliers.FirstOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new EntityNotFoundException($"No existe el proveedor con id {id}.");
        var taxId = request.TaxId.Trim();
        if (await _context.Suppliers.AnyAsync(
            item => item.Id != id && item.TaxId == taxId, cancellationToken))
        {
            throw new BusinessConflictException("Ya existe otro proveedor con esa identificación tributaria.");
        }

        ApplySupplierRequest(supplier, request);
        await _context.SaveChangesAsync(cancellationToken);
        return await GetSupplierAsync(id, cancellationToken);
    }

    public async Task<SparePartDetailDto> AddSupplierToPartAsync(
        int partId, int supplierId, PartSupplierRequest request, CancellationToken cancellationToken)
    {
        await EnsurePartAndSupplierExistAsync(partId, supplierId, cancellationToken);
        if (await _context.PartSuppliers.AnyAsync(
            relation => relation.SparePartId == partId && relation.SupplierId == supplierId,
            cancellationToken))
        {
            throw new BusinessConflictException("El proveedor ya está asociado a este repuesto.");
        }

        _context.PartSuppliers.Add(new PartSupplier
        {
            SparePartId = partId,
            SupplierId = supplierId,
            OfferedPrice = request.OfferedPrice,
            EstimatedDeliveryDays = request.EstimatedDeliveryDays
        });
        await _context.SaveChangesAsync(cancellationToken);
        return await GetSparePartAsync(partId, cancellationToken);
    }

    public async Task<SparePartDetailDto> UpdateSupplierConditionsAsync(
        int partId, int supplierId, PartSupplierRequest request, CancellationToken cancellationToken)
    {
        var relation = await _context.PartSuppliers.FirstOrDefaultAsync(
            item => item.SparePartId == partId && item.SupplierId == supplierId, cancellationToken)
            ?? throw new EntityNotFoundException("No existe esa asociación entre el repuesto y el proveedor.");
        relation.OfferedPrice = request.OfferedPrice;
        relation.EstimatedDeliveryDays = request.EstimatedDeliveryDays;
        await _context.SaveChangesAsync(cancellationToken);
        return await GetSparePartAsync(partId, cancellationToken);
    }

    private async Task<SparePart> GetSparePartEntityAsync(int id, CancellationToken cancellationToken) =>
        await _context.SpareParts.AsNoTracking()
            .Include(part => part.Suppliers)
                .ThenInclude(relation => relation.Supplier)
            .FirstOrDefaultAsync(part => part.Id == id, cancellationToken)
        ?? throw new EntityNotFoundException($"No existe el repuesto con id {id}.");

    private async Task<Supplier> GetSupplierEntityAsync(int id, CancellationToken cancellationToken) =>
        await _context.Suppliers.AsNoTracking()
            .Include(supplier => supplier.PartSuppliers)
                .ThenInclude(relation => relation.SparePart)
            .FirstOrDefaultAsync(supplier => supplier.Id == id && supplier.IsActive, cancellationToken)
        ?? throw new EntityNotFoundException($"No existe el proveedor activo con id {id}.");

    private async Task EnsurePartAndSupplierExistAsync(
        int partId, int supplierId, CancellationToken cancellationToken)
    {
        if (!await _context.SpareParts.AnyAsync(part => part.Id == partId, cancellationToken))
        {
            throw new EntityNotFoundException($"No existe el repuesto con id {partId}.");
        }

        if (!await _context.Suppliers.AnyAsync(
            supplier => supplier.Id == supplierId && supplier.IsActive, cancellationToken))
        {
            throw new EntityNotFoundException($"No existe el proveedor activo con id {supplierId}.");
        }
    }

    private static void ApplySparePartRequest(SparePart part, SparePartWriteRequest request)
    {
        part.Code = request.Code.Trim().ToUpperInvariant();
        part.Name = request.Name.Trim();
        part.UnitOfMeasure = request.UnitOfMeasure.Trim();
        part.CurrentStock = request.CurrentStock;
        part.MinimumStock = request.MinimumStock;
        part.Description = request.Description?.Trim();
    }

    private static void ApplySupplierRequest(Supplier supplier, SupplierWriteRequest request)
    {
        supplier.Name = request.Name.Trim();
        supplier.TaxId = request.TaxId.Trim();
        supplier.ContactName = request.ContactName.Trim();
        supplier.Phone = request.Phone?.Trim();
        supplier.Email = request.Email?.Trim();
        supplier.Address = request.Address.Trim();
        supplier.GeneralConditions = request.GeneralConditions.Trim();
    }
}
