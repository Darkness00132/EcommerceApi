using Application.Features.Brands.Commands.CreateBrand;
using Application.Features.Brands.Commands.UpdateBrand;
using Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers.Admin;

/// <summary>
/// Provides administrator endpoints for managing brands.
/// </summary>
[ApiController]
[Authorize(Roles = AppRoles.CatalogAdministrators)]
[Route("api/admin/brands")]
[Produces("application/json")]
public sealed class AdminBrandsController : ControllerBase
{
    private readonly ISender _sender;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminBrandsController"/> class.
    /// </summary>
    /// <param name="sender">The mediator sender used to dispatch application requests.</param>
    public AdminBrandsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Creates a new brand.
    /// </summary>
    /// <param name="command">The brand creation request.</param>
    /// <param name="cancellationToken">A token used to cancel the request.</param>
    /// <returns>The created brand identifier.</returns>
    /// <response code="201">The brand was created successfully.</response>
    /// <response code="400">The request contains invalid data.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="403">The user is not authorized to manage brands.</response>
    /// <response code="409">A brand with the same name already exists.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateBrand(
        [FromBody] CreateBrandCommand command,
        CancellationToken cancellationToken)
    {
        var brandId = await _sender.Send(command, cancellationToken);

        return CreatedAtAction(
            actionName: nameof(BrandsController.GetBrandById),
            controllerName: "Brands",
            routeValues: new { id = brandId },
            value: brandId);
    }

    /// <summary>
    /// Updates an existing brand.
    /// </summary>
    /// <param name="id">The brand identifier.</param>
    /// <param name="nameAr">The brand update arabic name.</param>
    /// <param name="nameEn">The brand update english name.</param>
    /// <param name="cancellationToken">A token used to cancel the request.</param>
    /// <returns>No content.</returns>
    /// <response code="204">The brand was updated successfully.</response>
    /// <response code="400">The request contains invalid data.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="403">The user is not authorized to manage brands.</response>
    /// <response code="404">The brand was not found.</response>
    /// <response code="409">A brand with the same name already exists.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateBrand(
        Guid id,
        string? nameEn,
        string? nameAr,
        CancellationToken cancellationToken)
    {
        var command = new UpdateBrandCommand(id, nameEn, nameAr);
        await _sender.Send(command, cancellationToken);

        return NoContent();
    }
}