using Proyecto.Domain.DTO;
using Proyecto.Domain.DTO.ResultadoPatron;

namespace Proyecto.Application.Interfaces;

public interface IRepuestoService
{
    Task<Resultado<IEnumerable<RepuestoListaDto>>> ObtenerTodosAsync(bool soloBajoMinimo = false);
    Task<Resultado<IEnumerable<RepuestoListaDto>>> ObtenerPaginacionAsync(int paginaNumero, int tamanoPagina);
    Task<Resultado<RepuestoDetalleDto>> ObtenerPorIdentificadorAsync(int id);
    Task<Resultado<RepuestoDetalleDto>> AgregarAsync(RepuestoCrearDto repuesto);
    Task<Resultado<RepuestoDetalleDto>> ActualizarAsync(int id, RepuestoActualizarDto repuesto);
    Task<Resultado<RepuestoDetalleDto>> AsociarProveedorAsync(int repuestoId, int proveedorId, CondicionesProveedorRepuestoDto condiciones);
    Task<Resultado<RepuestoDetalleDto>> ActualizarCondicionesProveedorAsync(int repuestoId, int proveedorId, CondicionesProveedorRepuestoDto condiciones);
}
