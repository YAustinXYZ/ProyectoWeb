using Microsoft.AspNetCore.Mvc;
using Proyecto.Application.DTO;
using Proyecto.Application.Interfaces;
using Proyecto.Domain.DTO.PatternResult;
using Proyecto.WebAPI.Models;

namespace Proyecto.WebAPI.Controllers;

[ApiController]
[Route("api/suppliers")]
public sealed class SuppliersController(
    IAssetManagementService service,
    ILogger<SuppliersController> logger) : LoggedApiController<SuppliersController>(logger)
{
    [HttpGet]
    [EndpointSummary("Lista proveedores activos.")]
    [ProducesResponseType(typeof(Result<IReadOnlyList<SupplierSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<IReadOnlyList<SupplierSummaryDto>>>> GetAll(CancellationToken cancellationToken)
    {
        LogStarted(nameof(GetAll));
        var suppliers = await service.GetSuppliersAsync(cancellationToken);
        LogCompleted(nameof(GetAll));
        return Ok(Result<IReadOnlyList<SupplierSummaryDto>>.Success(suppliers));
    }

    [HttpGet("{id:int}")]
    [EndpointSummary("Consulta un proveedor con los repuestos y condiciones que ofrece.")]
    [ProducesResponseType(typeof(Result<SupplierDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<SupplierDetailDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        LogStarted(nameof(GetById));
        var supplier = await service.GetSupplierAsync(id, cancellationToken);
        LogCompleted(nameof(GetById), id);
        return Ok(Result<SupplierDetailDto>.Success(supplier));
    }

    [HttpPost]
    [EndpointSummary("Registra un proveedor con identificación tributaria única.")]
    [ProducesResponseType(typeof(Result<SupplierDetailDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<SupplierDetailDto>>> Create(
        SupplierWriteRequest request, CancellationToken cancellationToken)
    {
        LogStarted(nameof(Create));
        var supplier = await service.CreateSupplierAsync(request, cancellationToken);
        LogCompleted(nameof(Create), supplier.Id);
        return CreatedAtAction(nameof(GetById), new { id = supplier.Id },
            Result<SupplierDetailDto>.Success(supplier, StatusCodes.Status201Created, "Created"));
    }

    [HttpPut("{id:int}")]
    [EndpointSummary("Actualiza los datos de contacto y condiciones generales del proveedor.")]
    [ProducesResponseType(typeof(Result<SupplierDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<SupplierDetailDto>>> Update(
        int id, SupplierWriteRequest request, CancellationToken cancellationToken)
    {
        LogStarted(nameof(Update));
        var supplier = await service.UpdateSupplierAsync(id, request, cancellationToken);
        LogCompleted(nameof(Update), id);
        return Ok(Result<SupplierDetailDto>.Success(supplier));
    }
}
