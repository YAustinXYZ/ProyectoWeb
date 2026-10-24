using Proyecto.Domain.DTO;
using Proyecto.Domain.DTO.ResultadoPatron;

namespace Proyecto.Application.Interfaces;

public interface IActivoService
{
    Task<Resultado<IEnumerable<ActivoListaDto>>> ObtenerTodosAsync(int? categoriaId = null, string? estado = null);
    Task<Resultado<IEnumerable<ActivoListaDto>>> ObtenerPaginacionAsync(int paginaNumero, int tamanoPagina, int? categoriaId = null, string? estado = null);
    Task<Resultado<ActivoDetalleDto>> ObtenerPorIdentificadorAsync(int id);
    Task<Resultado<ActivoDetalleDto>> AgregarAsync(ActivoCrearDto activo);
    Task<Resultado<ActivoDetalleDto>> ActualizarAsync(int id, ActivoActualizarDto activo);
    Task<Resultado<ActivoDetalleDto>> ActualizarEstadoAsync(int id, ActualizarEstadoActivoDto estado);
    Task<Resultado<int>> EliminarAsync(int id);
    Task<Resultado<string>> SubirImagenAsync(int id, Stream content, string nombreArchivo, long length);
    Task<Resultado<int>> EliminarImagenAsync(int id);
}
