using Proyecto.Domain.DTO;
using Proyecto.Domain.DTO.ResultadoPatron;

namespace Proyecto.Application.Interfaces;

public interface ITecnicoService
{
    Task<Resultado<IEnumerable<TecnicoListaDto>>> ObtenerTodosAsync(int? especialidadId = null, bool? estaDisponible = null);
    Task<Resultado<TecnicoDetalleDto>> ObtenerPorIdentificadorAsync(int id);
    Task<Resultado<TecnicoDetalleDto>> AgregarAsync(TecnicoCrearDto tecnico);
    Task<Resultado<TecnicoDetalleDto>> ActualizarAsync(int id, TecnicoActualizarDto tecnico);
    Task<Resultado<TecnicoDetalleDto>> AsociarEspecialidadAsync(int tecnicoId, int especialidadId, SolicitudEspecialidadTecnicoDto solicitud);
    Task<Resultado<TecnicoDetalleDto>> DesasociarEspecialidadAsync(int tecnicoId, int especialidadId);
}
