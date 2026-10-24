using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Proyecto.Application.Interfaces;
using Proyecto.Domain.DTO;
using Proyecto.Domain.DTO.ResultadoPatron;

namespace Proyecto.WebAPI.Controllers;

/// <summary>Administración de los técnicos y de sus especialidades.</summary>
[Route("api/technicians")]
[ApiController]
[Produces("application/json")]
public class TecnicosController : ControllerBase
{
    private readonly ITecnicoService _servicio;
    private readonly ILogger<TecnicosController> _registro;

    public TecnicosController(ITecnicoService servicio, ILogger<TecnicosController> registro)
    {
        _servicio = servicio;
        _registro = registro;
    }

    /// <summary>Obtiene la lista de técnicos.</summary>
    /// <remarks>Se puede filtrar por especialidad y por disponibilidad. Cada técnico incluye la cantidad de especialidades que tiene.</remarks>
    /// <param name="especialidadId">Id de la especialidad (opcional).</param>
    /// <param name="estaDisponible">true para disponibles, false para no disponibles (opcional).</param>
    /// <response code="200">Lista de técnicos.</response>
    /// <response code="400">Alguno de los filtros no es válido.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
    [HttpGet]
    [ProducesResponseType(typeof(Resultado<IEnumerable<TecnicoListaDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ObtenerTodosAsync(
        [FromQuery(Name = "specialtyId")] [Range(1, int.MaxValue, ErrorMessage = "El filtro specialtyId debe ser 1 o mayor.")] int? especialidadId,
        [FromQuery(Name = "isAvailable")] bool? estaDisponible)
    {
        _registro.LogInformation("Listando técnicos (specialtyId={EspecialidadId}, isAvailable={Disponible})", especialidadId, estaDisponible);

        var resultado = await _servicio.ObtenerTodosAsync(especialidadId, estaDisponible);

        _registro.LogInformation("Listado de técnicos completado: {Cantidad} registros", resultado.Datos?.Count() ?? 0);

        return StatusCode(resultado.Estado, resultado);
    }

    /// <summary>Obtiene el detalle de un técnico.</summary>
    /// <remarks>Incluye cada especialidad con su nivel de certificación, la fecha desde la que la tiene y la antigüedad en meses.</remarks>
    /// <param name="id">Id del técnico.</param>
    /// <response code="200">Detalle del técnico.</response>
    /// <response code="404">No existe un técnico con el id indicado.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
    [HttpGet("{id:int}", Name = "GetTechnicianById")]
    [ProducesResponseType(typeof(Resultado<TecnicoDetalleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorIdentificadorAsync(int id)
    {
        _registro.LogInformation("Consultando el técnico {Id}", id);

        var resultado = await _servicio.ObtenerPorIdentificadorAsync(id);

        _registro.LogInformation("Técnico {Id} consultado", id);

        return StatusCode(resultado.Estado, resultado);
    }

    /// <summary>Registra un técnico nuevo.</summary>
    /// <remarks>La identificación no se puede repetir.</remarks>
    /// <response code="201">Técnico registrado.</response>
    /// <response code="400">Los datos enviados no son válidos. La respuesta indica qué campo falló y por qué.</response>
    /// <response code="409">Ya existe un técnico con esa identificación.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Resultado<TecnicoDetalleDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AgregarAsync([FromBody] TecnicoCrearDto tecnico)
    {
        _registro.LogInformation("Registrando el técnico {Nombre}", tecnico.NombreCompleto);

        var resultado = await _servicio.AgregarAsync(tecnico);

        _registro.LogInformation("Técnico registrado con id {Id}", resultado.Datos!.Identificador);

        return CreatedAtRoute("GetTechnicianById", new { id = resultado.Datos.Identificador }, resultado);
    }

    /// <summary>Actualiza un técnico.</summary>
    /// <param name="id">Id del técnico.</param>
    /// <response code="200">Técnico actualizado.</response>
    /// <response code="400">Los datos enviados no son válidos. La respuesta indica qué campo falló y por qué.</response>
    /// <response code="404">No existe un técnico con el id indicado.</response>
    /// <response code="409">Ya existe otro técnico con esa identificación.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Resultado<TecnicoDetalleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ActualizarAsync([FromRoute] int id, [FromBody] TecnicoActualizarDto tecnico)
    {
        _registro.LogInformation("Actualizando el técnico {Id}", id);

        var resultado = await _servicio.ActualizarAsync(id, tecnico);

        _registro.LogInformation("Técnico {Id} actualizado", id);

        return StatusCode(resultado.Estado, resultado);
    }

    /// <summary>Asocia una especialidad a un técnico.</summary>
    /// <remarks>Se indica el nivel de certificación (Basic, Intermediate o Advanced) y la fecha desde la que la tiene, que no puede ser futura.</remarks>
    /// <param name="id">Id del técnico.</param>
    /// <param name="especialidadId">Id de la especialidad.</param>
    /// <response code="201">Especialidad asociada; devuelve el técnico actualizado.</response>
    /// <response code="400">Los datos enviados no son válidos. La respuesta indica qué campo falló y por qué.</response>
    /// <response code="404">No existe el técnico o la especialidad.</response>
    /// <response code="409">El técnico ya tiene esa especialidad.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
    [HttpPost("{id:int}/specialties/{specialtyId:int}")]
    [ProducesResponseType(typeof(Resultado<TecnicoDetalleDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AsociarEspecialidadAsync(int id, [FromRoute(Name = "specialtyId")] int especialidadId,
        [FromBody] SolicitudEspecialidadTecnicoDto solicitud)
    {
        _registro.LogInformation("Asociando la especialidad {EspecialidadId} al técnico {Id}", especialidadId, id);

        var resultado = await _servicio.AsociarEspecialidadAsync(id, especialidadId, solicitud);

        _registro.LogInformation("Especialidad {EspecialidadId} asociada al técnico {Id}", especialidadId, id);

        return CreatedAtRoute("GetTechnicianById", new { id }, resultado);
    }

    /// <summary>Quita una especialidad de un técnico.</summary>
    /// <param name="id">Id del técnico.</param>
    /// <param name="especialidadId">Id de la especialidad.</param>
    /// <response code="200">Especialidad quitada; devuelve el técnico actualizado.</response>
    /// <response code="404">No existe el técnico o no tiene esa especialidad.</response>
    /// <response code="500">Error inesperado del servidor. La respuesta incluye el traceId para buscarlo en el log.</response>
    [HttpDelete("{id:int}/specialties/{specialtyId:int}")]
    [ProducesResponseType(typeof(Resultado<TecnicoDetalleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Resultado<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DesasociarEspecialidadAsync(int id, [FromRoute(Name = "specialtyId")] int especialidadId)
    {
        _registro.LogInformation("Quitando la especialidad {EspecialidadId} del técnico {Id}", especialidadId, id);

        var resultado = await _servicio.DesasociarEspecialidadAsync(id, especialidadId);

        _registro.LogInformation("Especialidad {EspecialidadId} quitada del técnico {Id}", especialidadId, id);

        return StatusCode(resultado.Estado, resultado);
    }
}
