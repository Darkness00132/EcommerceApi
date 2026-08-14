using Application.Features.Discounts.Commands.CreateDiscount;
using Application.Features.Discounts.Commands.DeleteDiscount;
using Application.Features.Discounts.Commands.UpdateDiscount;
using Application.Features.Discounts.Common;
using Application.Features.Discounts.Queries.GetDiscount;
using Application.Features.Discounts.Queries.GetDiscounts;
using AutoMapper;
using Ecommerce.Api.Contracts.Discount;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers.Admin;

/// <summary>
/// Provides operations for creating, viewing, updating, and deleting discounts.
/// </summary>
/// <remarks>
/// Discounts are managed independently and can later be assigned to products.
/// These endpoints are intended for administrative use.
/// </remarks>
[ApiController]
[Route("api/discounts")]
public class AdminDiscountsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;

    public AdminDiscountsController(
        ISender sender,
        IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    /// <summary>
    /// Retrieves all discounts.
    /// </summary>
    /// <remarks>
    /// Returns all discounts in the system, including active, inactive,
    /// scheduled, and expired discounts.
    /// </remarks>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A collection of discounts.
    /// </returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<DiscountDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<DiscountDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetDiscountsQuery(),
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves a discount by its identifier.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the discount.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// The requested discount.
    /// </returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(DiscountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DiscountDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetDiscountQuery(id),
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Creates a new discount.
    /// </summary>
    /// <remarks>
    /// Discounts can be created as either percentage-based or fixed-amount discounts.
    /// The validity period determines when the discount can be applied.
    /// </remarks>
    /// <param name="command">
    /// The discount details.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// The identifier of the newly created discount.
    /// </returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        CreateDiscountCommand command,
        CancellationToken cancellationToken)
    {
        var discountId = await _sender.Send(
            command,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = discountId },
            discountId);
    }

    /// <summary>
    /// Updates an existing discount.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the discount to update.
    /// </param>
    /// <param name="request">
    /// The updated discount details.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the operation.
    /// </param>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateDiscountRequest request,
        CancellationToken cancellationToken)
    {
        var command = _mapper.Map<UpdateDiscountCommand>(request)
            with
        { Id = id };

        await _sender.Send(
            command,
            cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Deletes a discount.
    /// </summary>
    /// <remarks>
    /// Any products currently associated with this discount will have the
    /// discount removed before the discount itself is deleted.
    /// </remarks>
    /// <param name="id">
    /// The unique identifier of the discount to delete.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the operation.
    /// </param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _sender.Send(
            new DeleteDiscountCommand(id),
            cancellationToken);

        return NoContent();
    }
}