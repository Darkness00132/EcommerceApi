using Application.Common;
using Application.Common.Pagination;
using Application.Features.Category.CreateCategory;
using Application.Features.Category.DeleteCategory;
using Application.Features.Category.Dtos;
using Application.Features.Category.GetCategories;
using Application.Features.Category.GetProductsWithinCategory;
using Application.Features.Category.UpdateCategory;
using Application.Features.Product.Dto;
using AutoMapper;
using Domain.Enums;
using Ecommerce.Api.Contracts.Category;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public sealed class CategoryController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISender _sender;

        public CategoryController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Retrieves a paginated list of categories.
        /// </summary>
        /// <remarks>
        /// Supports pagination and any filtering or sorting options defined by
        /// <see cref="GetCategoriesQuery"/>.
        /// </remarks>
        /// <param name="query">
        /// The pagination, filtering, and sorting parameters.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancels the operation if the HTTP request is aborted.
        /// </param>
        /// <returns>
        /// A paginated collection of categories.
        /// </returns>
        [HttpGet]
        [ProducesResponseType(
            typeof(PaginationResult<CategoryResponse>),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PaginationResult<CategoryResponse>>>
            GetCategories(
                [FromQuery] PaginationRequest request,
                CancellationToken cancellationToken)
        {
            var query = new GetCategoriesQuery(request);
            var result = await _sender.Send(
                query,
                cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Retrieves the products that belong to a category.
        /// </summary>
        /// <param name="id">
        /// The identifier of the category.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancels the operation if the HTTP request is aborted.
        /// </param>
        /// <returns>
        /// The products associated with the specified category.
        /// </returns>
        [HttpGet("{id}/products")]
        [ProducesResponseType(
            typeof(IReadOnlyList<ProductItemResponse>),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyList<ProductItemResponse>>>
            GetProductsWithinCategory(
                [FromRoute] int id,
                [FromQuery] PaginationRequest pagination,
                CancellationToken cancellationToken)
        {
            var query = new GetProductsWithinCategoryQuery(id, pagination);

            var products = await _sender.Send(
                query,
                cancellationToken);

            return Ok(products);
        }

        /// <summary>
        /// Creates a new category.
        /// </summary>
        /// <remarks>
        /// This endpoint requires the administrator role.
        ///
        /// The request must use <c>multipart/form-data</c> because it can include
        /// an uploaded category image.
        /// </remarks>
        /// <param name="request">
        /// The category information and optional image.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancels the operation if the HTTP request is aborted.
        /// </param>
        /// <returns>
        /// A 201 Created response when the category is created successfully.
        /// </returns>
        [Authorize(Roles = nameof(UserRoles.Admin))]
        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateCategory(
            [FromForm] CreateCategoryRequest request,
            CancellationToken cancellationToken)
        {
            var command = _mapper.Map<CreateCategoryRequest, CreateCategoryCommand>(request);
            await _sender.Send(
                command,
                cancellationToken);

            // The command doesn't return the category ID, so no resource
            // location is included in the response.
            return StatusCode(StatusCodes.Status201Created);
        }

        /// <summary>
        /// Updates an existing category.
        /// </summary>
        /// <remarks>
        /// This endpoint requires the administrator role.
        ///
        /// The request must use <c>multipart/form-data</c> because it can include
        /// a replacement category image.
        /// </remarks>
        /// <param name="id">
        /// The identifier of the category to update.
        /// </param>
        /// <param name="request">
        /// The updated category information and optional replacement image.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancels the operation if the HTTP request is aborted.
        /// </param>
        /// <returns>
        /// A 204 No Content response when the category is updated successfully.
        /// </returns>
        [Authorize(Roles = nameof(UserRoles.Admin))]
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateCategory(
            [FromRoute] int id,
            [FromForm] UpdateCategoryRequest request,
            CancellationToken cancellationToken)
        {
            var command = _mapper.Map<UpdateCategoryRequest, UpdateCategoryCommand>(request);

            await _sender.Send(
                command,
                cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Deletes a category.
        /// </summary>
        /// <remarks>
        /// This endpoint requires the administrator role.
        ///
        /// Deletion may be rejected when the category is referenced by products
        /// or other protected resources.
        /// </remarks>
        /// <param name="id">
        /// The identifier of the category to delete.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancels the operation if the HTTP request is aborted.
        /// </param>
        /// <returns>
        /// A 204 No Content response when the category is deleted successfully.
        /// </returns>
        [Authorize(Roles = nameof(UserRoles.Admin))]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteCategory(
            [FromRoute] int id,
            CancellationToken cancellationToken)
        {
            var command = new DeleteCategoryCommand(id);

            await _sender.Send(
                command,
                cancellationToken);

            return NoContent();
        }
    }
}