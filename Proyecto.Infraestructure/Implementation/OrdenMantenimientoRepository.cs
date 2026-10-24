using Microsoft.EntityFrameworkCore;
using Proyecto.Domain.Entities;
using Proyecto.Infraestructure.Data;
using Proyecto.Infraestructure.Interfaces;

namespace Proyecto.Infraestructure.Implementation;

public class OrdenMantenimientoRepository : IOrdenMantenimientoRepository
{
    private readonly ContextoAplicacion _contexto;

    public OrdenMantenimientoRepository(ContextoAplicacion contexto)
    {
        _contexto = contexto;
    }

    // Consultas de lectura sin seguimiento de cambios (AsNoTracking)

    public async Task<IEnumerable<OrdenMantenimiento>> ObtenerTodosAsync()
    {
        var resultado = await _contexto.OrdenesMantenimiento
            .AsNoTracking()
            .Include(o => o.Activo)
            .Include(o => o.TipoMantenimiento)
            .Include(o => o.Proveedor)
            .OrderBy(o => o.Identificador)
            .ToListAsync();

        return resultado;
    }

    public async Task<IEnumerable<OrdenMantenimiento>> ObtenerPaginacionAsync(int paginaNumero, int tamanoPagina)
    {
        var resultado = await _contexto.OrdenesMantenimiento
            .AsNoTracking()
            .Include(o => o.Activo)
            .Include(o => o.TipoMantenimiento)
            .Include(o => o.Proveedor)
            .OrderBy(o => o.Identificador)                    // Orden necesario para Skip/Take
            .Skip((paginaNumero - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToListAsync();

        return resultado;
    }

    // Cantidad total de registros, utilizada en la paginación
    public async Task<int> ContarAsync()
    {
        var resultado = await _contexto.OrdenesMantenimiento.CountAsync();

        return resultado;
    }

    public async Task<OrdenMantenimiento?> ObtenerPorIdentificadorAsync(int id)
    {
        var resultado = await _contexto.OrdenesMantenimiento
            .AsNoTracking()
            .Include(o => o.Activo)
                .ThenInclude(a => a.Categoria)               // El detalle muestra la categoría del activo
            .Include(o => o.TipoMantenimiento)
            .Include(o => o.Proveedor)
            .FirstOrDefaultAsync(o => o.Identificador == id);

        return resultado;
    }

    public async Task<IEnumerable<OrdenMantenimiento>> ObtenerPorActivoAsync(int assetId)
    {
        var resultado = await _contexto.OrdenesMantenimiento
            .AsNoTracking()
            .Include(o => o.Activo)
            .Include(o => o.TipoMantenimiento)
            .Include(o => o.Proveedor)
            .Where(o => o.ActivoId == assetId)
            .OrderByDescending(o => o.SolicitadoEn)
            .ThenByDescending(o => o.Identificador)           // Desempate para órdenes con la misma fecha
            .ToListAsync();

        return resultado;
    }

    // Verificaciones: cada método devuelve null si el registro no existe

    public async Task<string?> ObtenerEstadoActivoAsync(int assetId)
    {
        var resultado = await _contexto.Activos
            .Where(a => a.Identificador == assetId)
            .Select(a => a.Estado)
            .FirstOrDefaultAsync();

        return resultado;
    }

    public async Task<bool?> ObtenerSiTipoMantenimientoEsEmergenciaAsync(int maintenanceTypeId)
    {
        var resultado = await _contexto.TiposMantenimiento
            .Where(t => t.Identificador == maintenanceTypeId)
            .Select(t => (bool?)t.EsEmergencia)
            .FirstOrDefaultAsync();

        return resultado;
    }

    public async Task<bool?> ObtenerSiProveedorEstaActivoAsync(int supplierId)
    {
        var resultado = await _contexto.Proveedores
            .Where(s => s.Identificador == supplierId)
            .Select(s => (bool?)s.EstaActivo)
            .FirstOrDefaultAsync();

        return resultado;
    }

    public async Task<bool> TieneOrdenesAbiertasAsync(int assetId, IEnumerable<string> closedStatuses)
    {
        var resultado = await _contexto.OrdenesMantenimiento
            .AnyAsync(o => o.ActivoId == assetId && !closedStatuses.Contains(o.Estado));

        return resultado;
    }

    // El prefijo incluye el año ("OT-2026-"), por lo que el consecutivo se reinicia cada año
    public async Task<string?> ObtenerUltimoCodigoOrdenTrabajoAsync(string codePrefix)
    {
        var resultado = await _contexto.OrdenesMantenimiento
            .Where(o => o.CodigoOrdenTrabajo.StartsWith(codePrefix))
            .OrderByDescending(o => o.CodigoOrdenTrabajo)
            .Select(o => o.CodigoOrdenTrabajo)
            .FirstOrDefaultAsync();

        return resultado;
    }

    // Operaciones de escritura
    public async Task AgregarAsync(OrdenMantenimiento orden)
    {
        // Identificador generado por la base de datos (IDENTITY)
        _contexto.OrdenesMantenimiento.Add(orden);
        await _contexto.SaveChangesAsync();
    }
}
