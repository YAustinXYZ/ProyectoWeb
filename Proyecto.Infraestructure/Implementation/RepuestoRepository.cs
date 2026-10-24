using Microsoft.EntityFrameworkCore;
using Proyecto.Domain.Entities;
using Proyecto.Infraestructure.Data;
using Proyecto.Infraestructure.Interfaces;

namespace Proyecto.Infraestructure.Implementation;

public class RepuestoRepository : IRepuestoRepository
{
    private readonly ContextoAplicacion _contexto;

    public RepuestoRepository(ContextoAplicacion contexto)
    {
        _contexto = contexto;
    }

    // Consultas de lectura sin seguimiento de cambios (AsNoTracking)

    // soloBajoMinimo usa la misma condición que el campo calculado isBelowMinimum
    public async Task<IEnumerable<Repuesto>> ObtenerTodosAsync(bool soloBajoMinimo = false)
    {
        var consulta = _contexto.Repuestos.AsNoTracking();

        if (soloBajoMinimo)
            consulta = consulta.Where(r => r.StockActual < r.StockMinimo);

        var resultado = await consulta
            .OrderBy(r => r.Codigo)
            .ToListAsync();

        return resultado;
    }

    public async Task<IEnumerable<Repuesto>> ObtenerPaginacionAsync(int paginaNumero, int tamanoPagina)
    {
        var resultado = await _contexto.Repuestos
            .AsNoTracking()
            .OrderBy(r => r.Codigo)                           // Orden necesario para Skip/Take
            .Skip((paginaNumero - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToListAsync();

        return resultado;
    }

    public async Task<int> ContarAsync()
    {
        var resultado = await _contexto.Repuestos.CountAsync();

        return resultado;
    }

    // El detalle muestra los proveedores con su precio y tiempo de entrega
    public async Task<Repuesto?> ObtenerPorIdentificadorAsync(int id)
    {
        var resultado = await _contexto.Repuestos
            .AsNoTracking()
            .Include(r => r.ProveedoresRepuesto)
                .ThenInclude(enlace => enlace.Proveedor)
            .FirstOrDefaultAsync(r => r.Identificador == id);

        return resultado;
    }

    public async Task<Repuesto?> ObtenerParaActualizarAsync(int id)
    {
        var resultado = await _contexto.Repuestos
            .FirstOrDefaultAsync(r => r.Identificador == id);

        return resultado;
    }

    // Un solo SELECT ... GROUP BY SparePartId; los repuestos sin proveedores valen 0
    public async Task<Dictionary<int, int>> ContarProveedoresPorRepuestoAsync(IEnumerable<int> idsRepuestos)
    {
        var ids = idsRepuestos.ToList();

        var resultado = await _contexto.RepuestosProveedor
            .Where(e => ids.Contains(e.RepuestoId))
            .GroupBy(e => e.RepuestoId)
            .Select(grupo => new { RepuestoId = grupo.Key, Cantidad = grupo.Count() })
            .ToDictionaryAsync(x => x.RepuestoId, x => x.Cantidad);

        return resultado;
    }

    // excluirId: repuesto que se excluye de la verificación al editar
    public async Task<bool> ExistePorCodigoAsync(string codigo, int? excluirId = null)
    {
        var resultado = await _contexto.Repuestos
            .AnyAsync(r => r.Codigo == codigo && (excluirId == null || r.Identificador != excluirId));

        return resultado;
    }

    // null si el proveedor no existe
    public async Task<bool?> ObtenerSiProveedorEstaActivoAsync(int proveedorId)
    {
        var resultado = await _contexto.Proveedores
            .Where(p => p.Identificador == proveedorId)
            .Select(p => (bool?)p.EstaActivo)
            .FirstOrDefaultAsync();

        return resultado;
    }

    // Con seguimiento: se usa para actualizar las condiciones
    public async Task<RepuestoProveedor?> ObtenerAsociacionAsync(int repuestoId, int proveedorId)
    {
        var resultado = await _contexto.RepuestosProveedor
            .FirstOrDefaultAsync(e => e.RepuestoId == repuestoId && e.ProveedorId == proveedorId);

        return resultado;
    }

    public async Task AgregarAsync(Repuesto repuesto)
    {
        _contexto.Repuestos.Add(repuesto);
        await _contexto.SaveChangesAsync();
    }

    public async Task AgregarAsociacionAsync(RepuestoProveedor asociacion)
    {
        _contexto.RepuestosProveedor.Add(asociacion);
        await _contexto.SaveChangesAsync();
    }

    public async Task GuardarCambiosAsync()
    {
        await _contexto.SaveChangesAsync();
    }
}
