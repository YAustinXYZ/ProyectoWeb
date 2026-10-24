using Proyecto.Domain.DTO;
using Proyecto.Domain.DTO.ResultadoPatron;

namespace Proyecto.Application.Interfaces;

public interface ICatalogoService
{
    Task<Resultado<IEnumerable<UbicacionListaDto>>> ObtenerUbicacionesAsync();
    Task<Resultado<IEnumerable<TipoMantenimientoListaDto>>> ObtenerTiposMantenimientoAsync();
}
