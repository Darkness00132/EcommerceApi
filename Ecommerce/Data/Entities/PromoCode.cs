using Ecommerce.Data.Enums;

namespace Ecommerce.Data.Entities
{
    public class PromoCode
    {
        public int Id { get; set; }

        public string Code { get; set; } = null!;
        public PromoDiscountType DiscountType { get; set; } = PromoDiscountType.Percentage;

        public decimal Value { get; set; }
        public decimal MinimumOrder { get; set; }

        public int UsageLimit { get; set; }
        public int UsedCount { get; set; }

        public DateOnly ExpirationDate { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Order> Orders { get; set; } = [];
    }
}
