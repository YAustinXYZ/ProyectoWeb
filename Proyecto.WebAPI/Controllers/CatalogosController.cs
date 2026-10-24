using Microsoft.AspNetCore.Mvc;
using Proyecto.Application.Interfaces;
using Proyecto.Domain.DTO;
using Proyecto.Domain.DTO.ResultadoPatron;

namespace Proyecto.WebAPI.Controllers;

/// <summary>Catálogos de solo lectura para los formularios: ubicaciones y tipos de mantenimiento.</summary>
[Route("api")]
[ApiController]
[Produces("application/json")]
public class CatalogosController : ControllerBase
{
    private readonly ICatalogoService _servicio;
    private readonly ILogger<CatalogosController> _registro;

    public CatalogosController(ICatalogoService servicio, ILogger<CatalogosController> registro)
    {
        _servicio = servicio;
        _registro = registro;
    }

    /// <summary>Lista las ubicaciones activas.</summary>
    [HttpGet("locations")]
    [ProducesResponseType(typeof(Resultado<IEnumerable<UbicacionListaDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerUbicacionesAsync()
    {
        _registro.LogInformation("Listando ubicaciones");

        var resultado = await _servicio.ObtenerUbicacionesAsync();

        _registro.LogInformation("Listado de ubicaciones completado: {Cantidad} registros", resultado.Datos?.Count() ?? 0);

        return StatusCode(resultado.Estado, resultado);
    }

    /// <summary>Lista los tipos de mantenimiento con la especialidad que requiere cada uno.</summary>
    [HttpGet("maintenance-types")]
    [ProducesResponseType(typeof(Resultado<IEnumerable<TipoMantenimientoListaDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerTiposMantenimientoAsync()
    {
        _registro.LogInformation("Listando tipos de mantenimiento");

        var resultado = await _servicio.ObtenerTiposMantenimientoAsync();

        _registro.LogInformation("Listado de tipos de mantenimiento completado: {Cantidad} registros", resultado.Datos?.Count() ?? 0);

        return StatusCode(resultado.Estado, resultado);
    }
}
