using MapsterMapper;
using Proyecto.Application.Interfaces;
using Proyecto.Domain.DTO;
using Proyecto.Domain.DTO.ResultadoPatron;
using Proyecto.Infraestructure.Interfaces;

namespace Proyecto.Application.Implementation;

// Antes accedía directo al DbContext; ahora pasa por ICatalogoRepository como el resto de servicios
public class CatalogoService : ICatalogoService
{
    private readonly ICatalogoRepository _repositorio;
    private readonly IMapper _mapeador;

    public CatalogoService(ICatalogoRepository repositorio, IMapper mapeador)
    {
        _repositorio = repositorio;
        _mapeador = mapeador;
    }

    public async Task<Resultado<IEnumerable<UbicacionListaDto>>> ObtenerUbicacionesAsync()
    {
        var ubicaciones = await _repositorio.ObtenerUbicacionesActivasAsync();

        var datos = _mapeador.Map<IEnumerable<UbicacionListaDto>>(ubicaciones);

        return Resultado<IEnumerable<UbicacionListaDto>>.Exito(datos);
    }

    public async Task<Resultado<IEnumerable<TipoMantenimientoListaDto>>> ObtenerTiposMantenimientoAsync()
    {
        var tipos = await _repositorio.ObtenerTiposMantenimientoAsync();

        var datos = _mapeador.Map<IEnumerable<TipoMantenimientoListaDto>>(tipos);

        return Resultado<IEnumerable<TipoMantenimientoListaDto>>.Exito(datos);
    }
}
