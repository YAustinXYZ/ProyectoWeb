using Microsoft.EntityFrameworkCore;
using Proyecto.Application.Interfaces;
using Proyecto.Infraestructure.Data;

namespace Proyecto.Infraestructure.Services;

public sealed partial class AssetManagementService(AppDbContext context) : IAssetManagementService
{
    private readonly AppDbContext _context = context;
}
