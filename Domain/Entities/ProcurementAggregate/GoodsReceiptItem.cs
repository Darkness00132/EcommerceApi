using Domain.Common;
using Domain.Entities.Catalog;
using Domain.Exceptions;

namespace Domain.Entities.ProcurementAggregate;

public sealed class GoodsReceiptItem : Entity
{
    public Guid GoodsReceiptId { get; private set; }

    public GoodsReceipt GoodsReceipt { get; private set; } = null!;

    public Guid ProductId { get; private set; }

    public Product Product { get; private set; } = null!;

    public int Quantity { get; private set; }

    private GoodsReceiptItem() { }

    internal GoodsReceiptItem(
        Guid goodsReceiptId,
        Guid productId,
        int quantity)
        : base(Guid.NewGuid())
    {
        if (goodsReceiptId == Guid.Empty)
            throw new DomainException("Goods receipt id is required.");

        if (productId == Guid.Empty)
            throw new DomainException("Product id is required.");

        if (quantity <= 0)
            throw new DomainException(
                "Quantity must be greater than zero.");

        GoodsReceiptId = goodsReceiptId;
        ProductId = productId;
        Quantity = quantity;
    }

    internal void IncreaseQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException(
                "Quantity must be greater than zero.");

        Quantity += quantity;
    }
}
