using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Proyecto.Application.Interfaces;
using Proyecto.Domain.DTO;
using Proyecto.Domain.DTO.ResultadoPatron;

namespace Proyecto.WebAPI.Controllers;

/// <summary>Gestión de activos: registro, consulta, actualización, estado, baja lógica e imagen.</summary>
/// <remarks>
/// Las fallas de negocio no se atrapan aquí: el servicio lanza una excepción y el
/// ManejadorGlobalExcepciones responde 400/404/409 y registra el Warning con el traceId.
/// </remarks>
[Route("api/Asset")]
[ApiController]
[Produces("application/json")]
public class ActivoController : ControllerBase
{
    private readonly IActivoService _servicio;
    private readonly ILogger<ActivoController> _registro;

    public ActivoController(IActivoService servicio, ILogger<ActivoController> registro)
    {
        _servicio = servicio;
        _registro = registro;
    }

    /// <summary>Lista los activos, con filtro opcional por categoría y por estado.</summary>
    /// <remarks>Sin el filtro status no se listan los activos dados de baja (Decommissioned), igual que la baja lógica vista en clase.</remarks>
    /// <param name="categoriaId">Id de la categoría (opcional).</param>
    /// <param name="estado">Operational, UnderMaintenance, OutOfService o Decommissioned (opcional).</param>
    [HttpGet]
    [ProducesResponseType(typeof(Resultado<IEnumerable<ActivoListaDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ObtenerTodosAsync([FromQuery(Name = "categoryId")] [Range(1, int.MaxValue, ErrorMessage = "El filtro categoryId debe ser 1 o mayor.")] int? categoriaId,
        [FromQuery(Name = "status")] [RegularExpression("^(Operational|UnderMaintenance|OutOfService|Decommissioned)$", ErrorMessage = "El filtro status debe ser Operational, UnderMaintenance, OutOfService o Decommissioned.")] string? estado)
    {
        _registro.LogInformation("Listando activos (categoryId={CategoriaId}, status={Estado})", categoriaId, estado);

        var resultado = await _servicio.ObtenerTodosAsync(categoriaId, estado);

        _registro.LogInformation("Listado de activos completado: {Cantidad} registros", resultado.Datos?.Count() ?? 0);

        return StatusCode(resultado.Estado, resultado);
    }

    /// <summary>Lista los activos por páginas, con los mismos filtros opcionales.</summary>
    [HttpGet("{pageNumber:int}/{pageSize:int}")]
    [ProducesResponseType(typeof(Resultado<IEnumerable<ActivoListaDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ObtenerPaginacionAsync([FromRoute(Name = "pageNumber")] int paginaNumero, [FromRoute(Name = "pageSize")] int tamanoPagina,
        [FromQuery(Name = "categoryId")] [Range(1, int.MaxValue, ErrorMessage = "El filtro categoryId debe ser 1 o mayor.")] int? categoriaId,
        [FromQuery(Name = "status")] [RegularExpression("^(Operational|UnderMaintenance|OutOfService|Decommissioned)$", ErrorMessage = "El filtro status debe ser Operational, UnderMaintenance, OutOfService o Decommissioned.")] string? estado)
    {
        _registro.LogInformation("Listando activos página {Pagina} de tamaño {Tamano}", paginaNumero, tamanoPagina);

        var resultado = await _servicio.ObtenerPaginacionAsync(paginaNumero, tamanoPagina, categoriaId, estado);

        _registro.LogInformation("Página {Pagina} de activos completada: {Total} registros en total", paginaNumero, resultado.RegistrosTotales);

        return StatusCode(resultado.Estado, resultado);
    }

    /// <summary>Detalle de un activo: categoría con su umbral preventivo, antigüedad, conteo y resumen de órdenes.</summary>
    [HttpGet("{id:int}", Name = "GetAssetById")]
    [ProducesResponseType(typeof(Resultado<ActivoDetalleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorIdentificadorAsync(int id)
    {
        _registro.LogInformation("Consultando el activo {Id}", id);

        var resultado = await _servicio.ObtenerPorIdentificadorAsync(id);

        _registro.LogInformation("Activo {Id} consultado", id);

        return StatusCode(resultado.Estado, resultado);
    }

    /// <summary>Registra un activo; el código lo genera el servidor con el prefijo de la categoría.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(Resultado<ActivoDetalleDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AgregarAsync([FromBody] ActivoCrearDto activo)
    {
        _registro.LogInformation("Registrando activo {Nombre} en la categoría {CategoriaId}", activo.Nombre, activo.CategoriaActivoId);

        var resultado = await _servicio.AgregarAsync(activo);

        _registro.LogInformation("Activo {Codigo} registrado con id {Id}", resultado.Datos!.CodigoActivo, resultado.Datos.Identificador);

        return CreatedAtRoute("GetAssetById", new { id = resultado.Datos.Identificador }, resultado);
    }

    /// <summary>Actualiza los datos de un activo.</summary>
    /// <remarks>Un activo dado de baja responde 404: para las operaciones de escritura ya no existe.</remarks>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Resultado<ActivoDetalleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ActualizarAsync([FromRoute] int id, [FromBody] ActivoActualizarDto activo)
    {
        _registro.LogInformation("Actualizando el activo {Id}", id);

        var resultado = await _servicio.ActualizarAsync(id, activo);

        _registro.LogInformation("Activo {Id} actualizado", id);

        return StatusCode(resultado.Estado, resultado);
    }

    /// <summary>Cambia el estado administrativo (Operational, OutOfService o Decommissioned).</summary>
    /// <remarks>404 si no existe o ya está dado de baja; 409 si tiene una orden de mantenimiento abierta.</remarks>
    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(typeof(Resultado<ActivoDetalleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ActualizarEstadoAsync([FromRoute] int id, [FromBody] ActualizarEstadoActivoDto estado)
    {
        _registro.LogInformation("Cambiando el estado del activo {Id} a {Estado}", id, estado.Estado);

        var resultado = await _servicio.ActualizarEstadoAsync(id, estado);

        _registro.LogInformation("Estado del activo {Id} cambiado a {Estado}", id, estado.Estado);

        return StatusCode(resultado.Estado, resultado);
    }

    /// <summary>Baja lógica: el activo pasa a Decommissioned y conserva su historial.</summary>
    /// <remarks>404 si no existe o ya fue dado de baja; 409 si tiene una orden de mantenimiento abierta.</remarks>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(Resultado<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> EliminarAsync(int id)
    {
        _registro.LogInformation("Dando de baja el activo {Id}", id);

        var resultado = await _servicio.EliminarAsync(id);

        _registro.LogInformation("Activo {Id} dado de baja", id);

        return StatusCode(resultado.Estado, resultado);
    }

    /// <summary>Carga o reemplaza la imagen del activo (jpg, jpeg, png o webp; máximo 2 MB).</summary>
    [HttpPost("{id:int}/image")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(Resultado<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> SubirImagenAsync(int id,
        [FromForm(Name = "file")] [Required(ErrorMessage = "El archivo de imagen es obligatorio.")] IFormFile archivo)
    {
        _registro.LogInformation("Subiendo imagen {Archivo} ({Bytes} bytes) para el activo {Id}", archivo.FileName, archivo.Length, id);

        await using var stream = archivo.OpenReadStream();

        var resultado = await _servicio.SubirImagenAsync(id, stream, archivo.FileName, archivo.Length);

        _registro.LogInformation("Imagen del activo {Id} guardada como {Archivo}", id, resultado.Datos);

        return StatusCode(resultado.Estado, resultado);
    }

    /// <summary>Quita la imagen propia del activo; vuelve a mostrarse la imagen por defecto.</summary>
    [HttpDelete("{id:int}/image")]
    [ProducesResponseType(typeof(Resultado<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> EliminarImagenAsync(int id)
    {
        _registro.LogInformation("Eliminando la imagen del activo {Id}", id);

        var resultado = await _servicio.EliminarImagenAsync(id);

        _registro.LogInformation("Imagen del activo {Id} eliminada", id);

        return StatusCode(resultado.Estado, resultado);
    }
}
