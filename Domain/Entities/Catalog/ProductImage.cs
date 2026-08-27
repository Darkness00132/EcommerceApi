using System.ComponentModel.DataAnnotations;
using Domain.Common;
using Domain.Exceptions;

namespace Domain.Entities.Catalog;

public sealed class ProductImage : IEntity
{
    public Guid ProductId { get; private set; }

    [MaxLength(400)]
    public string ImageKey { get; private set; } = null!;

    public Product Product { get; private set; } = null!;

    private ProductImage() { } // Required for EF Core

    public ProductImage(Guid productId, string imageKey)
    {
        if (productId == Guid.Empty)
            throw new DomainException("Product ID cannot be empty.");

        if (string.IsNullOrWhiteSpace(imageKey))
            throw new DomainException("Product image key is required.");

        ProductId = productId;
        ImageKey = imageKey.Trim();
    }
}
