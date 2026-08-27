using Api.Contracts.Categories;
using Application.Features.Categories.Commands.CreateCategory;
using Application.Features.Categories.Commands.UpdateCategory;
using AutoMapper;
using Domain.Constants;
using Ecommerce.Api.Contracts.Categories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers.Admin;

/// <summary>
/// Provides administrator endpoints for managing categories.
/// </summary>
[ApiController]
[Authorize(Roles = AppRoles.CatalogAdministrators)]
[Route("api/admin/categories")]
public sealed class AdminCategoriesController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly ISender _sender;

    public AdminCategoriesController(IMapper mapper, ISender sender)
    {
        _mapper = mapper;
        _sender = sender;
    }

    /// <summary>
    /// Creates a new category.
    /// </summary>
    /// <param name="request">The create category request.</param>
    /// <param name="cancellationToken">A token used to cancel the request.</param>
    /// <returns>The created category identifier.</returns>
    /// <response code="201">The category was created successfully.</response>
    /// <response code="400">The request contains invalid data.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="403">The user is not authorized to manage categories.</response>
    /// <response code="409">A category with the same name already exists.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateCategory(
        [FromForm] CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var command = _mapper.Map<CreateCategoryCommand>(request);
        var categoryId = await _sender.Send(command, cancellationToken);

        return CreatedAtAction(
            actionName: nameof(CategoriesController.GetCategoryById),
            controllerName: "Categories",
            routeValues: new { id = categoryId },
            value: categoryId);
    }

    /// <summary>
    /// Updates an existing category.
    /// </summary>
    /// <param name="id">The category identifier.</param>
    /// <param name="request">The update category request.</param>
    /// <param name="cancellationToken">A token used to cancel the request.</param>
    /// <returns>No content.</returns>
    /// <response code="204">The category was updated successfully.</response>
    /// <response code="400">The request contains invalid data.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="403">The user is not authorized to manage categories.</response>
    /// <response code="404">The category was not found.</response>
    /// <response code="409">A category with the same name already exists.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateCategory(
        Guid id,
        [FromForm] UpdateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var command = _mapper.Map<UpdateCategoryCommand>(request) with { Id = id };
        await _sender.Send(cancellationToken);

        return NoContent();
    }
}
