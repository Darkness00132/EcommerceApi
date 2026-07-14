using Ecommerce.Data.Enums;

namespace Ecommerce.Data.Entities
{
    public class Order
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }
        public AppUser User { get; set; } = null!;

        public int? PromoCodeId { get; set; }
        public PromoCode? PromoCode { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public decimal Subtotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal Total { get; set; }

        public string ShippingAddress { get; set; } = null!;
        public string ShippingCity { get; set; } = null!;
        public string ShippingPhone { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Payment? Payment { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = [];
    }
}
