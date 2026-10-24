using Proyecto.Domain.Entities;

namespace Proyecto.Infraestructure.Interfaces;

public interface ITecnicoRepository
{
    // Lectura
    Task<IEnumerable<Tecnico>> ObtenerTodosAsync(int? especialidadId = null, bool? estaDisponible = null);
    Task<Tecnico?> ObtenerPorIdentificadorAsync(int id);
    Task<Tecnico?> ObtenerParaActualizarAsync(int id);
    Task<Dictionary<int, int>> ContarEspecialidadesPorTecnicoAsync(IEnumerable<int> idsTecnicos);

    // Verificaciones para las reglas de negocio
    Task<bool> ExisteAsync(int id);
    Task<bool> ExistePorIdentificacionAsync(string numeroIdentificacion, int? excluirId = null);
    Task<bool> ExisteEspecialidadAsync(int especialidadId);
    Task<TecnicoEspecialidad?> ObtenerAsignacionAsync(int tecnicoId, int especialidadId);

    // Escritura
    Task AgregarAsync(Tecnico tecnico);
    Task AgregarAsignacionAsync(TecnicoEspecialidad asignacion);
    Task EliminarAsignacionAsync(TecnicoEspecialidad asignacion);
    Task GuardarCambiosAsync();
}
