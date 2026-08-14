using Domain.Common;
using Domain.Exceptions;

namespace Domain.Entities.Catalog;

public sealed class Category : Entity
{
    public string NameEn { get; private set; } = null!;
    public string NameAr { get; private set; } = null!;

    public string? DescriptionEn { get; private set; }
    public string? DescriptionAr { get; private set; }

    public string ImageKey { get; private set; } = null!;

    public ICollection<Product> Products { get; private set; } = new List<Product>();

    private Category() { }
    public Category(
        string nameEn,
        string nameAr,
        string imageKey,
        string? descriptionEn = null,
        string? descriptionAr = null)
    {
        if (string.IsNullOrWhiteSpace(nameEn))
            throw new DomainException("English category name is required.");

        if (string.IsNullOrWhiteSpace(nameAr))
            throw new DomainException("Arabic category name is required.");

        if (string.IsNullOrWhiteSpace(imageKey))
            throw new DomainException("Category image is required.");

        Id = Guid.NewGuid();
        NameEn = nameEn.Trim();
        NameAr = nameAr.Trim();
        ImageKey = imageKey.Trim();
        DescriptionEn = string.IsNullOrWhiteSpace(descriptionEn) ? null : descriptionEn.Trim();
        DescriptionAr = string.IsNullOrWhiteSpace(descriptionAr) ? null : descriptionAr.Trim();
    }

    public void Update(
        string? nameEn,
        string? nameAr,
        string? imageKey,
        string? descriptionEn = null,
        string? descriptionAr = null)
    {

        NameEn = string.IsNullOrWhiteSpace(nameEn) ? NameEn : nameEn.Trim();
        NameAr = string.IsNullOrWhiteSpace(nameAr) ? NameAr : nameAr.Trim();
        ImageKey = string.IsNullOrWhiteSpace(imageKey) ? ImageKey : imageKey.Trim();
        DescriptionEn = string.IsNullOrWhiteSpace(descriptionEn) ? DescriptionEn : descriptionEn.Trim();
        DescriptionAr = string.IsNullOrWhiteSpace(descriptionAr) ? DescriptionAr : descriptionAr.Trim();
    }
}