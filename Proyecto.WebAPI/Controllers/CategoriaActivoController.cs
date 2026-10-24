using Microsoft.AspNetCore.Mvc;
using Proyecto.Application.Interfaces;
using Proyecto.Domain.DTO;
using Proyecto.Domain.DTO.ResultadoPatron;

namespace Proyecto.WebAPI.Controllers;

/// <summary>Administración de las categorías de activos y de su umbral de mantenimiento preventivo.</summary>
[Route("api/asset-categories")]
[ApiController]
[Produces("application/json")]
public class CategoriaActivoController : ControllerBase
{
    private readonly ICategoriaActivoService _servicio;
    private readonly ILogger<CategoriaActivoController> _registro;

    public CategoriaActivoController(ICategoriaActivoService servicio, ILogger<CategoriaActivoController> registro)
    {
        _servicio = servicio;
        _registro = registro;
    }

    /// <summary>Obtiene la lista de categorías.</summary>
    /// <remarks>Cada categoría incluye la cantidad de activos asociados.</remarks>
    /// <response code="200">Lista de categorías.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
    [HttpGet]
    [ProducesResponseType(typeof(Resultado<IEnumerable<CategoriaActivoListaDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerTodosAsync()
    {
        _registro.LogInformation("Listando categorías");

        var resultado = await _servicio.ObtenerTodosAsync();

        _registro.LogInformation("Listado de categorías completado: {Cantidad} registros", resultado.Datos?.Count() ?? 0);

        return StatusCode(resultado.Estado, resultado);
    }

    /// <summary>Obtiene la lista de categorías por páginas.</summary>
    /// <param name="paginaNumero">Número de página, empezando en 1.</param>
    /// <param name="tamanoPagina">Cantidad de registros por página (entre 1 y 50).</param>
    /// <response code="200">Página de categorías.</response>
    /// <response code="400">La página o el tamaño no son válidos.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
    [HttpGet("{pageNumber:int}/{pageSize:int}")]
    [ProducesResponseType(typeof(Resultado<IEnumerable<CategoriaActivoListaDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ObtenerPaginacionAsync([FromRoute(Name = "pageNumber")] int paginaNumero, [FromRoute(Name = "pageSize")] int tamanoPagina)
    {
        _registro.LogInformation("Listando categorías página {Pagina} de tamaño {Tamano}", paginaNumero, tamanoPagina);

        var resultado = await _servicio.ObtenerPaginacionAsync(paginaNumero, tamanoPagina);

        _registro.LogInformation("Página {Pagina} de categorías completada", paginaNumero);

        return StatusCode(resultado.Estado, resultado);
    }

    /// <summary>Obtiene el detalle de una categoría.</summary>
    /// <remarks>Incluye su umbral de mantenimiento preventivo, la cantidad de activos y el listado de esos activos (código, nombre y estado).</remarks>
    /// <param name="id">Id de la categoría.</param>
    /// <response code="200">Detalle de la categoría.</response>
    /// <response code="404">No existe una categoría con el id indicado.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
    [HttpGet("{id:int}", Name = "GetAssetCategoryById")]
    [ProducesResponseType(typeof(Resultado<CategoriaActivoDetalleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorIdentificadorAsync(int id)
    {
        _registro.LogInformation("Consultando la categoría {Id}", id);

        var resultado = await _servicio.ObtenerPorIdentificadorAsync(id);

        _registro.LogInformation("Categoría {Id} consultada", id);

        return StatusCode(resultado.Estado, resultado);
    }

    /// <summary>Registra una categoría nueva.</summary>
    /// <remarks>El nombre y el prefijo de código no se pueden repetir.</remarks>
    /// <response code="201">Categoría registrada.</response>
    /// <response code="400">Los datos enviados no son válidos. La respuesta indica qué campo falló y por qué.</response>
    /// <response code="409">Ya existe una categoría con ese nombre o prefijo.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Resultado<CategoriaActivoDetalleDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AgregarAsync([FromBody] CategoriaActivoCrearDto categoria)
    {
        _registro.LogInformation("Registrando la categoría {Nombre}", categoria.Nombre);

        var resultado = await _servicio.AgregarAsync(categoria);

        _registro.LogInformation("Categoría registrada con id {Id}", resultado.Datos!.Identificador);

        return CreatedAtRoute("GetAssetCategoryById", new { id = resultado.Datos.Identificador }, resultado);
    }

    /// <summary>Actualiza una categoría.</summary>
    /// <param name="id">Id de la categoría.</param>
    /// <response code="200">Categoría actualizada.</response>
    /// <response code="400">Los datos enviados no son válidos. La respuesta indica qué campo falló y por qué.</response>
    /// <response code="404">No existe una categoría con el id indicado.</response>
    /// <response code="409">Ya existe otra categoría con ese nombre o prefijo.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Resultado<CategoriaActivoDetalleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ActualizarAsync([FromRoute] int id, [FromBody] CategoriaActivoActualizarDto categoria)
    {
        _registro.LogInformation("Actualizando la categoría {Id}", id);

        var resultado = await _servicio.ActualizarAsync(id, categoria);

        _registro.LogInformation("Categoría {Id} actualizada", id);

        return StatusCode(resultado.Estado, resultado);
    }

    /// <summary>Elimina una categoría.</summary>
    /// <remarks>Solo se puede eliminar si no tiene activos asociados.</remarks>
    /// <param name="id">Id de la categoría.</param>
    /// <response code="200">Categoría eliminada.</response>
    /// <response code="404">No existe una categoría con el id indicado.</response>
    /// <response code="409">La categoría tiene activos asociados.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(Resultado<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> EliminarAsync(int id)
    {
        _registro.LogInformation("Eliminando la categoría {Id}", id);

        var resultado = await _servicio.EliminarAsync(id);

        _registro.LogInformation("Categoría {Id} eliminada", id);

        return StatusCode(resultado.Estado, resultado);
    }
}
