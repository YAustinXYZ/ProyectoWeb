using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Proyecto.Application.Interfaces;
using Proyecto.Domain.DTO;
using Proyecto.Domain.DTO.ResultadoPatron;

namespace Proyecto.WebAPI.Controllers;

/// <summary>Solicitudes de mantenimiento: registro y consulta.</summary>
[Route("api/maintenance-orders")]
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

    /// <summary>Obtiene la lista de órdenes de mantenimiento.</summary>
    /// <remarks>Se puede filtrar por activo. Cada orden incluye el código y el nombre del activo y los días transcurridos desde la solicitud.</remarks>
    /// <param name="activoId">Id del activo (opcional).</param>
    /// <response code="200">Lista de órdenes.</response>
    /// <response code="400">El filtro no es válido.</response>
    /// <response code="404">No existe el activo indicado en el filtro.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
    [HttpGet]
    [ProducesResponseType(typeof(Resultado<IEnumerable<OrdenMantenimientoListaDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerTodosAsync(
        [FromQuery(Name = "assetId")] [Range(1, int.MaxValue, ErrorMessage = "El filtro assetId debe ser 1 o mayor.")] int? activoId)
    {
        _registro.LogInformation("Listando órdenes de mantenimiento (assetId={ActivoId})", activoId);

        var resultado = activoId is null
            ? await _servicio.ObtenerTodosAsync()
            : await _servicio.ObtenerPorActivoAsync(activoId.Value);

        _registro.LogInformation("Listado de órdenes completado: {Cantidad} registros", resultado.Datos?.Count() ?? 0);

        return StatusCode(resultado.Estado, resultado);
    }

    /// <summary>Obtiene la lista de órdenes por páginas.</summary>
    /// <param name="paginaNumero">Número de página, empezando en 1.</param>
    /// <param name="tamanoPagina">Cantidad de registros por página (entre 1 y 50).</param>
    /// <response code="200">Página de órdenes.</response>
    /// <response code="400">La página o el tamaño no son válidos.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
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

    /// <summary>Obtiene el detalle de una orden.</summary>
    /// <remarks>Incluye el activo relacionado (código, nombre, estado y categoría) y los días transcurridos desde la solicitud.</remarks>
    /// <param name="id">Id de la orden.</param>
    /// <response code="200">Detalle de la orden.</response>
    /// <response code="404">No existe una orden con el id indicado.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
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

    /// <summary>Registra una solicitud de mantenimiento.</summary>
    /// <remarks>La orden siempre se crea en estado Requested y el sistema asigna su código (por ejemplo, OT-2026-0011). No se permite para un activo dado de baja.</remarks>
    /// <response code="201">Solicitud registrada.</response>
    /// <response code="400">Los datos no son válidos, o el tipo de mantenimiento o el proveedor no existen.</response>
    /// <response code="404">No existe el activo indicado.</response>
    /// <response code="409">El activo está dado de baja, el proveedor está inactivo o el activo ya tiene una orden abierta.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
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
