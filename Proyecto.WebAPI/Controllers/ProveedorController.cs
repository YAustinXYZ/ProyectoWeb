using Microsoft.AspNetCore.Mvc;
using Proyecto.Application.Interfaces;
using Proyecto.Domain.DTO;
using Proyecto.Domain.DTO.ResultadoPatron;

namespace Proyecto.WebAPI.Controllers;

/// <summary>Administración de los proveedores de servicios y de repuestos.</summary>
[Route("api/suppliers")]
[ApiController]
[Produces("application/json")]
public class ProveedorController : ControllerBase
{
    private readonly IProveedorService _servicio;
    private readonly ILogger<ProveedorController> _registro;

    public ProveedorController(IProveedorService servicio, ILogger<ProveedorController> registro)
    {
        _servicio = servicio;
        _registro = registro;
    }

    /// <summary>Obtiene la lista de proveedores.</summary>
    /// <response code="200">Lista de proveedores.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
    [HttpGet]
    [ProducesResponseType(typeof(Resultado<IEnumerable<ProveedorListaDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerTodosAsync()
    {
        _registro.LogInformation("Listando proveedores");

        var resultado = await _servicio.ObtenerTodosAsync();

        _registro.LogInformation("Listado de proveedores completado: {Cantidad} registros", resultado.Datos?.Count() ?? 0);

        return StatusCode(resultado.Estado, resultado);
    }

    /// <summary>Obtiene la lista de proveedores por páginas.</summary>
    /// <param name="paginaNumero">Número de página, empezando en 1.</param>
    /// <param name="tamanoPagina">Cantidad de registros por página (entre 1 y 50).</param>
    /// <response code="200">Página de proveedores.</response>
    /// <response code="400">La página o el tamaño no son válidos.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
    [HttpGet("{pageNumber:int}/{pageSize:int}")]
    [ProducesResponseType(typeof(Resultado<IEnumerable<ProveedorListaDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ObtenerPaginacionAsync([FromRoute(Name = "pageNumber")] int paginaNumero, [FromRoute(Name = "pageSize")] int tamanoPagina)
    {
        _registro.LogInformation("Listando proveedores página {Pagina} de tamaño {Tamano}", paginaNumero, tamanoPagina);

        var resultado = await _servicio.ObtenerPaginacionAsync(paginaNumero, tamanoPagina);

        _registro.LogInformation("Página {Pagina} de proveedores completada", paginaNumero);

        return StatusCode(resultado.Estado, resultado);
    }

    /// <summary>Obtiene el detalle de un proveedor.</summary>
    /// <remarks>Incluye los repuestos que suministra, con su precio ofrecido y tiempo de entrega.</remarks>
    /// <param name="id">Id del proveedor.</param>
    /// <response code="200">Detalle del proveedor.</response>
    /// <response code="404">No existe un proveedor con el id indicado.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
    [HttpGet("{id:int}", Name = "GetSupplierById")]
    [ProducesResponseType(typeof(Resultado<ProveedorDetalleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorIdentificadorAsync(int id)
    {
        _registro.LogInformation("Consultando el proveedor {Id}", id);

        var resultado = await _servicio.ObtenerPorIdentificadorAsync(id);

        _registro.LogInformation("Proveedor {Id} consultado", id);

        return StatusCode(resultado.Estado, resultado);
    }

    /// <summary>Registra un proveedor nuevo.</summary>
    /// <remarks>La cédula jurídica no se puede repetir.</remarks>
    /// <response code="201">Proveedor registrado.</response>
    /// <response code="400">Los datos enviados no son válidos. La respuesta indica qué campo falló y por qué.</response>
    /// <response code="409">Ya existe un proveedor con esa cédula jurídica.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Resultado<ProveedorDetalleDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AgregarAsync([FromBody] ProveedorCrearDto proveedor)
    {
        _registro.LogInformation("Registrando el proveedor {Nombre}", proveedor.Nombre);

        var resultado = await _servicio.AgregarAsync(proveedor);

        _registro.LogInformation("Proveedor registrado con id {Id}", resultado.Datos!.Identificador);

        return CreatedAtRoute("GetSupplierById", new { id = resultado.Datos.Identificador }, resultado);
    }

    /// <summary>Actualiza un proveedor.</summary>
    /// <param name="id">Id del proveedor.</param>
    /// <response code="200">Proveedor actualizado.</response>
    /// <response code="400">Los datos enviados no son válidos. La respuesta indica qué campo falló y por qué.</response>
    /// <response code="404">No existe un proveedor con el id indicado.</response>
    /// <response code="409">Ya existe otro proveedor con esa cédula jurídica.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Resultado<ProveedorDetalleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ActualizarAsync([FromRoute] int id, [FromBody] ProveedorActualizarDto proveedor)
    {
        _registro.LogInformation("Actualizando el proveedor {Id}", id);

        var resultado = await _servicio.ActualizarAsync(id, proveedor);

        _registro.LogInformation("Proveedor {Id} actualizado", id);

        return StatusCode(resultado.Estado, resultado);
    }

    /// <summary>Elimina un proveedor.</summary>
    /// <remarks>Solo se puede eliminar si no tiene activos, órdenes ni repuestos asociados.</remarks>
    /// <param name="id">Id del proveedor.</param>
    /// <response code="200">Proveedor eliminado.</response>
    /// <response code="404">No existe un proveedor con el id indicado.</response>
    /// <response code="409">El proveedor tiene registros asociados.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(Resultado<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> EliminarAsync(int id)
    {
        _registro.LogInformation("Eliminando el proveedor {Id}", id);

        var resultado = await _servicio.EliminarAsync(id);

        _registro.LogInformation("Proveedor {Id} eliminado", id);

        return StatusCode(resultado.Estado, resultado);
    }
}
