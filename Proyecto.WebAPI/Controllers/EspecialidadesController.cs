using Microsoft.AspNetCore.Mvc;
using Proyecto.Application.Interfaces;
using Proyecto.Domain.DTO;
using Proyecto.Domain.DTO.ResultadoPatron;

namespace Proyecto.WebAPI.Controllers;

/// <summary>Administración de las especialidades técnicas.</summary>
[Route("api/specialties")]
[ApiController]
[Produces("application/json")]
public class EspecialidadesController : ControllerBase
{
    private readonly IEspecialidadService _servicio;
    private readonly ILogger<EspecialidadesController> _registro;

    public EspecialidadesController(IEspecialidadService servicio, ILogger<EspecialidadesController> registro)
    {
        _servicio = servicio;
        _registro = registro;
    }

    /// <summary>Obtiene la lista de especialidades.</summary>
    /// <remarks>Cada especialidad incluye la cantidad de técnicos que la tienen.</remarks>
    /// <response code="200">Lista de especialidades.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
    [HttpGet]
    [ProducesResponseType(typeof(Resultado<IEnumerable<EspecialidadListaDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerTodosAsync()
    {
        _registro.LogInformation("Listando especialidades");

        var resultado = await _servicio.ObtenerTodosAsync();

        _registro.LogInformation("Listado de especialidades completado: {Cantidad} registros", resultado.Datos?.Count() ?? 0);

        return StatusCode(resultado.Estado, resultado);
    }

    /// <summary>Obtiene el detalle de una especialidad.</summary>
    /// <remarks>Incluye los técnicos que la tienen, con su nivel de certificación y disponibilidad.</remarks>
    /// <param name="id">Id de la especialidad.</param>
    /// <response code="200">Detalle de la especialidad.</response>
    /// <response code="404">No existe una especialidad con el id indicado.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
    [HttpGet("{id:int}", Name = "GetSpecialtyById")]
    [ProducesResponseType(typeof(Resultado<EspecialidadDetalleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorIdentificadorAsync(int id)
    {
        _registro.LogInformation("Consultando la especialidad {Id}", id);

        var resultado = await _servicio.ObtenerPorIdentificadorAsync(id);

        _registro.LogInformation("Especialidad {Id} consultada", id);

        return StatusCode(resultado.Estado, resultado);
    }

    /// <summary>Registra una especialidad nueva.</summary>
    /// <remarks>El nombre no se puede repetir.</remarks>
    /// <response code="201">Especialidad registrada.</response>
    /// <response code="400">Los datos enviados no son válidos. La respuesta indica qué campo falló y por qué.</response>
    /// <response code="409">Ya existe una especialidad con ese nombre.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Resultado<EspecialidadDetalleDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AgregarAsync([FromBody] EspecialidadCrearDto especialidad)
    {
        _registro.LogInformation("Registrando la especialidad {Nombre}", especialidad.Nombre);

        var resultado = await _servicio.AgregarAsync(especialidad);

        _registro.LogInformation("Especialidad registrada con id {Id}", resultado.Datos!.Identificador);

        return CreatedAtRoute("GetSpecialtyById", new { id = resultado.Datos.Identificador }, resultado);
    }

    /// <summary>Actualiza una especialidad.</summary>
    /// <param name="id">Id de la especialidad.</param>
    /// <response code="200">Especialidad actualizada.</response>
    /// <response code="400">Los datos enviados no son válidos. La respuesta indica qué campo falló y por qué.</response>
    /// <response code="404">No existe una especialidad con el id indicado.</response>
    /// <response code="409">Ya existe otra especialidad con ese nombre.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Resultado<EspecialidadDetalleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ActualizarAsync([FromRoute] int id, [FromBody] EspecialidadActualizarDto especialidad)
    {
        _registro.LogInformation("Actualizando la especialidad {Id}", id);

        var resultado = await _servicio.ActualizarAsync(id, especialidad);

        _registro.LogInformation("Especialidad {Id} actualizada", id);

        return StatusCode(resultado.Estado, resultado);
    }
}
