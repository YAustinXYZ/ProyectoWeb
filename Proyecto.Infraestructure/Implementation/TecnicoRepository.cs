using Microsoft.EntityFrameworkCore;
using Proyecto.Domain.Entities;
using Proyecto.Infraestructure.Data;
using Proyecto.Infraestructure.Interfaces;

namespace Proyecto.Infraestructure.Implementation;

public class TecnicoRepository : ITecnicoRepository
{
    private readonly ContextoAplicacion _contexto;

    public TecnicoRepository(ContextoAplicacion contexto)
    {
        _contexto = contexto;
    }

    // Consultas de lectura sin seguimiento de cambios (AsNoTracking)

    // Filtros opcionales: por especialidad (técnicos que la poseen) y por disponibilidad
    public async Task<IEnumerable<Tecnico>> ObtenerTodosAsync(int? especialidadId = null, bool? estaDisponible = null)
    {
        var consulta = _contexto.Tecnicos.AsNoTracking();

        if (especialidadId is not null)
            consulta = consulta.Where(t => t.EspecialidadesTecnico.Any(e => e.EspecialidadId == especialidadId));

        if (estaDisponible is not null)
            consulta = consulta.Where(t => t.EstaDisponible == estaDisponible);

        var resultado = await consulta
            .OrderBy(t => t.NombreCompleto)
            .ToListAsync();

        return resultado;
    }

    public async Task<Tecnico?> ObtenerPorIdentificadorAsync(int id)
    {
        var resultado = await _contexto.Tecnicos
            .AsNoTracking()
            .Include(t => t.EspecialidadesTecnico)
                .ThenInclude(e => e.Especialidad)
            .FirstOrDefaultAsync(t => t.Identificador == id);

        return resultado;
    }

    // Con seguimiento de cambios: SaveChangesAsync actualiza solo las columnas modificadas
    public async Task<Tecnico?> ObtenerParaActualizarAsync(int id)
    {
        var resultado = await _contexto.Tecnicos
            .FirstOrDefaultAsync(t => t.Identificador == id);

        return resultado;
    }

    // Un solo SELECT ... GROUP BY TechnicianId; los técnicos sin especialidades valen 0
    public async Task<Dictionary<int, int>> ContarEspecialidadesPorTecnicoAsync(IEnumerable<int> idsTecnicos)
    {
        var ids = idsTecnicos.ToList();

        var resultado = await _contexto.EspecialidadesTecnico
            .Where(e => ids.Contains(e.TecnicoId))
            .GroupBy(e => e.TecnicoId)
            .Select(grupo => new { TecnicoId = grupo.Key, Cantidad = grupo.Count() })
            .ToDictionaryAsync(x => x.TecnicoId, x => x.Cantidad);

        return resultado;
    }

    // Verificaciones de existencia

    public async Task<bool> ExisteAsync(int id)
    {
        var resultado = await _contexto.Tecnicos.AnyAsync(t => t.Identificador == id);

        return resultado;
    }

    // excluirId: técnico que se excluye de la verificación al editar
    public async Task<bool> ExistePorIdentificacionAsync(string numeroIdentificacion, int? excluirId = null)
    {
        var resultado = await _contexto.Tecnicos
            .AnyAsync(t => t.NumeroIdentificacion == numeroIdentificacion
                        && (excluirId == null || t.Identificador != excluirId));

        return resultado;
    }

    public async Task<bool> ExisteEspecialidadAsync(int especialidadId)
    {
        var resultado = await _contexto.Especialidades.AnyAsync(e => e.Identificador == especialidadId);

        return resultado;
    }

    // Con seguimiento: se usa para eliminar la asignación
    public async Task<TecnicoEspecialidad?> ObtenerAsignacionAsync(int tecnicoId, int especialidadId)
    {
        var resultado = await _contexto.EspecialidadesTecnico
            .FirstOrDefaultAsync(e => e.TecnicoId == tecnicoId && e.EspecialidadId == especialidadId);

        return resultado;
    }

    // Operaciones de escritura

    public async Task AgregarAsync(Tecnico tecnico)
    {
        // Identificador generado por la base de datos (IDENTITY)
        _contexto.Tecnicos.Add(tecnico);
        await _contexto.SaveChangesAsync();
    }

    public async Task AgregarAsignacionAsync(TecnicoEspecialidad asignacion)
    {
        _contexto.EspecialidadesTecnico.Add(asignacion);
        await _contexto.SaveChangesAsync();
    }

    public async Task EliminarAsignacionAsync(TecnicoEspecialidad asignacion)
    {
        _contexto.EspecialidadesTecnico.Remove(asignacion);
        await _contexto.SaveChangesAsync();
    }

    public async Task GuardarCambiosAsync()
    {
        await _contexto.SaveChangesAsync();
    }
}
