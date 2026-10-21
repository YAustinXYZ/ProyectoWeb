using Microsoft.EntityFrameworkCore;
using Proyecto.Domain.Entities;
using Proyecto.Domain.Exceptions;
using Proyecto.Infraestructure.Data;
using Proyecto.Infraestructure.Services;

namespace Proyecto.Tests;

public sealed class AssetManagementServiceTests
{
    [Fact]
    public async Task GetAssetAsync_DoesNotReturnInactiveAssets()
    {
        await using var context = await CreateContextAsync();
        var asset = await context.Assets.FirstAsync();
        asset.IsActive = false;
        await context.SaveChangesAsync();

        var service = new AssetManagementService(context);

        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => service.GetAssetAsync(asset.Id, CancellationToken.None));
    }

    [Theory]
    [InlineData("Solicitada")]
    [InlineData("Planned")]
    [InlineData("InProgress")]
    public async Task ChangeAssetStatusAsync_RejectsAssetsWithOpenOrders(string status)
    {
        await using var context = await CreateContextAsync();
        var order = await context.MaintenanceOrders.FirstAsync();
        order.Status = status;
        await context.SaveChangesAsync();

        var service = new AssetManagementService(context);

        await Assert.ThrowsAsync<BusinessConflictException>(
            () => service.ChangeAssetStatusAsync(order.AssetId, "Fuera de Servicio", CancellationToken.None));
    }

    [Fact]
    public async Task GetSpecialtyAsync_ReturnsCountMatchingItsTechnicianList()
    {
        await using var context = await CreateContextAsync();
        var specialty = await context.Specialties.FirstAsync();
        var service = new AssetManagementService(context);

        var result = await service.GetSpecialtyAsync(specialty.Id, CancellationToken.None);

        Assert.Equal(result.Technicians.Count, result.TechnicianCount);
    }

    [Fact]
    public async Task DeleteCategoryAsync_RemovesUnassociatedCategory()
    {
        await using var context = await CreateContextAsync();
        var category = new AssetCategory { Name = "Categoría temporal de prueba" };
        context.AssetCategories.Add(category);
        await context.SaveChangesAsync();
        var service = new AssetManagementService(context);

        await service.DeleteCategoryAsync(category.Id, CancellationToken.None);

        Assert.False(await context.AssetCategories.AnyAsync(item => item.Id == category.Id));
    }

    [Fact]
    public async Task DeleteCategoryAsync_RejectsCategoryWithAssociatedAssets()
    {
        await using var context = await CreateContextAsync();
        var category = await context.AssetCategories.FirstAsync();
        var service = new AssetManagementService(context);

        await Assert.ThrowsAsync<BusinessConflictException>(
            () => service.DeleteCategoryAsync(category.Id, CancellationToken.None));
    }

    private static async Task<AppDbContext> CreateContextAsync()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();
        return context;
    }
}
