using Domain.Enums;

namespace Domain.Entities
{
    public class Discount
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public DiscountType DiscountType { get; set; }

        public decimal Value { get; set; }

        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Product> Products { get; set; } = [];
    }
}
