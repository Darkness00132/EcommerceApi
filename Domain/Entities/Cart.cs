namespace Domain.Entities
{
    public class Cart
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }
        public AppUser User { get; set; } = null!;

        public ICollection<CartItem> CartItems { get; set; } = [];
    }
}
