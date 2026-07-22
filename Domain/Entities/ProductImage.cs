namespace Domain.Entities
{
    public class ProductImage
    {
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public string ImageKey { get; set; } = null!;
    }
}
