using Proyecto.Domain.Entities;

namespace Proyecto.Infraestructure.Interfaces;

public interface ICategoriaActivoRepository
{
    // Lectura
    Task<IEnumerable<CategoriaActivo>> ObtenerTodosAsync();
    Task<IEnumerable<CategoriaActivo>> ObtenerPaginacionAsync(int paginaNumero, int tamanoPagina);
    Task<int> ContarAsync();
    Task<CategoriaActivo?> ObtenerPorIdentificadorAsync(int id);
    Task<Dictionary<int, int>> ContarActivosPorCategoriaAsync(IEnumerable<int> idsCategorias);

    // Verificaciones para las reglas de negocio
    Task<bool> ExisteAsync(int id);
    Task<bool> ExistePorNombreAsync(string name, int? excludeId = null);
    Task<bool> ExistePorPrefijoCodigoAsync(string codePrefix, int? excludeId = null);
    Task<bool> TieneActivosAsync(int id);

    // Escritura
    Task AgregarAsync(CategoriaActivo categoria);
    Task ActualizarAsync(CategoriaActivo categoria);
    Task EliminarAsync(int id);
}
