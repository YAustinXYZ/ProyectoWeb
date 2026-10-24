using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Proyecto.Application.Interfaces;
using Proyecto.Domain.DTO;
using Proyecto.Domain.DTO.ResultadoPatron;

namespace Proyecto.WebAPI.Controllers;

/// <summary>Técnicos y sus especialidades (relación N:N con nivel y fecha de certificación).</summary>
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

    /// <summary>Lista los técnicos, con filtro opcional por especialidad y por disponibilidad.</summary>
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

    /// <summary>Detalle de un técnico con cada especialidad, su nivel, fecha y antigüedad en meses.</summary>
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

    /// <summary>Registra un técnico; la identificación debe ser única.</summary>
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

    /// <summary>Asocia una especialidad al técnico con su nivel y fecha de certificación.</summary>
    /// <remarks>400 si el nivel es inválido o la fecha es futura; 404 si no existe el técnico o la especialidad; 409 si ya la tiene.</remarks>
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

    /// <summary>Quita una especialidad del técnico y devuelve el técnico actualizado.</summary>
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
