using Microsoft.AspNetCore.Mvc;
using Proyecto.Application.Interfaces;
using Proyecto.Domain.DTO;
using Proyecto.Domain.DTO.ResultadoPatron;

namespace Proyecto.WebAPI.Controllers;

/// <summary>Proveedores de servicios y de repuestos.</summary>
[Route("api/Supplier")]
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

    /// <summary>Lista los proveedores.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(Resultado<IEnumerable<ProveedorListaDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerTodosAsync()
    {
        _registro.LogInformation("Listando proveedores");

        var resultado = await _servicio.ObtenerTodosAsync();

        _registro.LogInformation("Listado de proveedores completado: {Cantidad} registros", resultado.Datos?.Count() ?? 0);

        return StatusCode(resultado.Estado, resultado);
    }

    /// <summary>Lista los proveedores por páginas.</summary>
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

    /// <summary>Detalle de un proveedor con los repuestos que suministra, su precio y tiempo de entrega.</summary>
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

    /// <summary>Registra un proveedor; la cédula jurídica debe ser única.</summary>
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

    /// <summary>Elimina un proveedor sin activos, órdenes ni repuestos asociados (409 si los tiene).</summary>
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
