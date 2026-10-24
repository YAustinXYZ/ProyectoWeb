using Proyecto.Domain.Entities;

namespace Proyecto.Infraestructure.Interfaces;

public interface IEspecialidadRepository
{
    // Lectura
    Task<IEnumerable<Especialidad>> ObtenerTodosAsync();
    Task<Especialidad?> ObtenerPorIdentificadorAsync(int id);
    Task<Especialidad?> ObtenerParaActualizarAsync(int id);
    Task<Dictionary<int, int>> ContarTecnicosPorEspecialidadAsync(IEnumerable<int> idsEspecialidades);

    // Verificaciones para las reglas de negocio
    Task<bool> ExistePorNombreAsync(string nombre, int? excluirId = null);

    // Escritura
    Task AgregarAsync(Especialidad especialidad);
    Task GuardarCambiosAsync();
}
