using Domain.Common;
using Domain.Entities.Identity;
using Domain.Exceptions;

namespace Domain.Entities.Carts;

public sealed class Cart : IEntity
{
    private readonly List<CartItem> _items = new();

    public Guid UserId { get; private set; }

    public AppUser User { get; private set; } = null!;

    public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

    private Cart() { }

    public Cart(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new DomainException("User ID cannot be empty.");

        UserId = userId;
    }

    public void AddItem(Guid productId, int quantity)
    {
        if (productId == Guid.Empty)
            throw new DomainException("Product ID cannot be empty.");

        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        var existingItem = _items.FirstOrDefault(x => x.ProductId == productId);

        if (existingItem is not null) {
            existingItem.IncreaseQuantity(quantity);
            return;
        }

        _items.Add(new CartItem(
            cartId: UserId,
            productId: productId,
            quantity: quantity));
    }

    public void UpdateItemQuantity(Guid productId, int quantity)
    {
        if (productId == Guid.Empty)
            throw new DomainException("Product ID cannot be empty.");

        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        var item = _items.FirstOrDefault(x => x.ProductId == productId);

        if (item is null)
            throw new DomainException("Cart item was not found.");

        item.ChangeQuantity(quantity);
    }

    public void RemoveItem(Guid productId)
    {
        if (productId == Guid.Empty) return;

        var item = _items.FirstOrDefault(x => x.ProductId == productId);

        if (item is null)
            return;

        _items.Remove(item);
    }

    public void Clear()
    {
        _items.Clear();
    }
}
