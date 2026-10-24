using Microsoft.AspNetCore.Mvc;
using Proyecto.Application.Interfaces;
using Proyecto.Domain.DTO;
using Proyecto.Domain.DTO.ResultadoPatron;

namespace Proyecto.WebAPI.Controllers;

/// <summary>Especialidades técnicas.</summary>
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

    /// <summary>Lista las especialidades con la cantidad de técnicos que las poseen.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(Resultado<IEnumerable<EspecialidadListaDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerTodosAsync()
    {
        _registro.LogInformation("Listando especialidades");

        var resultado = await _servicio.ObtenerTodosAsync();

        _registro.LogInformation("Listado de especialidades completado: {Cantidad} registros", resultado.Datos?.Count() ?? 0);

        return StatusCode(resultado.Estado, resultado);
    }

    /// <summary>Detalle de una especialidad con los técnicos que la poseen, su nivel y disponibilidad.</summary>
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

    /// <summary>Registra una especialidad; el nombre debe ser único.</summary>
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
