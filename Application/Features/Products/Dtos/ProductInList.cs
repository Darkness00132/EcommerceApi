using Domain.Enums;

namespace Application.Features.Products.Dtos;

public record ProductInList
{
    public Guid Id { get; set; }
    public string NameEn { get; set; } = null!;
    public string NameAr { get; set; } = null!;
    public decimal Price { get; set; }
    public string CategoryNameEn { get; set; } = null!;
    public string CategoryNameAr { get; set; } = null!;
    public string BrandNameEn { get; set; } = null!;
    public string BrandNameAr { get; set; } = null!;
    public DiscountInProduct? Discount { get; set; }

    public List<ProductImageDto> Images { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}
