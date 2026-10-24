using Microsoft.EntityFrameworkCore;
using Proyecto.Domain.Entities;
using Proyecto.Infraestructure.Data;
using Proyecto.Infraestructure.Interfaces;

namespace Proyecto.Infraestructure.Implementation;

public class CatalogoRepository : ICatalogoRepository
{
    private readonly ContextoAplicacion _contexto;

    public CatalogoRepository(ContextoAplicacion contexto)
    {
        _contexto = contexto;
    }

    public async Task<IEnumerable<Ubicacion>> ObtenerUbicacionesActivasAsync()
    {
        var resultado = await _contexto.Ubicaciones
            .AsNoTracking()
            .Where(u => u.EstaActivo)
            .OrderBy(u => u.Nombre)
            .ToListAsync();

        return resultado;
    }

    public async Task<IEnumerable<TipoMantenimiento>> ObtenerTiposMantenimientoAsync()
    {
        var resultado = await _contexto.TiposMantenimiento
            .AsNoTracking()
            .Include(t => t.EspecialidadRequerida)
            .OrderBy(t => t.Nombre)
            .ToListAsync();

        return resultado;
    }
}
