using Microsoft.AspNetCore.Mvc;
using Proyecto.Application.Interfaces;
using Proyecto.Domain.DTO;
using Proyecto.Domain.DTO.ResultadoPatron;

namespace Proyecto.WebAPI.Controllers;

/// <summary>Órdenes de mantenimiento (alcance del Avance 1: solicitud y consulta).</summary>
[Route("api/MaintenanceOrder")]
[ApiController]
[Produces("application/json")]
public class OrdenMantenimientoController : ControllerBase
{
    private readonly IOrdenMantenimientoService _servicio;
    private readonly ILogger<OrdenMantenimientoController> _registro;

    public OrdenMantenimientoController(IOrdenMantenimientoService servicio, ILogger<OrdenMantenimientoController> registro)
    {
        _servicio = servicio;
        _registro = registro;
    }

    /// <summary>Lista las órdenes con los días transcurridos desde la solicitud.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(Resultado<IEnumerable<OrdenMantenimientoListaDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerTodosAsync()
    {
        _registro.LogInformation("Listando órdenes de mantenimiento");

        var resultado = await _servicio.ObtenerTodosAsync();

        _registro.LogInformation("Listado de órdenes completado: {Cantidad} registros", resultado.Datos?.Count() ?? 0);

        return StatusCode(resultado.Estado, resultado);
    }

    /// <summary>Lista las órdenes por páginas.</summary>
    [HttpGet("{pageNumber:int}/{pageSize:int}")]
    [ProducesResponseType(typeof(Resultado<IEnumerable<OrdenMantenimientoListaDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ObtenerPaginacionAsync([FromRoute(Name = "pageNumber")] int paginaNumero, [FromRoute(Name = "pageSize")] int tamanoPagina)
    {
        _registro.LogInformation("Listando órdenes página {Pagina} de tamaño {Tamano}", paginaNumero, tamanoPagina);

        var resultado = await _servicio.ObtenerPaginacionAsync(paginaNumero, tamanoPagina);

        _registro.LogInformation("Página {Pagina} de órdenes completada", paginaNumero);

        return StatusCode(resultado.Estado, resultado);
    }

    /// <summary>Detalle de una orden con el activo relacionado (código, nombre, estado y categoría).</summary>
    [HttpGet("{id:int}", Name = "GetMaintenanceOrderById")]
    [ProducesResponseType(typeof(Resultado<OrdenMantenimientoDetalleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorIdentificadorAsync(int id)
    {
        _registro.LogInformation("Consultando la orden {Id}", id);

        var resultado = await _servicio.ObtenerPorIdentificadorAsync(id);

        _registro.LogInformation("Orden {Id} consultada", id);

        return StatusCode(resultado.Estado, resultado);
    }

    /// <summary>Lista las órdenes de un activo (filtro por activo).</summary>
    [HttpGet("by-asset/{assetId:int}")]
    [ProducesResponseType(typeof(Resultado<IEnumerable<OrdenMantenimientoListaDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorActivoAsync(int assetId)
    {
        _registro.LogInformation("Listando las órdenes del activo {ActivoId}", assetId);

        var resultado = await _servicio.ObtenerPorActivoAsync(assetId);

        _registro.LogInformation("Órdenes del activo {ActivoId} listadas: {Cantidad}", assetId, resultado.Datos?.Count() ?? 0);

        return StatusCode(resultado.Estado, resultado);
    }

    /// <summary>Crea una solicitud de orden; el estado siempre inicia en Requested.</summary>
    /// <remarks>404 si el activo no existe; 400 si el tipo o el proveedor no existen; 409 si el activo está dado de baja.</remarks>
    [HttpPost]
    [ProducesResponseType(typeof(Resultado<OrdenMantenimientoDetalleDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AgregarAsync([FromBody] OrdenMantenimientoCrearDto orden)
    {
        _registro.LogInformation("Creando una orden para el activo {ActivoId}", orden.ActivoId);

        var resultado = await _servicio.AgregarAsync(orden);

        _registro.LogInformation("Orden {Codigo} creada con id {Id}", resultado.Datos!.CodigoOrdenTrabajo, resultado.Datos.Identificador);

        return CreatedAtRoute("GetMaintenanceOrderById", new { id = resultado.Datos.Identificador }, resultado);
    }
}
