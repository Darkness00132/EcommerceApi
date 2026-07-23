using Application.Common.Filters;
using Application.Common.Pagination;
using Application.Features.Product.CreateProduct;
using Application.Features.Product.DeleteProduct;
using Application.Features.Product.Dto;
using Application.Features.Product.GetProduct;
using Application.Features.Product.GetProducts;
using Application.Features.Product.PatchProductStatus;
using Application.Features.Product.PatchProductStock;
using Application.Features.Product.UpdateProduct;
using AutoMapper;
using Domain.Enums;
using Ecommerce.Api.Contracts.Product;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public ProductController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a paginated list of products based on filter criteria.
        /// </summary>
        /// <remarks>
        /// Supports search filters, sorting options, and pagination controls.
        /// </remarks>
        /// <param name="filters">Criteria used to filter products (e.g., category, price range, status).</param>
        /// <param name="pagination">Pagination configuration including page size and index.</param>
        /// <param name="ct">Cancels the operation if the HTTP request is aborted.</param>
        /// <returns>A paginated collection of products matching the specified criteria.</returns>
        [HttpGet]
        public async Task<ActionResult<PaginationResult<ProductItemResponse>>> GetProducts(
            [FromQuery] ProductFilters filters,
            [FromQuery] PaginationRequest pagination,
            CancellationToken ct)
        {
            var query = new GetProductsQuery(filters, pagination);
            var result = await _sender.Send(query, ct);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves detailed information for a specific product by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the target product.</param>
        /// <param name="ct">Cancels the operation if the HTTP request is aborted.</param>
        /// <returns>The detailed product record if found.</returns>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<DetailedProductResponse>> GetProduct(int id, CancellationToken ct)
        {
            var query = new GetProductQuery(id);
            var response = await _sender.Send(query, ct);
            return Ok(response);
        }

        /// <summary>
        /// Creates a new product catalog item with image assets.
        /// </summary>
        /// <remarks>
        /// Requires administrative privileges. Expects multipart form data to process file uploads.
        /// </remarks>
        /// <param name="request">The form payload containing product metadata and uploaded image files.</param>
        /// <param name="ct">Cancels the operation if the HTTP request is aborted.</param>
        /// <returns>201 status code in success.</returns>
        [Authorize(Roles = nameof(UserRoles.Admin))]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<DetailedProductResponse>> CreateProduct(
            [FromForm] CreateProductRequest request,
            CancellationToken ct)
        {
            var command = _mapper.Map<CreateProductCommand>(request);
            await _sender.Send(command, ct);
            return StatusCode(StatusCodes.Status201Created);
        }

        /// <summary>
        /// Updates the details and media of an existing product.
        /// </summary>
        /// <remarks>
        /// Requires administrative privileges. Replaces the product details and optional new images via form payload.
        /// </remarks>
        /// <param name="id">The unique identifier of the product to update.</param>
        /// <param name="request">The form payload containing updated product values and file assets.</param>
        /// <param name="ct">Cancels the operation if the HTTP request is aborted.</param>
        /// <returns>An empty response with HTTP status code 204 (No Content) upon success.</returns>
        [Authorize(Roles = nameof(UserRoles.Admin))]
        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateProduct(int id,[FromForm] UpdateProductRequest request,
            CancellationToken ct)
        {
            var command = _mapper.Map<UpdateProductCommand>(request) with { Id = id};
            await _sender.Send(command, ct);
            return NoContent();
        }

        /// <summary>
        /// Updates the stock quantity of a specific product.
        /// </summary>
        /// <remarks>
        /// Requires administrative privileges. Performs a partial update focused strictly on inventory levels.
        /// </remarks>
        /// <param name="command">Contains the target product ID and new stock count.</param>
        /// <param name="ct">Cancels the operation if the HTTP request is aborted.</param>
        /// <returns>An empty response with HTTP status code 204 (No Content) upon success.</returns>
        [Authorize(Roles = nameof(UserRoles.Admin))]
        [HttpPatch("{id:int}/stock")]
        public async Task<IActionResult> PatchProductStock(PatchProductStockCommand command, CancellationToken ct)
        {
            await _sender.Send(command, ct);
            return NoContent();
        }

        /// <summary>
        /// Toggles or updates the active status of a product.
        /// </summary>
        /// <remarks>
        /// Requires administrative privileges. Modifies product visibility/availability in the catalog.
        /// </remarks>
        /// <param name="command">Contains the target product ID and status update details.</param>
        /// <param name="ct">Cancels the operation if the HTTP request is aborted.</param>
        /// <returns>An empty response with HTTP status code 204 (No Content) upon success.</returns>
        [Authorize(Roles = nameof(UserRoles.Admin))]
        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> PatchProductStatus(PatchProductStatusCommand command, CancellationToken ct)
        {
            await _sender.Send(command, ct);
            return NoContent();
        }

        /// <summary>
        /// Permanently removes a product from the catalog.
        /// </summary>
        /// <remarks>
        /// Requires administrative privileges. Performs a hard deletion of the specified product entity.
        /// </remarks>
        /// <param name="id">The unique identifier of the product to delete.</param>
        /// <param name="ct">Cancels the operation if the HTTP request is aborted.</param>
        /// <returns>An empty response with HTTP status code 204 (No Content) upon success.</returns>
        [Authorize(Roles = nameof(UserRoles.Admin))]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id, CancellationToken ct)
        {
            var command = new DeleteProductCommand(id);
            await _sender.Send(command, ct);
            return NoContent();
        }
    }
}