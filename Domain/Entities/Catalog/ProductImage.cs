using Domain.Common;
using Domain.Exceptions;

namespace Domain.Entities.Catalog;

public sealed class ProductImage : IEntity
{
    public Guid ProductId { get; private set; }

    public string ImageKey { get; private set; } = null!;

    public Product Product { get; private set; } = null!;

    private ProductImage() { }

    public ProductImage(Guid productId, string imageKey)
    {
        if (string.IsNullOrWhiteSpace(imageKey))
            throw new DomainException("Product image is required.");

        ProductId = productId;
        ImageKey = imageKey.Trim();
    }
}