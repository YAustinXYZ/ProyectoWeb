using Microsoft.EntityFrameworkCore;
using Proyecto.Application.DTO;
using Proyecto.Application.Mappers;
using Proyecto.Domain.Entities;
using Proyecto.Domain.Exceptions;

namespace Proyecto.Infraestructure.Services;

public sealed partial class AssetManagementService
{
    public async Task<IReadOnlyList<MaintenanceOrderSummaryDto>> GetMaintenanceOrdersAsync(
        int? assetId, CancellationToken cancellationToken)
    {
        var query = _context.MaintenanceOrders.AsNoTracking()
            .Include(order => order.Asset)
            .Include(order => order.MaintenanceType)
            .AsQueryable();
        if (assetId.HasValue)
        {
            query = query.Where(order => order.AssetId == assetId.Value);
        }

        var orders = await query.OrderByDescending(order => order.RequestedAt).ToListAsync(cancellationToken);
        var today = DateTime.UtcNow;
        return orders.Select(order => ResponseMapper.ToMaintenanceOrderSummary(order, today)).ToArray();
    }

    public async Task<MaintenanceOrderDetailDto> GetMaintenanceOrderAsync(
        int id, CancellationToken cancellationToken)
    {
        var order = await GetMaintenanceOrderEntityAsync(id, cancellationToken);
        return ResponseMapper.ToMaintenanceOrderDetail(order, DateTime.UtcNow);
    }

    public async Task<MaintenanceOrderDetailDto> CreateMaintenanceOrderAsync(
        MaintenanceOrderRequest request, CancellationToken cancellationToken)
    {
        var code = request.WorkOrderCode.Trim().ToUpperInvariant();
        if (await _context.MaintenanceOrders.AnyAsync(order => order.WorkOrderCode == code, cancellationToken))
        {
            throw new BusinessConflictException($"Ya existe una orden con el código {code}.");
        }

        var asset = await _context.Assets.FirstOrDefaultAsync(
            item => item.Id == request.AssetId && item.IsActive && item.Status != "Dado de Baja",
            cancellationToken);
        if (asset is null)
        {
            throw new EntityNotFoundException($"No existe un activo disponible con id {request.AssetId}.");
        }

        if (!await _context.MaintenanceTypes.AnyAsync(
            type => type.Id == request.MaintenanceTypeId, cancellationToken))
        {
            throw new EntityNotFoundException($"No existe el tipo de mantenimiento con id {request.MaintenanceTypeId}.");
        }

        var requestedAt = DateTime.UtcNow;
        if (request.ScheduledFor.HasValue && request.ScheduledFor.Value < requestedAt.Date)
        {
            throw new BusinessValidationException(
                nameof(request.ScheduledFor),
                "La fecha programada no puede ser anterior a la fecha de solicitud.");
        }

        var order = new MaintenanceOrder
        {
            WorkOrderCode = code,
            AssetId = request.AssetId,
            MaintenanceTypeId = request.MaintenanceTypeId,
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            Priority = request.Priority,
            Status = "Solicitada",
            RequestedAt = requestedAt,
            ScheduledFor = request.ScheduledFor,
            CreatedAt = requestedAt
        };
        _context.MaintenanceOrders.Add(order);
        await _context.SaveChangesAsync(cancellationToken);
        return await GetMaintenanceOrderAsync(order.Id, cancellationToken);
    }

    public async Task<IReadOnlyList<LookupDto>> GetLocationsAsync(CancellationToken cancellationToken)
    {
        return await _context.Locations.AsNoTracking()
            .Where(location => location.IsActive)
            .OrderBy(location => location.Name)
            .Select(location => new LookupDto(location.Id, location.Name, location.Description))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<LookupDto>> GetMaintenanceTypesAsync(CancellationToken cancellationToken)
    {
        return await _context.MaintenanceTypes.AsNoTracking()
            .OrderBy(type => type.Name)
            .Select(type => new LookupDto(type.Id, type.Name, type.Description))
            .ToListAsync(cancellationToken);
    }

    private async Task<MaintenanceOrder> GetMaintenanceOrderEntityAsync(
        int id, CancellationToken cancellationToken) =>
        await _context.MaintenanceOrders.AsNoTracking()
            .Include(order => order.Asset)
                .ThenInclude(asset => asset.Category)
            .Include(order => order.MaintenanceType)
            .FirstOrDefaultAsync(order => order.Id == id, cancellationToken)
        ?? throw new EntityNotFoundException($"No existe la orden de mantenimiento con id {id}.");
}
