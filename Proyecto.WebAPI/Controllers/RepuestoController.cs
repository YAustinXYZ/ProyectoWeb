using Microsoft.AspNetCore.Mvc;
using Proyecto.Application.Interfaces;
using Proyecto.Domain.DTO;
using Proyecto.Domain.DTO.ResultadoPatron;

namespace Proyecto.WebAPI.Controllers;

/// <summary>Repuestos, su stock y los proveedores que los ofrecen (relación N:N con precio y tiempo de entrega).</summary>
[Route("api/SparePart")]
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

    /// <summary>Lista los repuestos con el indicador de stock bajo, el faltante y la cantidad de proveedores.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(Resultado<IEnumerable<RepuestoListaDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerTodosAsync()
    {
        _registro.LogInformation("Listando repuestos");

        var resultado = await _servicio.ObtenerTodosAsync();

        _registro.LogInformation("Listado de repuestos completado: {Cantidad} registros", resultado.Datos?.Count() ?? 0);

        return StatusCode(resultado.Estado, resultado);
    }

    /// <summary>Lista solo los repuestos con stock actual por debajo del mínimo.</summary>
    [HttpGet("below-minimum")]
    [ProducesResponseType(typeof(Resultado<IEnumerable<RepuestoListaDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerBajoMinimoAsync()
    {
        _registro.LogInformation("Listando repuestos con stock bajo el mínimo");

        var resultado = await _servicio.ObtenerTodosAsync(soloBajoMinimo: true);

        _registro.LogInformation("Repuestos bajo el mínimo: {Cantidad}", resultado.Datos?.Count() ?? 0);

        return StatusCode(resultado.Estado, resultado);
    }

    /// <summary>Lista los repuestos por páginas.</summary>
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

    /// <summary>Detalle de un repuesto con sus proveedores, precio ofrecido y tiempo de entrega.</summary>
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

    /// <summary>Registra un repuesto; el código debe ser único y el stock no puede ser negativo.</summary>
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

    /// <summary>Asocia un proveedor al repuesto con su precio y tiempo de entrega.</summary>
    /// <remarks>404 si no existe el repuesto o el proveedor; 409 si ya está asociado o el proveedor está inactivo.</remarks>
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

    /// <summary>Actualiza el precio y el tiempo de entrega de un proveedor para el repuesto.</summary>
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
