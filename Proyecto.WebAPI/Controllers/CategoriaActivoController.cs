using Microsoft.AspNetCore.Mvc;
using Proyecto.Application.Interfaces;
using Proyecto.Domain.DTO;
using Proyecto.Domain.DTO.ResultadoPatron;

namespace Proyecto.WebAPI.Controllers;

/// <summary>Categorías de activos y su umbral de mantenimiento preventivo por horas de uso.</summary>
[Route("api/AssetCategory")]
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

    /// <summary>Lista las categorías con la cantidad de activos de cada una.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(Resultado<IEnumerable<CategoriaActivoListaDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerTodosAsync()
    {
        _registro.LogInformation("Listando categorías");

        var resultado = await _servicio.ObtenerTodosAsync();

        _registro.LogInformation("Listado de categorías completado: {Cantidad} registros", resultado.Datos?.Count() ?? 0);

        return StatusCode(resultado.Estado, resultado);
    }

    /// <summary>Lista las categorías por páginas.</summary>
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

    /// <summary>Detalle de una categoría con el listado resumido de sus activos y su conteo.</summary>
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

    /// <summary>Registra una categoría; el nombre y el prefijo de código deben ser únicos.</summary>
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

    /// <summary>Elimina una categoría sin activos asociados (409 si los tiene).</summary>
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
