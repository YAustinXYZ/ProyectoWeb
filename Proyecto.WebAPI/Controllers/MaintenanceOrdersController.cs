using Microsoft.AspNetCore.Mvc;
using Proyecto.Application.DTO;
using Proyecto.Application.Interfaces;
using Proyecto.Domain.DTO.PatternResult;
using Proyecto.WebAPI.Models;

namespace Proyecto.WebAPI.Controllers;

[ApiController]
[Route("api/maintenance-orders")]
public sealed class MaintenanceOrdersController(
    IAssetManagementService service,
    ILogger<MaintenanceOrdersController> logger) : LoggedApiController<MaintenanceOrdersController>(logger)
{
    [HttpGet]
    [EndpointSummary("Lista solicitudes de mantenimiento, opcionalmente filtradas por activo.")]
    [ProducesResponseType(typeof(Result<IReadOnlyList<MaintenanceOrderSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<IReadOnlyList<MaintenanceOrderSummaryDto>>>> GetAll(
        [FromQuery] int? assetId, CancellationToken cancellationToken)
    {
        LogStarted(nameof(GetAll));
        var orders = await service.GetMaintenanceOrdersAsync(assetId, cancellationToken);
        LogCompleted(nameof(GetAll));
        return Ok(Result<IReadOnlyList<MaintenanceOrderSummaryDto>>.Success(orders));
    }

    [HttpGet("{id:int}")]
    [EndpointSummary("Consulta una solicitud con los datos del activo y los días transcurridos.")]
    [ProducesResponseType(typeof(Result<MaintenanceOrderDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<MaintenanceOrderDetailDto>>> GetById(
        int id, CancellationToken cancellationToken)
    {
        LogStarted(nameof(GetById));
        var order = await service.GetMaintenanceOrderAsync(id, cancellationToken);
        LogCompleted(nameof(GetById), id);
        return Ok(Result<MaintenanceOrderDetailDto>.Success(order));
    }

    [HttpPost]
    [EndpointSummary("Crea una solicitud de mantenimiento en estado fijo Solicitada.")]
    [ProducesResponseType(typeof(Result<MaintenanceOrderDetailDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<MaintenanceOrderDetailDto>>> Create(
        MaintenanceOrderRequest request, CancellationToken cancellationToken)
    {
        LogStarted(nameof(Create));
        var order = await service.CreateMaintenanceOrderAsync(request, cancellationToken);
        LogCompleted(nameof(Create), order.Id);
        return CreatedAtAction(nameof(GetById), new { id = order.Id },
            Result<MaintenanceOrderDetailDto>.Success(order, StatusCodes.Status201Created, "Created"));
    }
}
