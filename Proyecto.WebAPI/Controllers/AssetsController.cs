using Microsoft.AspNetCore.Mvc;
using Proyecto.Application.DTO;
using Proyecto.Application.Interfaces;
using Proyecto.Domain.DTO.PatternResult;
using Proyecto.WebAPI.Models;

namespace Proyecto.WebAPI.Controllers;

[ApiController]
[Route("api/assets")]
public sealed class AssetsController(
    IAssetManagementService service,
    ILogger<AssetsController> logger) : LoggedApiController<AssetsController>(logger)
{
    [HttpGet]
    [EndpointSummary("Lista activos, con filtros opcionales de categoría y estado.")]
    [ProducesResponseType(typeof(Result<IReadOnlyList<AssetSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<IReadOnlyList<AssetSummaryDto>>>> GetAll(
        [FromQuery] int? categoryId,
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        LogStarted(nameof(GetAll));
        var assets = await service.GetAssetsAsync(categoryId, status, cancellationToken);
        LogCompleted(nameof(GetAll));
        return Ok(Result<IReadOnlyList<AssetSummaryDto>>.Success(assets));
    }

    [HttpGet("{id:int}")]
    [EndpointSummary("Consulta un activo con su categoría, ubicación y órdenes relacionadas.")]
    [ProducesResponseType(typeof(Result<AssetDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<AssetDetailDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        LogStarted(nameof(GetById));
        var asset = await service.GetAssetAsync(id, cancellationToken);
        LogCompleted(nameof(GetById), id);
        return Ok(Result<AssetDetailDto>.Success(asset));
    }

    [HttpPost]
    [EndpointSummary("Registra un activo nuevo con código ACT- único.")]
    [ProducesResponseType(typeof(Result<AssetDetailDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<AssetDetailDto>>> Create(
        AssetWriteRequest request, CancellationToken cancellationToken)
    {
        LogStarted(nameof(Create));
        var asset = await service.CreateAssetAsync(request, cancellationToken);
        LogCompleted(nameof(Create), asset.Id);
        return CreatedAtAction(nameof(GetById), new { id = asset.Id },
            Result<AssetDetailDto>.Success(asset, StatusCodes.Status201Created, "Created"));
    }

    [HttpPut("{id:int}")]
    [EndpointSummary("Actualiza los datos generales de un activo.")]
    [ProducesResponseType(typeof(Result<AssetDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<AssetDetailDto>>> Update(
        int id, AssetWriteRequest request, CancellationToken cancellationToken)
    {
        LogStarted(nameof(Update));
        var asset = await service.UpdateAssetAsync(id, request, cancellationToken);
        LogCompleted(nameof(Update), id);
        return Ok(Result<AssetDetailDto>.Success(asset));
    }

    [HttpPatch("{id:int}/status")]
    [EndpointSummary("Cambia administrativamente el estado del activo a fuera de servicio o dado de baja.")]
    [ProducesResponseType(typeof(Result<AssetDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<AssetDetailDto>>> ChangeStatus(
        int id, AssetStatusRequest request, CancellationToken cancellationToken)
    {
        LogStarted(nameof(ChangeStatus));
        var asset = await service.ChangeAssetStatusAsync(id, request.Status, cancellationToken);
        LogCompleted(nameof(ChangeStatus), id);
        return Ok(Result<AssetDetailDto>.Success(asset));
    }

    [HttpDelete("{id:int}")]
    [EndpointSummary("Da de baja lógica un activo sin órdenes de mantenimiento asociadas.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Deactivate(int id, CancellationToken cancellationToken)
    {
        LogStarted(nameof(Deactivate));
        await service.DeactivateAssetAsync(id, cancellationToken);
        LogCompleted(nameof(Deactivate), id);
        return NoContent();
    }

    [HttpPut("{id:int}/image")]
    [EndpointSummary("Actualiza la URL de la imagen representativa del activo.")]
    [ProducesResponseType(typeof(Result<AssetDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<AssetDetailDto>>> UpdateImage(
        int id, AssetImageRequest request, CancellationToken cancellationToken)
    {
        LogStarted(nameof(UpdateImage));
        var asset = await service.UpdateAssetImageAsync(id, request.ImageUrl, cancellationToken);
        LogCompleted(nameof(UpdateImage), id);
        return Ok(Result<AssetDetailDto>.Success(asset));
    }
}
