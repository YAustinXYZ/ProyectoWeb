using Microsoft.AspNetCore.Mvc;
using Proyecto.Application.DTO;
using Proyecto.Application.Interfaces;
using Proyecto.Domain.DTO.PatternResult;
using Proyecto.WebAPI.Models;

namespace Proyecto.WebAPI.Controllers;

[ApiController]
[Route("api/spare-parts")]
public sealed class SparePartsController(
    IAssetManagementService service,
    ILogger<SparePartsController> logger) : LoggedApiController<SparePartsController>(logger)
{
    [HttpGet]
    [EndpointSummary("Lista repuestos con sus indicadores de inventario.")]
    [ProducesResponseType(typeof(Result<IReadOnlyList<SparePartSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<IReadOnlyList<SparePartSummaryDto>>>> GetAll(
        CancellationToken cancellationToken)
    {
        LogStarted(nameof(GetAll));
        var parts = await service.GetSparePartsAsync(false, cancellationToken);
        LogCompleted(nameof(GetAll));
        return Ok(Result<IReadOnlyList<SparePartSummaryDto>>.Success(parts));
    }

    [HttpGet("below-minimum")]
    [EndpointSummary("Lista repuestos cuyo stock actual está por debajo del mínimo.")]
    [ProducesResponseType(typeof(Result<IReadOnlyList<SparePartSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<IReadOnlyList<SparePartSummaryDto>>>> GetBelowMinimum(
        CancellationToken cancellationToken)
    {
        LogStarted(nameof(GetBelowMinimum));
        var parts = await service.GetSparePartsAsync(true, cancellationToken);
        LogCompleted(nameof(GetBelowMinimum));
        return Ok(Result<IReadOnlyList<SparePartSummaryDto>>.Success(parts));
    }

    [HttpGet("{id:int}")]
    [EndpointSummary("Consulta un repuesto con los proveedores y condiciones asociados.")]
    [ProducesResponseType(typeof(Result<SparePartDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<SparePartDetailDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        LogStarted(nameof(GetById));
        var part = await service.GetSparePartAsync(id, cancellationToken);
        LogCompleted(nameof(GetById), id);
        return Ok(Result<SparePartDetailDto>.Success(part));
    }

    [HttpPost]
    [EndpointSummary("Registra un repuesto con código REP- único y stock no negativo.")]
    [ProducesResponseType(typeof(Result<SparePartDetailDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<SparePartDetailDto>>> Create(
        SparePartWriteRequest request, CancellationToken cancellationToken)
    {
        LogStarted(nameof(Create));
        var part = await service.CreateSparePartAsync(request, cancellationToken);
        LogCompleted(nameof(Create), part.Id);
        return CreatedAtAction(nameof(GetById), new { id = part.Id },
            Result<SparePartDetailDto>.Success(part, StatusCodes.Status201Created, "Created"));
    }

    [HttpPut("{id:int}")]
    [EndpointSummary("Actualiza los datos y existencias de un repuesto.")]
    [ProducesResponseType(typeof(Result<SparePartDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<SparePartDetailDto>>> Update(
        int id, SparePartWriteRequest request, CancellationToken cancellationToken)
    {
        LogStarted(nameof(Update));
        var part = await service.UpdateSparePartAsync(id, request, cancellationToken);
        LogCompleted(nameof(Update), id);
        return Ok(Result<SparePartDetailDto>.Success(part));
    }

    [HttpPost("{partId:int}/suppliers/{supplierId:int}")]
    [EndpointSummary("Asocia un proveedor a un repuesto con precio y plazo de entrega.")]
    [ProducesResponseType(typeof(Result<SparePartDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<SparePartDetailDto>>> AddSupplier(
        int partId, int supplierId, PartSupplierRequest request, CancellationToken cancellationToken)
    {
        LogStarted(nameof(AddSupplier));
        var part = await service.AddSupplierToPartAsync(partId, supplierId, request, cancellationToken);
        LogCompleted(nameof(AddSupplier), partId);
        return Ok(Result<SparePartDetailDto>.Success(part));
    }

    [HttpPut("{partId:int}/suppliers/{supplierId:int}")]
    [EndpointSummary("Actualiza el precio o plazo de entrega de un proveedor para un repuesto.")]
    [ProducesResponseType(typeof(Result<SparePartDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<SparePartDetailDto>>> UpdateSupplierConditions(
        int partId, int supplierId, PartSupplierRequest request, CancellationToken cancellationToken)
    {
        LogStarted(nameof(UpdateSupplierConditions));
        var part = await service.UpdateSupplierConditionsAsync(partId, supplierId, request, cancellationToken);
        LogCompleted(nameof(UpdateSupplierConditions), partId);
        return Ok(Result<SparePartDetailDto>.Success(part));
    }
}
