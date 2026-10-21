using Microsoft.AspNetCore.Mvc;
using Proyecto.Application.DTO;
using Proyecto.Application.Interfaces;
using Proyecto.Domain.DTO.PatternResult;
using Proyecto.WebAPI.Models;

namespace Proyecto.WebAPI.Controllers;

[ApiController]
[Route("api/specialties")]
public sealed class SpecialtiesController(
    IAssetManagementService service,
    ILogger<SpecialtiesController> logger) : LoggedApiController<SpecialtiesController>(logger)
{
    [HttpGet]
    [EndpointSummary("Lista especialidades y la cantidad de técnicos que las poseen.")]
    [ProducesResponseType(typeof(Result<IReadOnlyList<SpecialtySummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<IReadOnlyList<SpecialtySummaryDto>>>> GetAll(CancellationToken cancellationToken)
    {
        LogStarted(nameof(GetAll));
        var specialties = await service.GetSpecialtiesAsync(cancellationToken);
        LogCompleted(nameof(GetAll));
        return Ok(Result<IReadOnlyList<SpecialtySummaryDto>>.Success(specialties));
    }

    [HttpGet("{id:int}")]
    [EndpointSummary("Consulta una especialidad con los técnicos que la poseen.")]
    [ProducesResponseType(typeof(Result<SpecialtyDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<SpecialtyDetailDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        LogStarted(nameof(GetById));
        var specialty = await service.GetSpecialtyAsync(id, cancellationToken);
        LogCompleted(nameof(GetById), id);
        return Ok(Result<SpecialtyDetailDto>.Success(specialty));
    }

    [HttpPost]
    [EndpointSummary("Registra una especialidad nueva.")]
    [ProducesResponseType(typeof(Result<SpecialtyDetailDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<SpecialtyDetailDto>>> Create(
        SpecialtyWriteRequest request, CancellationToken cancellationToken)
    {
        LogStarted(nameof(Create));
        var specialty = await service.CreateSpecialtyAsync(request, cancellationToken);
        LogCompleted(nameof(Create), specialty.Id);
        return CreatedAtAction(nameof(GetById), new { id = specialty.Id },
            Result<SpecialtyDetailDto>.Success(specialty, StatusCodes.Status201Created, "Created"));
    }

    [HttpPut("{id:int}")]
    [EndpointSummary("Actualiza el nombre o la descripción de una especialidad.")]
    [ProducesResponseType(typeof(Result<SpecialtyDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<SpecialtyDetailDto>>> Update(
        int id, SpecialtyWriteRequest request, CancellationToken cancellationToken)
    {
        LogStarted(nameof(Update));
        var specialty = await service.UpdateSpecialtyAsync(id, request, cancellationToken);
        LogCompleted(nameof(Update), id);
        return Ok(Result<SpecialtyDetailDto>.Success(specialty));
    }
}
