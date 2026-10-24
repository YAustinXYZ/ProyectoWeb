using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Proyecto.Application.Interfaces;
using Proyecto.Domain.DTO;
using Proyecto.Domain.DTO.ResultadoPatron;

namespace Proyecto.WebAPI.Controllers;

/// <summary>Administración de los activos de TI: registro, consulta, actualización, cambio de estado, baja e imagen.</summary>
[Route("api/assets")]
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

    /// <summary>Obtiene la lista de activos.</summary>
    /// <remarks>Se puede filtrar por categoría y por estado. Si no se indica un estado, no se incluyen los activos dados de baja. Cada activo incluye el nombre de su categoría, su antigüedad en meses y la cantidad de órdenes de mantenimiento.</remarks>
    /// <param name="categoriaId">Id de la categoría (opcional).</param>
    /// <param name="estado">Estado del activo (opcional): Operational, UnderMaintenance, OutOfService o Decommissioned.</param>
    /// <response code="200">Lista de activos.</response>
    /// <response code="400">Alguno de los filtros no es válido.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
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

    /// <summary>Obtiene la lista de activos por páginas.</summary>
    /// <remarks>Acepta los mismos filtros que la lista completa.</remarks>
    /// <param name="paginaNumero">Número de página, empezando en 1.</param>
    /// <param name="tamanoPagina">Cantidad de registros por página (entre 1 y 50).</param>
    /// <param name="categoriaId">Id de la categoría (opcional).</param>
    /// <param name="estado">Estado del activo (opcional).</param>
    /// <response code="200">Página de activos con el total de registros y de páginas.</response>
    /// <response code="400">La página, el tamaño o algún filtro no son válidos.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
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

    /// <summary>Obtiene el detalle de un activo.</summary>
    /// <remarks>Incluye todos sus datos, la categoría con su umbral de mantenimiento preventivo, la antigüedad, las horas desde el último mantenimiento, si ya requiere mantenimiento preventivo y el resumen de sus órdenes.</remarks>
    /// <param name="id">Id del activo.</param>
    /// <response code="200">Detalle del activo.</response>
    /// <response code="404">No existe un activo con el id indicado.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
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

    /// <summary>Registra un activo nuevo.</summary>
    /// <remarks>El código del activo lo asigna el sistema con el prefijo de la categoría y un número consecutivo (por ejemplo, SRV-0003). El número de serie no se puede repetir.</remarks>
    /// <response code="201">Activo registrado.</response>
    /// <response code="400">Los datos enviados no son válidos. La respuesta indica qué campo falló y por qué.</response>
    /// <response code="409">Ya existe un activo con ese número de serie.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
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
    /// <remarks>No se puede actualizar un activo dado de baja.</remarks>
    /// <param name="id">Id del activo.</param>
    /// <response code="200">Activo actualizado.</response>
    /// <response code="400">Los datos enviados no son válidos. La respuesta indica qué campo falló y por qué.</response>
    /// <response code="404">No existe un activo con ese id o está dado de baja.</response>
    /// <response code="409">Ya existe otro activo con ese número de serie.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
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

    /// <summary>Cambia el estado de un activo.</summary>
    /// <remarks>Estados permitidos: Operational, OutOfService (fuera de servicio) y Decommissioned (dado de baja). No se puede cambiar el estado mientras el activo tenga una orden de mantenimiento abierta.</remarks>
    /// <param name="id">Id del activo.</param>
    /// <response code="200">Estado actualizado.</response>
    /// <response code="400">El estado indicado no es válido.</response>
    /// <response code="404">No existe un activo con ese id o ya está dado de baja.</response>
    /// <response code="409">El activo tiene una orden de mantenimiento abierta.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
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

    /// <summary>Da de baja un activo.</summary>
    /// <remarks>Es una baja lógica: el activo pasa al estado Decommissioned y se conserva su historial; no se borra de la base de datos. No se permite si tiene una orden de mantenimiento abierta.</remarks>
    /// <param name="id">Id del activo.</param>
    /// <response code="200">Activo dado de baja.</response>
    /// <response code="404">No existe un activo con ese id o ya está dado de baja.</response>
    /// <response code="409">El activo tiene una orden de mantenimiento abierta.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
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

    /// <summary>Carga o reemplaza la imagen de un activo.</summary>
    /// <remarks>Se envía como multipart/form-data en el campo file. Formatos permitidos: jpg, jpeg, png y webp, de hasta 2 MB.</remarks>
    /// <param name="id">Id del activo.</param>
    /// <param name="archivo">Archivo de imagen.</param>
    /// <response code="200">Imagen guardada; devuelve el nombre del archivo.</response>
    /// <response code="400">El archivo está vacío, supera 2 MB o no tiene un formato permitido.</response>
    /// <response code="404">No existe un activo con ese id o está dado de baja.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
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

    /// <summary>Elimina la imagen de un activo.</summary>
    /// <remarks>Después de eliminarla, el activo muestra la imagen predeterminada.</remarks>
    /// <param name="id">Id del activo.</param>
    /// <response code="200">Imagen eliminada.</response>
    /// <response code="404">No existe el activo, está dado de baja o no tiene imagen propia.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
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
