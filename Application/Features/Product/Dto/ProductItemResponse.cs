namespace Application.Features.Product.Dto
{
    public record ProductItemResponse(string NameEn,
        string NameAr,
        decimal Price,
        int Stock,
        string Brand,
        DateTime CreatedAt);
}
