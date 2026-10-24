using Microsoft.AspNetCore.Mvc;
using Proyecto.Application.Interfaces;
using Proyecto.Domain.DTO;
using Proyecto.Domain.DTO.ResultadoPatron;

namespace Proyecto.WebAPI.Controllers;

/// <summary>Administración de los repuestos, su stock y los proveedores que los ofrecen.</summary>
[Route("api/spare-parts")]
[ApiController]
[Produces("application/json")]
public class RepuestoController : ControllerBase
{
    private readonly IRepuestoService _servicio;
    private readonly ILogger<RepuestoController> _registro;

    public RepuestoController(IRepuestoService servicio, ILogger<RepuestoController> registro)
    {
        _servicio = servicio;
        _registro = registro;
    }

    /// <summary>Obtiene la lista de repuestos.</summary>
    /// <remarks>Cada repuesto indica si está por debajo del stock mínimo, cuántas unidades faltan para llegar al mínimo y cuántos proveedores lo ofrecen.</remarks>
    /// <response code="200">Lista de repuestos.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
    [HttpGet]
    [ProducesResponseType(typeof(Resultado<IEnumerable<RepuestoListaDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerTodosAsync()
    {
        _registro.LogInformation("Listando repuestos");

        var resultado = await _servicio.ObtenerTodosAsync();

        _registro.LogInformation("Listado de repuestos completado: {Cantidad} registros", resultado.Datos?.Count() ?? 0);

        return StatusCode(resultado.Estado, resultado);
    }

    /// <summary>Obtiene los repuestos con stock por debajo del mínimo.</summary>
    /// <response code="200">Lista de repuestos con stock bajo.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
    [HttpGet("below-minimum")]
    [ProducesResponseType(typeof(Resultado<IEnumerable<RepuestoListaDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerBajoMinimoAsync()
    {
        _registro.LogInformation("Listando repuestos con stock bajo el mínimo");

        var resultado = await _servicio.ObtenerTodosAsync(soloBajoMinimo: true);

        _registro.LogInformation("Repuestos bajo el mínimo: {Cantidad}", resultado.Datos?.Count() ?? 0);

        return StatusCode(resultado.Estado, resultado);
    }

    /// <summary>Obtiene la lista de repuestos por páginas.</summary>
    /// <param name="paginaNumero">Número de página, empezando en 1.</param>
    /// <param name="tamanoPagina">Cantidad de registros por página (entre 1 y 50).</param>
    /// <response code="200">Página de repuestos.</response>
    /// <response code="400">La página o el tamaño no son válidos.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
    [HttpGet("{pageNumber:int}/{pageSize:int}")]
    [ProducesResponseType(typeof(Resultado<IEnumerable<RepuestoListaDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ObtenerPaginacionAsync([FromRoute(Name = "pageNumber")] int paginaNumero, [FromRoute(Name = "pageSize")] int tamanoPagina)
    {
        _registro.LogInformation("Listando repuestos página {Pagina} de tamaño {Tamano}", paginaNumero, tamanoPagina);

        var resultado = await _servicio.ObtenerPaginacionAsync(paginaNumero, tamanoPagina);

        _registro.LogInformation("Página {Pagina} de repuestos completada", paginaNumero);

        return StatusCode(resultado.Estado, resultado);
    }

    /// <summary>Obtiene el detalle de un repuesto.</summary>
    /// <remarks>Incluye los proveedores que lo ofrecen, con su precio y tiempo de entrega.</remarks>
    /// <param name="id">Id del repuesto.</param>
    /// <response code="200">Detalle del repuesto.</response>
    /// <response code="404">No existe un repuesto con el id indicado.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
    [HttpGet("{id:int}", Name = "GetSparePartById")]
    [ProducesResponseType(typeof(Resultado<RepuestoDetalleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorIdentificadorAsync(int id)
    {
        _registro.LogInformation("Consultando el repuesto {Id}", id);

        var resultado = await _servicio.ObtenerPorIdentificadorAsync(id);

        _registro.LogInformation("Repuesto {Id} consultado", id);

        return StatusCode(resultado.Estado, resultado);
    }

    /// <summary>Registra un repuesto nuevo.</summary>
    /// <remarks>El código no se puede repetir y el stock no puede ser negativo.</remarks>
    /// <response code="201">Repuesto registrado.</response>
    /// <response code="400">Los datos enviados no son válidos. La respuesta indica qué campo falló y por qué.</response>
    /// <response code="409">Ya existe un repuesto con ese código.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Resultado<RepuestoDetalleDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AgregarAsync([FromBody] RepuestoCrearDto repuesto)
    {
        _registro.LogInformation("Registrando el repuesto {Codigo}", repuesto.Codigo);

        var resultado = await _servicio.AgregarAsync(repuesto);

        _registro.LogInformation("Repuesto {Codigo} registrado con id {Id}", repuesto.Codigo, resultado.Datos!.Identificador);

        return CreatedAtRoute("GetSparePartById", new { id = resultado.Datos.Identificador }, resultado);
    }

    /// <summary>Actualiza un repuesto.</summary>
    /// <param name="id">Id del repuesto.</param>
    /// <response code="200">Repuesto actualizado.</response>
    /// <response code="400">Los datos enviados no son válidos. La respuesta indica qué campo falló y por qué.</response>
    /// <response code="404">No existe un repuesto con el id indicado.</response>
    /// <response code="409">Ya existe otro repuesto con ese código.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Resultado<RepuestoDetalleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ActualizarAsync([FromRoute] int id, [FromBody] RepuestoActualizarDto repuesto)
    {
        _registro.LogInformation("Actualizando el repuesto {Id}", id);

        var resultado = await _servicio.ActualizarAsync(id, repuesto);

        _registro.LogInformation("Repuesto {Id} actualizado", id);

        return StatusCode(resultado.Estado, resultado);
    }

    /// <summary>Asocia un proveedor a un repuesto.</summary>
    /// <remarks>Se indica el precio ofrecido y el tiempo de entrega estimado en días. El proveedor debe estar activo.</remarks>
    /// <param name="id">Id del repuesto.</param>
    /// <param name="proveedorId">Id del proveedor.</param>
    /// <response code="201">Proveedor asociado.</response>
    /// <response code="400">Los datos enviados no son válidos. La respuesta indica qué campo falló y por qué.</response>
    /// <response code="404">No existe el repuesto o el proveedor.</response>
    /// <response code="409">El proveedor ya está asociado o está inactivo.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
    [HttpPost("{id:int}/suppliers/{supplierId:int}")]
    [ProducesResponseType(typeof(Resultado<RepuestoDetalleDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AsociarProveedorAsync(int id, [FromRoute(Name = "supplierId")] int proveedorId,
        [FromBody] CondicionesProveedorRepuestoDto condiciones)
    {
        _registro.LogInformation("Asociando el proveedor {ProveedorId} al repuesto {Id}", proveedorId, id);

        var resultado = await _servicio.AsociarProveedorAsync(id, proveedorId, condiciones);

        _registro.LogInformation("Proveedor {ProveedorId} asociado al repuesto {Id}", proveedorId, id);

        return CreatedAtRoute("GetSparePartById", new { id }, resultado);
    }

    /// <summary>Actualiza el precio y el tiempo de entrega de un proveedor para un repuesto.</summary>
    /// <param name="id">Id del repuesto.</param>
    /// <param name="proveedorId">Id del proveedor.</param>
    /// <response code="200">Condiciones actualizadas.</response>
    /// <response code="400">Los datos enviados no son válidos. La respuesta indica qué campo falló y por qué.</response>
    /// <response code="404">El proveedor no está asociado a ese repuesto.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
    [HttpPut("{id:int}/suppliers/{supplierId:int}")]
    [ProducesResponseType(typeof(Resultado<RepuestoDetalleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActualizarCondicionesProveedorAsync(int id, [FromRoute(Name = "supplierId")] int proveedorId,
        [FromBody] CondicionesProveedorRepuestoDto condiciones)
    {
        _registro.LogInformation("Actualizando las condiciones del proveedor {ProveedorId} para el repuesto {Id}", proveedorId, id);

        var resultado = await _servicio.ActualizarCondicionesProveedorAsync(id, proveedorId, condiciones);

        _registro.LogInformation("Condiciones del proveedor {ProveedorId} para el repuesto {Id} actualizadas", proveedorId, id);

        return StatusCode(resultado.Estado, resultado);
    }
}
