using Domain.Common;
using Domain.Entities.Identity;
using Domain.Exceptions;

namespace Domain.Entities.Carts;

public sealed class Cart : IEntity
{
    public Guid UserId { get; private set; }

    public AppUser User { get; private set; } = null!;

    public ICollection<CartItem> Items { get; private set; } = new List<CartItem>();

    private Cart()
    {
    }

    public Cart(Guid userId)
    {
        UserId = userId;
    }

    public void AddItem(
        Guid productId,
        int quantity,
        decimal unitPrice,
        decimal discountAmount = 0)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        if (unitPrice < 0)
            throw new DomainException("Unit price cannot be negative.");

        if (discountAmount < 0)
            throw new DomainException("Discount amount cannot be negative.");

        var existingItem = Items.FirstOrDefault(x => x.ProductId == productId);

        if (existingItem is not null)
        {
            existingItem.IncreaseQuantity(quantity);
            return;
        }

        Items.Add(new CartItem(
            cartId: UserId,
            productId: productId,
            quantity: quantity,
            unitPrice: unitPrice,
            discountAmount: discountAmount));
    }

    public void UpdateItemQuantity(Guid productId, int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        var item = Items.FirstOrDefault(x => x.ProductId == productId);

        if (item is null)
            throw new DomainException("Cart item was not found.");

        item.ChangeQuantity(quantity);
    }

    public void RemoveItem(Guid productId)
    {
        var item = Items.FirstOrDefault(x => x.ProductId == productId);

        if (item is null)
            return;

        Items.Remove(item);
    }

    public void Clear()
    {
        Items.Clear();
    }
}