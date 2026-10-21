using Microsoft.EntityFrameworkCore;
using Proyecto.Application.DTO;
using Proyecto.Application.Mappers;
using Proyecto.Domain.Entities;
using Proyecto.Domain.Exceptions;

namespace Proyecto.Infraestructure.Services;

public sealed partial class AssetManagementService
{
    public async Task<IReadOnlyList<AssetSummaryDto>> GetAssetsAsync(
        int? categoryId, string? status, CancellationToken cancellationToken)
    {
        var query = _context.Assets.AsNoTracking()
            .Include(asset => asset.Category)
            .Where(asset => asset.IsActive);
        if (categoryId.HasValue)
        {
            query = query.Where(asset => asset.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(asset => asset.Status == status);
        }

        var records = await query
            .OrderBy(asset => asset.AssetCode)
            .Select(asset => new { Asset = asset, OrderCount = asset.MaintenanceOrders.Count() })
            .ToListAsync(cancellationToken);

        var today = DateTime.UtcNow;
        return records.Select(record => ResponseMapper.ToAssetSummary(record.Asset, record.OrderCount, today)).ToArray();
    }

    public async Task<AssetDetailDto> GetAssetAsync(int id, CancellationToken cancellationToken)
    {
        var asset = await GetAssetEntityAsync(id, cancellationToken);
        var orderCount = await _context.MaintenanceOrders.CountAsync(
            order => order.AssetId == id, cancellationToken);
        return ResponseMapper.ToAssetDetail(asset, orderCount, DateTime.UtcNow);
    }

    public async Task<AssetDetailDto> CreateAssetAsync(AssetWriteRequest request, CancellationToken cancellationToken)
    {
        var code = request.AssetCode.Trim().ToUpperInvariant();
        if (await _context.Assets.AnyAsync(asset => asset.AssetCode == code, cancellationToken))
        {
            throw new BusinessConflictException($"Ya existe un activo con el código {code}.");
        }

        await ValidateAssetReferencesAsync(request, null, cancellationToken);
        if (request.AcquiredOn.Date < new DateTime(1900, 1, 1) ||
            request.AcquiredOn.Date > DateTime.UtcNow.Date)
        {
            throw new BusinessValidationException(
                nameof(request.AcquiredOn), "La fecha de adquisición debe estar entre 1900 y hoy.");
        }

        var asset = new Asset { AssetCode = code };
        ApplyAssetRequest(asset, request);
        asset.Status = "Operativo";
        asset.IsActive = true;
        _context.Assets.Add(asset);
        await _context.SaveChangesAsync(cancellationToken);
        return await GetAssetAsync(asset.Id, cancellationToken);
    }

    public async Task<AssetDetailDto> UpdateAssetAsync(
        int id, AssetWriteRequest request, CancellationToken cancellationToken)
    {
        var asset = await _context.Assets.FirstOrDefaultAsync(
            item => item.Id == id && item.IsActive, cancellationToken)
            ?? throw new EntityNotFoundException($"No existe el activo con id {id}.");
        var code = request.AssetCode.Trim().ToUpperInvariant();
        if (await _context.Assets.AnyAsync(item => item.Id != id && item.AssetCode == code, cancellationToken))
        {
            throw new BusinessConflictException($"Ya existe otro activo con el código {code}.");
        }

        await ValidateAssetReferencesAsync(request, id, cancellationToken);
        if (request.AcquiredOn.Date < new DateTime(1900, 1, 1) ||
            request.AcquiredOn.Date > DateTime.UtcNow.Date)
        {
            throw new BusinessValidationException(
                nameof(request.AcquiredOn), "La fecha de adquisición debe estar entre 1900 y hoy.");
        }

        asset.AssetCode = code;
        ApplyAssetRequest(asset, request);
        await _context.SaveChangesAsync(cancellationToken);
        return await GetAssetAsync(id, cancellationToken);
    }

    public async Task<AssetDetailDto> ChangeAssetStatusAsync(
        int id, string status, CancellationToken cancellationToken)
    {
        if (status is not ("Fuera de Servicio" or "Dado de Baja"))
        {
            throw new BusinessValidationException(
                nameof(status), "El cambio administrativo solo acepta Fuera de Servicio o Dado de Baja.");
        }

        var asset = await _context.Assets.FirstOrDefaultAsync(item => item.Id == id && item.IsActive, cancellationToken)
            ?? throw new EntityNotFoundException($"No existe el activo activo con id {id}.");
        var hasOpenOrder = await _context.MaintenanceOrders
            .AnyAsync(
                order => order.AssetId == id &&
                    (order.Status == "Solicitada" || order.Status == "Planned" || order.Status == "InProgress"),
                cancellationToken);
        if (hasOpenOrder)
        {
            throw new BusinessConflictException("No se puede cambiar el estado: el activo tiene solicitudes de mantenimiento abiertas.");
        }

        asset.Status = status;
        await _context.SaveChangesAsync(cancellationToken);
        return await GetAssetAsync(id, cancellationToken);
    }

    public async Task DeactivateAssetAsync(int id, CancellationToken cancellationToken)
    {
        var asset = await _context.Assets.FirstOrDefaultAsync(item => item.Id == id && item.IsActive, cancellationToken)
            ?? throw new EntityNotFoundException($"No existe el activo activo con id {id}.");
        if (await _context.MaintenanceOrders.AnyAsync(order => order.AssetId == id, cancellationToken))
        {
            throw new BusinessConflictException("No se puede dar de baja un activo con órdenes de mantenimiento asociadas.");
        }

        asset.IsActive = false;
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<AssetDetailDto> UpdateAssetImageAsync(
        int id, string imageUrl, CancellationToken cancellationToken)
    {
        var asset = await _context.Assets.FirstOrDefaultAsync(item => item.Id == id && item.IsActive, cancellationToken)
            ?? throw new EntityNotFoundException($"No existe el activo activo con id {id}.");
        asset.ImageUrl = imageUrl.Trim();
        await _context.SaveChangesAsync(cancellationToken);
        return await GetAssetAsync(id, cancellationToken);
    }

    private async Task<Asset> GetAssetEntityAsync(int id, CancellationToken cancellationToken) =>
        await _context.Assets.AsNoTracking()
            .Include(asset => asset.Category)
            .Include(asset => asset.Location)
            .Include(asset => asset.Supplier)
            .Include(asset => asset.MaintenanceOrders)
                .ThenInclude(order => order.MaintenanceType)
            .FirstOrDefaultAsync(asset => asset.Id == id && asset.IsActive, cancellationToken)
        ?? throw new EntityNotFoundException($"No existe el activo con id {id}.");

    private async Task ValidateAssetReferencesAsync(
        AssetWriteRequest request, int? assetId, CancellationToken cancellationToken)
    {
        if (!await _context.AssetCategories.AnyAsync(category => category.Id == request.CategoryId && category.IsActive, cancellationToken))
        {
            throw new EntityNotFoundException($"No existe la categoría activa con id {request.CategoryId}.");
        }

        if (!await _context.Locations.AnyAsync(location => location.Id == request.LocationId && location.IsActive, cancellationToken))
        {
            throw new EntityNotFoundException($"No existe la ubicación activa con id {request.LocationId}.");
        }

        if (request.SupplierId.HasValue &&
            !await _context.Suppliers.AnyAsync(supplier => supplier.Id == request.SupplierId.Value && supplier.IsActive, cancellationToken))
        {
            throw new EntityNotFoundException($"No existe el proveedor activo con id {request.SupplierId}.");
        }

        if (!string.IsNullOrWhiteSpace(request.SerialNumber) &&
            await _context.Assets.AnyAsync(
                asset => asset.SerialNumber == request.SerialNumber && (!assetId.HasValue || asset.Id != assetId.Value),
                cancellationToken))
        {
            throw new BusinessConflictException($"Ya existe un activo con el número de serie {request.SerialNumber}.");
        }
    }

    private static void ApplyAssetRequest(Asset asset, AssetWriteRequest request)
    {
        asset.Name = request.Name.Trim();
        asset.Description = request.Description?.Trim();
        asset.Brand = request.Brand?.Trim();
        asset.Model = request.Model?.Trim();
        asset.SerialNumber = string.IsNullOrWhiteSpace(request.SerialNumber) ? null : request.SerialNumber.Trim();
        asset.AcquiredOn = request.AcquiredOn;
        asset.PurchaseCost = request.PurchaseCost;
        asset.UsefulLifeMonths = request.UsefulLifeMonths;
        asset.UsageHours = request.UsageHours;
        asset.Criticality = request.Criticality;
        asset.CategoryId = request.CategoryId;
        asset.LocationId = request.LocationId;
        asset.SupplierId = request.SupplierId;
    }
}
