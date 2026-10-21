using Microsoft.AspNetCore.Mvc;
using Proyecto.Application.DTO;
using Proyecto.Application.Interfaces;
using Proyecto.Domain.DTO.PatternResult;
using Proyecto.WebAPI.Models;

namespace Proyecto.WebAPI.Controllers;

[ApiController]
[Route("api/asset-categories")]
public sealed class AssetCategoriesController(
    IAssetManagementService service,
    ILogger<AssetCategoriesController> logger) : LoggedApiController<AssetCategoriesController>(logger)
{
    [HttpGet]
    [EndpointSummary("Lista categorías activas y su cantidad de activos asociados.")]
    [ProducesResponseType(typeof(Result<IReadOnlyList<CategorySummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<IReadOnlyList<CategorySummaryDto>>>> GetAll(CancellationToken cancellationToken)
    {
        LogStarted(nameof(GetAll));
        var categories = await service.GetCategoriesAsync(cancellationToken);
        LogCompleted(nameof(GetAll));
        return Ok(Result<IReadOnlyList<CategorySummaryDto>>.Success(categories));
    }

    [HttpGet("{id:int}")]
    [EndpointSummary("Consulta una categoría con sus activos y el intervalo preventivo.")]
    [ProducesResponseType(typeof(Result<CategoryDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<CategoryDetailDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        LogStarted(nameof(GetById));
        var category = await service.GetCategoryAsync(id, cancellationToken);
        LogCompleted(nameof(GetById), id);
        return Ok(Result<CategoryDetailDto>.Success(category));
    }

    [HttpPost]
    [EndpointSummary("Registra una categoría de activos.")]
    [ProducesResponseType(typeof(Result<CategoryDetailDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<CategoryDetailDto>>> Create(
        CategoryWriteRequest request, CancellationToken cancellationToken)
    {
        LogStarted(nameof(Create));
        var category = await service.CreateCategoryAsync(request, cancellationToken);
        LogCompleted(nameof(Create), category.Id);
        return CreatedAtAction(nameof(GetById), new { id = category.Id },
            Result<CategoryDetailDto>.Success(category, StatusCodes.Status201Created, "Created"));
    }

    [HttpPut("{id:int}")]
    [EndpointSummary("Actualiza el nombre, la descripción o el intervalo preventivo de una categoría.")]
    [ProducesResponseType(typeof(Result<CategoryDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<CategoryDetailDto>>> Update(
        int id, CategoryWriteRequest request, CancellationToken cancellationToken)
    {
        LogStarted(nameof(Update));
        var category = await service.UpdateCategoryAsync(id, request, cancellationToken);
        LogCompleted(nameof(Update), id);
        return Ok(Result<CategoryDetailDto>.Success(category));
    }

    [HttpDelete("{id:int}")]
    [EndpointSummary("Elimina una categoría que no tenga activos asociados.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        LogStarted(nameof(Delete));
        await service.DeleteCategoryAsync(id, cancellationToken);
        LogCompleted(nameof(Delete), id);
        return NoContent();
    }
}
