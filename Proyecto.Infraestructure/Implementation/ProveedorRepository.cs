using Microsoft.EntityFrameworkCore;
using Proyecto.Domain.Entities;
using Proyecto.Infraestructure.Data;
using Proyecto.Infraestructure.Interfaces;

namespace Proyecto.Infraestructure.Implementation;

public class ProveedorRepository : IProveedorRepository
{
    private readonly ContextoAplicacion _contexto;

    public ProveedorRepository(ContextoAplicacion contexto)
    {
        _contexto = contexto;
    }

    // Consultas de lectura sin seguimiento de cambios (AsNoTracking)

    public async Task<IEnumerable<Proveedor>> ObtenerTodosAsync()
    {
        var resultado = await _contexto.Proveedores
            .AsNoTracking()
            .OrderBy(s => s.Identificador)
            .ToListAsync();

        return resultado;
    }

    public async Task<IEnumerable<Proveedor>> ObtenerPaginacionAsync(int paginaNumero, int tamanoPagina)
    {
        var resultado = await _contexto.Proveedores
            .AsNoTracking()
            .OrderBy(s => s.Identificador)                    // Orden necesario para Skip/Take
            .Skip((paginaNumero - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToListAsync();

        return resultado;
    }

    // Cantidad total de registros, utilizada en la paginación
    public async Task<int> ContarAsync()
    {
        var resultado = await _contexto.Proveedores.CountAsync();

        return resultado;
    }

    public async Task<Proveedor?> ObtenerPorIdentificadorAsync(int id)
    {
        var resultado = await _contexto.Proveedores
            .AsNoTracking()
            .Include(s => s.RepuestosProveedor)                // Repuestos que suministra (detalle)
                .ThenInclude(enlace => enlace.Repuesto)
            .FirstOrDefaultAsync(s => s.Identificador == id);

        return resultado;
    }

    // Verificaciones de existencia mediante AnyAsync

    public async Task<bool> ExisteAsync(int id)
    {
        var resultado = await _contexto.Proveedores
            .AnyAsync(s => s.Identificador == id);

        return resultado;
    }

    // excludeId: Identificador del proveedor que se excluye de la verificación al editar
    public async Task<bool> ExistePorIdentificacionFiscalAsync(string taxId, int? excludeId = null)
    {
        var resultado = await _contexto.Proveedores
            .AnyAsync(s => s.IdentificacionFiscal == taxId
                        && (excludeId == null || s.Identificador != excludeId));

        return resultado;
    }

    // Reúne en un solo lugar todas las relaciones que impiden eliminar un proveedor
    public async Task<bool> TieneRegistrosRelacionadosAsync(int id)
    {
        var resultado = await _contexto.Proveedores
            .Where(s => s.Identificador == id)
            .AnyAsync(s => s.Activos.Any() || s.OrdenesMantenimiento.Any() || s.RepuestosProveedor.Any());

        return resultado;
    }

    // Operaciones de escritura
    public async Task AgregarAsync(Proveedor proveedor)
    {
        // Identificador generado por la base de datos (IDENTITY)
        _contexto.Proveedores.Add(proveedor);
        await _contexto.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Proveedor proveedor)
    {
        // Actualiza todas las columnas de la entidad
        _contexto.Proveedores.Update(proveedor);
        await _contexto.SaveChangesAsync();
    }

    public async Task EliminarAsync(int id)
    {
        // Eliminación directa en la base de datos, sin cargar la entidad
        await _contexto.Proveedores
            .Where(s => s.Identificador == id)
            .ExecuteDeleteAsync();
    }
}
