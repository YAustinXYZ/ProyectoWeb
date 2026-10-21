using Microsoft.AspNetCore.Mvc;
using Proyecto.Application.DTO;
using Proyecto.Application.Interfaces;
using Proyecto.Domain.DTO.PatternResult;
using Proyecto.WebAPI.Models;

namespace Proyecto.WebAPI.Controllers;

[ApiController]
[Route("api")]
public sealed class CatalogsController(
    IAssetManagementService service,
    ILogger<CatalogsController> logger) : LoggedApiController<CatalogsController>(logger)
{
    [HttpGet("locations")]
    [EndpointSummary("Lista ubicaciones activas para asignar activos.")]
    [ProducesResponseType(typeof(Result<IReadOnlyList<LookupDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<IReadOnlyList<LookupDto>>>> GetLocations(CancellationToken cancellationToken)
    {
        LogStarted(nameof(GetLocations));
        var locations = await service.GetLocationsAsync(cancellationToken);
        LogCompleted(nameof(GetLocations));
        return Ok(Result<IReadOnlyList<LookupDto>>.Success(locations));
    }

    [HttpGet("maintenance-types")]
    [EndpointSummary("Lista tipos de mantenimiento disponibles para las solicitudes.")]
    [ProducesResponseType(typeof(Result<IReadOnlyList<LookupDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<IReadOnlyList<LookupDto>>>> GetMaintenanceTypes(CancellationToken cancellationToken)
    {
        LogStarted(nameof(GetMaintenanceTypes));
        var types = await service.GetMaintenanceTypesAsync(cancellationToken);
        LogCompleted(nameof(GetMaintenanceTypes));
        return Ok(Result<IReadOnlyList<LookupDto>>.Success(types));
    }
}
