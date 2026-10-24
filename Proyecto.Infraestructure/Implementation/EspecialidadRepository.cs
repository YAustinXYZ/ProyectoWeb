using Microsoft.EntityFrameworkCore;
using Proyecto.Domain.Entities;
using Proyecto.Infraestructure.Data;
using Proyecto.Infraestructure.Interfaces;

namespace Proyecto.Infraestructure.Implementation;

public class EspecialidadRepository : IEspecialidadRepository
{
    private readonly ContextoAplicacion _contexto;

    public EspecialidadRepository(ContextoAplicacion contexto)
    {
        _contexto = contexto;
    }

    public async Task<IEnumerable<Especialidad>> ObtenerTodosAsync()
    {
        var resultado = await _contexto.Especialidades
            .AsNoTracking()
            .OrderBy(e => e.Nombre)
            .ToListAsync();

        return resultado;
    }

    // El detalle muestra los técnicos que poseen la especialidad
    public async Task<Especialidad?> ObtenerPorIdentificadorAsync(int id)
    {
        var resultado = await _contexto.Especialidades
            .AsNoTracking()
            .Include(e => e.EspecialidadesTecnico)
                .ThenInclude(enlace => enlace.Tecnico)
            .FirstOrDefaultAsync(e => e.Identificador == id);

        return resultado;
    }

    public async Task<Especialidad?> ObtenerParaActualizarAsync(int id)
    {
        var resultado = await _contexto.Especialidades
            .FirstOrDefaultAsync(e => e.Identificador == id);

        return resultado;
    }

    // Un solo SELECT ... GROUP BY SpecialtyId; las especialidades sin técnicos valen 0
    public async Task<Dictionary<int, int>> ContarTecnicosPorEspecialidadAsync(IEnumerable<int> idsEspecialidades)
    {
        var ids = idsEspecialidades.ToList();

        var resultado = await _contexto.EspecialidadesTecnico
            .Where(e => ids.Contains(e.EspecialidadId))
            .GroupBy(e => e.EspecialidadId)
            .Select(grupo => new { EspecialidadId = grupo.Key, Cantidad = grupo.Count() })
            .ToDictionaryAsync(x => x.EspecialidadId, x => x.Cantidad);

        return resultado;
    }

    // excluirId: especialidad que se excluye de la verificación al editar
    public async Task<bool> ExistePorNombreAsync(string nombre, int? excluirId = null)
    {
        var resultado = await _contexto.Especialidades
            .AnyAsync(e => e.Nombre == nombre && (excluirId == null || e.Identificador != excluirId));

        return resultado;
    }

    public async Task AgregarAsync(Especialidad especialidad)
    {
        _contexto.Especialidades.Add(especialidad);
        await _contexto.SaveChangesAsync();
    }

    public async Task GuardarCambiosAsync()
    {
        await _contexto.SaveChangesAsync();
    }
}
