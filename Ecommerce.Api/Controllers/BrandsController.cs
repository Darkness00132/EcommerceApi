using Api.Contracts.Common;
using Application.Common.Pagination;
using Application.Features.Brands.Dtos;
using Application.Features.Brands.Queries.GetBrandById;
using Application.Features.Brands.Queries.GetBrands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

/// <summary>
/// Provides public endpoints for browsing brands.
/// </summary>
[ApiController]
[Route("api/brands")]
public sealed class BrandsController : ControllerBase
{
    private readonly ISender _sender;

    /// <summary>
    /// Initializes a new instance of the <see cref="BrandsController"/> class.
    /// </summary>
    /// <param name="sender">The mediator sender used to dispatch application requests.</param>
    public BrandsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Retrieves a paginated list of brands.
    /// </summary>
    /// <param name="cancellationToken">A token used to cancel the request.</param>
    /// <returns>A paginated list of brands.</returns>
    /// <response code="200">Returns the brands.</response>
    /// <response code="400">The pagination request is invalid.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<BrandDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<BrandDto>>> GetBrands(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetBrandsQuery(), cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves a brand by its identifier.
    /// </summary>
    /// <param name="id">The brand identifier.</param>
    /// <param name="cancellationToken">A token used to cancel the request.</param>
    /// <returns>The requested brand.</returns>
    /// <response code="200">Returns the requested brand.</response>
    /// <response code="404">The brand was not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBrandById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetBrandByIdQuery(id),
            cancellationToken);

        return Ok(result);
    }
}
