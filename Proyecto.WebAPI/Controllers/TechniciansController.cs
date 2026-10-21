using Microsoft.AspNetCore.Mvc;
using Proyecto.Application.DTO;
using Proyecto.Application.Interfaces;
using Proyecto.Domain.DTO.PatternResult;
using Proyecto.WebAPI.Models;

namespace Proyecto.WebAPI.Controllers;

[ApiController]
[Route("api/technicians")]
public sealed class TechniciansController(
    IAssetManagementService service,
    ILogger<TechniciansController> logger) : LoggedApiController<TechniciansController>(logger)
{
    [HttpGet]
    [EndpointSummary("Lista técnicos con filtros opcionales de especialidad y disponibilidad.")]
    [ProducesResponseType(typeof(Result<IReadOnlyList<TechnicianSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<IReadOnlyList<TechnicianSummaryDto>>>> GetAll(
        [FromQuery] int? specialtyId, [FromQuery] bool? isAvailable, CancellationToken cancellationToken)
    {
        LogStarted(nameof(GetAll));
        var technicians = await service.GetTechniciansAsync(specialtyId, isAvailable, cancellationToken);
        LogCompleted(nameof(GetAll));
        return Ok(Result<IReadOnlyList<TechnicianSummaryDto>>.Success(technicians));
    }

    [HttpGet("{id:int}")]
    [EndpointSummary("Consulta un técnico con sus especialidades y antigüedad de certificación.")]
    [ProducesResponseType(typeof(Result<TechnicianDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<TechnicianDetailDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        LogStarted(nameof(GetById));
        var technician = await service.GetTechnicianAsync(id, cancellationToken);
        LogCompleted(nameof(GetById), id);
        return Ok(Result<TechnicianDetailDto>.Success(technician));
    }

    [HttpPost]
    [EndpointSummary("Registra un técnico con identificación única y tarifa por hora.")]
    [ProducesResponseType(typeof(Result<TechnicianDetailDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<TechnicianDetailDto>>> Create(
        TechnicianWriteRequest request, CancellationToken cancellationToken)
    {
        LogStarted(nameof(Create));
        var technician = await service.CreateTechnicianAsync(request, cancellationToken);
        LogCompleted(nameof(Create), technician.Id);
        return CreatedAtAction(nameof(GetById), new { id = technician.Id },
            Result<TechnicianDetailDto>.Success(technician, StatusCodes.Status201Created, "Created"));
    }

    [HttpPut("{id:int}")]
    [EndpointSummary("Actualiza los datos de contacto, disponibilidad o tarifa de un técnico.")]
    [ProducesResponseType(typeof(Result<TechnicianDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<TechnicianDetailDto>>> Update(
        int id, TechnicianWriteRequest request, CancellationToken cancellationToken)
    {
        LogStarted(nameof(Update));
        var technician = await service.UpdateTechnicianAsync(id, request, cancellationToken);
        LogCompleted(nameof(Update), id);
        return Ok(Result<TechnicianDetailDto>.Success(technician));
    }

    [HttpPost("{id:int}/specialties/{specialtyId:int}")]
    [EndpointSummary("Asocia una especialidad y su certificación a un técnico.")]
    [ProducesResponseType(typeof(Result<TechnicianDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<TechnicianDetailDto>>> AddSpecialty(
        int id, int specialtyId, TechnicianSpecialtyRequest request, CancellationToken cancellationToken)
    {
        LogStarted(nameof(AddSpecialty));
        var technician = await service.AddSpecialtyAsync(id, specialtyId, request, cancellationToken);
        LogCompleted(nameof(AddSpecialty), id);
        return Ok(Result<TechnicianDetailDto>.Success(technician));
    }

    [HttpDelete("{id:int}/specialties/{specialtyId:int}")]
    [EndpointSummary("Desasocia una especialidad de un técnico.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveSpecialty(int id, int specialtyId, CancellationToken cancellationToken)
    {
        LogStarted(nameof(RemoveSpecialty));
        await service.RemoveSpecialtyAsync(id, specialtyId, cancellationToken);
        LogCompleted(nameof(RemoveSpecialty), id);
        return NoContent();
    }
}
