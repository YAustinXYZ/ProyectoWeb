using Microsoft.EntityFrameworkCore;
using Proyecto.Domain.Entities;
using Proyecto.Infraestructure.Data;
using Proyecto.Infraestructure.Interfaces;

namespace Proyecto.Infraestructure.Implementation;

public class CategoriaActivoRepository : ICategoriaActivoRepository
{
    private readonly ContextoAplicacion _contexto;

    public CategoriaActivoRepository(ContextoAplicacion contexto)
    {
        _contexto = contexto;
    }

    // Consultas de lectura sin seguimiento de cambios (AsNoTracking)

    public async Task<IEnumerable<CategoriaActivo>> ObtenerTodosAsync()
    {
        var resultado = await _contexto.CategoriasActivos
            .AsNoTracking()
            .OrderBy(c => c.Identificador)
            .ToListAsync();

        return resultado;
    }

    public async Task<IEnumerable<CategoriaActivo>> ObtenerPaginacionAsync(int paginaNumero, int tamanoPagina)
    {
        var resultado = await _contexto.CategoriasActivos
            .AsNoTracking()
            .OrderBy(c => c.Identificador)                    // Orden necesario para Skip/Take
            .Skip((paginaNumero - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToListAsync();

        return resultado;
    }

    // Cantidad total de registros, utilizada en la paginación
    public async Task<int> ContarAsync()
    {
        var resultado = await _contexto.CategoriasActivos.CountAsync();

        return resultado;
    }

    public async Task<CategoriaActivo?> ObtenerPorIdentificadorAsync(int id)
    {
        var resultado = await _contexto.CategoriasActivos
            .AsNoTracking()
            .Include(c => c.Activos.OrderBy(a => a.CodigoActivo))   // Listado resumido del detalle
            .FirstOrDefaultAsync(c => c.Identificador == id);

        return resultado;
    }

    // Un solo SELECT ... GROUP BY CategoryId; las categorías sin activos no aparecen y valen 0
    public async Task<Dictionary<int, int>> ContarActivosPorCategoriaAsync(IEnumerable<int> idsCategorias)
    {
        var ids = idsCategorias.ToList();

        var resultado = await _contexto.Activos
            .Where(a => ids.Contains(a.CategoriaActivoId))
            .GroupBy(a => a.CategoriaActivoId)
            .Select(grupo => new { CategoriaId = grupo.Key, Cantidad = grupo.Count() })
            .ToDictionaryAsync(x => x.CategoriaId, x => x.Cantidad);

        return resultado;
    }

    // Verificaciones de existencia mediante AnyAsync

    public async Task<bool> ExisteAsync(int id)
    {
        var resultado = await _contexto.CategoriasActivos
            .AnyAsync(c => c.Identificador == id);

        return resultado;
    }

    // excludeId: Identificador de la categoría que se excluye de la verificación al editar
    public async Task<bool> ExistePorNombreAsync(string name, int? excludeId = null)
    {
        var resultado = await _contexto.CategoriasActivos
            .AnyAsync(c => c.Nombre == name
                        && (excludeId == null || c.Identificador != excludeId));

        return resultado;
    }

    // excludeId: Identificador de la categoría que se excluye de la verificación al editar
    public async Task<bool> ExistePorPrefijoCodigoAsync(string codePrefix, int? excludeId = null)
    {
        var resultado = await _contexto.CategoriasActivos
            .AnyAsync(c => c.PrefijoCodigo == codePrefix
                        && (excludeId == null || c.Identificador != excludeId));

        return resultado;
    }

    public async Task<bool> TieneActivosAsync(int id)
    {
        var resultado = await _contexto.Activos
            .AnyAsync(a => a.CategoriaActivoId == id);

        return resultado;
    }

    // Operaciones de escritura
    public async Task AgregarAsync(CategoriaActivo categoria)
    {
        // Identificador generado por la base de datos (IDENTITY)
        _contexto.CategoriasActivos.Add(categoria);
        await _contexto.SaveChangesAsync();
    }

    public async Task ActualizarAsync(CategoriaActivo categoria)
    {
        // Actualiza todas las columnas de la entidad
        _contexto.CategoriasActivos.Update(categoria);
        await _contexto.SaveChangesAsync();
    }

    public async Task EliminarAsync(int id)
    {
        // Eliminación directa en la base de datos, sin cargar la entidad
        await _contexto.CategoriasActivos
            .Where(c => c.Identificador == id)
            .ExecuteDeleteAsync();
    }
}
