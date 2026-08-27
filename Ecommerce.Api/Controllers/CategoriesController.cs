using Api.Contracts.Common;
using Application.Common.Pagination;
using Application.Features.Categories.Dtos;
using Application.Features.Categories.Queries.GetCategories;
using Application.Features.Categories.Queries.GetCategoryById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

/// <summary>
/// Provides public endpoints for browsing categories.
/// </summary>
[ApiController]
[Route("api/categories")]
public sealed class CategoriesController : ControllerBase
{
    private readonly ISender _sender;

    /// <summary>
    /// Initializes a new instance of the <see cref="CategoriesController"/> class.
    /// </summary>
    /// <param name="sender">The mediator sender used to dispatch application requests.</param>
    public CategoriesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Retrieves a paginated list of categories.
    /// </summary>
    /// <param name="cancellationToken">A token used to cancel the request.</param>
    /// <returns>A paginated list of categories.</returns>
    /// <response code="200">Returns the paginated categories.</response>
    /// <response code="400">The pagination request is invalid.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<CategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetCategoriesQuery(),
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves a category by its identifier.
    /// </summary>
    /// <param name="id">The category identifier.</param>
    /// <param name="cancellationToken">A token used to cancel the request.</param>
    /// <returns>The requested category.</returns>
    /// <response code="200">Returns the requested category.</response>
    /// <response code="404">The category was not found.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCategoryById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetCategoryByIdQuery(id),
            cancellationToken);

        return Ok(result);
    }
}
