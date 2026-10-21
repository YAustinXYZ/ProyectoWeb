using Microsoft.EntityFrameworkCore;
using Proyecto.Application.DTO;
using Proyecto.Application.Mappers;
using Proyecto.Domain.Entities;
using Proyecto.Domain.Exceptions;

namespace Proyecto.Infraestructure.Services;

public sealed partial class AssetManagementService
{
    public async Task<IReadOnlyList<CategorySummaryDto>> GetCategoriesAsync(CancellationToken cancellationToken)
    {
        var records = await _context.AssetCategories.AsNoTracking()
            .Where(category => category.IsActive)
            .OrderBy(category => category.Name)
            .Select(category => new { Category = category, AssetCount = category.Assets.Count(asset => asset.IsActive) })
            .ToListAsync(cancellationToken);
        return records.Select(record => ResponseMapper.ToCategorySummary(record.Category, record.AssetCount)).ToArray();
    }

    public async Task<CategoryDetailDto> GetCategoryAsync(int id, CancellationToken cancellationToken)
    {
        var category = await GetCategoryEntityAsync(id, cancellationToken);
        var assetCount = await _context.Assets.CountAsync(
            asset => asset.CategoryId == id && asset.IsActive, cancellationToken);
        return ResponseMapper.ToCategoryDetail(category, assetCount);
    }

    public async Task<CategoryDetailDto> CreateCategoryAsync(
        CategoryWriteRequest request, CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();
        if (await _context.AssetCategories.AnyAsync(category => category.Name == name, cancellationToken))
        {
            throw new BusinessConflictException($"Ya existe una categoría llamada '{name}'.");
        }

        var category = new AssetCategory();
        ApplyCategoryRequest(category, request);
        _context.AssetCategories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);
        return await GetCategoryAsync(category.Id, cancellationToken);
    }

    public async Task<CategoryDetailDto> UpdateCategoryAsync(
        int id, CategoryWriteRequest request, CancellationToken cancellationToken)
    {
        var category = await _context.AssetCategories.FirstOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new EntityNotFoundException($"No existe la categoría con id {id}.");
        var name = request.Name.Trim();
        if (await _context.AssetCategories.AnyAsync(item => item.Id != id && item.Name == name, cancellationToken))
        {
            throw new BusinessConflictException($"Ya existe otra categoría llamada '{name}'.");
        }

        ApplyCategoryRequest(category, request);
        await _context.SaveChangesAsync(cancellationToken);
        return await GetCategoryAsync(id, cancellationToken);
    }

    public async Task DeleteCategoryAsync(int id, CancellationToken cancellationToken)
    {
        var category = await _context.AssetCategories.FirstOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new EntityNotFoundException($"No existe la categoría con id {id}.");
        if (await _context.Assets.AnyAsync(asset => asset.CategoryId == id, cancellationToken))
        {
            throw new BusinessConflictException("No se puede eliminar una categoría que tiene activos asociados.");
        }

        _context.AssetCategories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task<AssetCategory> GetCategoryEntityAsync(int id, CancellationToken cancellationToken) =>
        await _context.AssetCategories.AsNoTracking()
            .Include(category => category.Assets.Where(asset => asset.IsActive))
            .FirstOrDefaultAsync(category => category.Id == id && category.IsActive, cancellationToken)
        ?? throw new EntityNotFoundException($"No existe la categoría activa con id {id}.");

    private static void ApplyCategoryRequest(AssetCategory category, CategoryWriteRequest request)
    {
        category.Name = request.Name.Trim();
        category.Description = request.Description?.Trim();
        category.PreventiveMaintenanceIntervalDays = request.PreventiveMaintenanceIntervalDays;
        category.IsActive = true;
    }
}
