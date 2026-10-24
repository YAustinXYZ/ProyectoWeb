using Proyecto.Domain.Entities;

namespace Proyecto.Infraestructure.Interfaces;

public interface IRepuestoRepository
{
    // Lectura
    Task<IEnumerable<Repuesto>> ObtenerTodosAsync(bool soloBajoMinimo = false);
    Task<IEnumerable<Repuesto>> ObtenerPaginacionAsync(int paginaNumero, int tamanoPagina);
    Task<int> ContarAsync();
    Task<Repuesto?> ObtenerPorIdentificadorAsync(int id);
    Task<Repuesto?> ObtenerParaActualizarAsync(int id);
    Task<Dictionary<int, int>> ContarProveedoresPorRepuestoAsync(IEnumerable<int> idsRepuestos);

    // Verificaciones para las reglas de negocio
    Task<bool> ExistePorCodigoAsync(string codigo, int? excluirId = null);
    Task<bool?> ObtenerSiProveedorEstaActivoAsync(int proveedorId);
    Task<RepuestoProveedor?> ObtenerAsociacionAsync(int repuestoId, int proveedorId);

    // Escritura
    Task AgregarAsync(Repuesto repuesto);
    Task AgregarAsociacionAsync(RepuestoProveedor asociacion);
    Task GuardarCambiosAsync();
}
