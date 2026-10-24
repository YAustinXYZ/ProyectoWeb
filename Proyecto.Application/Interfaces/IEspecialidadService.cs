using Proyecto.Domain.DTO;
using Proyecto.Domain.DTO.ResultadoPatron;

namespace Proyecto.Application.Interfaces;

public interface IEspecialidadService
{
    Task<Resultado<IEnumerable<EspecialidadListaDto>>> ObtenerTodosAsync();
    Task<Resultado<EspecialidadDetalleDto>> ObtenerPorIdentificadorAsync(int id);
    Task<Resultado<EspecialidadDetalleDto>> AgregarAsync(EspecialidadCrearDto especialidad);
    Task<Resultado<EspecialidadDetalleDto>> ActualizarAsync(int id, EspecialidadActualizarDto especialidad);
}
