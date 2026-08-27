using System.ComponentModel.DataAnnotations;
using Domain.Common;
using Domain.Exceptions;

namespace Domain.Entities.Catalog;

public sealed class Category : Entity
{
    private readonly List<Product> _products = new();

    [MaxLength(100)]
    public string NameEn { get; private set; } = null!;

    [MaxLength(100)]
    public string NameAr { get; private set; } = null!;

    [MaxLength(500)]
    public string? DescriptionEn { get; private set; }

    [MaxLength(500)]
    public string? DescriptionAr { get; private set; }

    [MaxLength(500)]
    public string ImageKey { get; private set; } = null!;

    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

    private Category() { } // Required for EF Core

    public Category(
        string nameEn,
        string nameAr,
        string imageKey,
        string? descriptionEn = null,
        string? descriptionAr = null)
    {
        UpdateDetails(nameEn, nameAr, descriptionEn, descriptionAr);
        UpdateImageKey(imageKey);
        Id = Guid.NewGuid();
    }

    public void UpdateDetails(
        string nameEn,
        string nameAr,
        string? descriptionEn = null,
        string? descriptionAr = null)
    {
        if (string.IsNullOrWhiteSpace(nameEn))
            throw new DomainException("English category name is required.");

        if (string.IsNullOrWhiteSpace(nameAr))
            throw new DomainException("Arabic category name is required.");

        NameEn = nameEn.Trim();
        NameAr = nameAr.Trim();
        DescriptionEn = string.IsNullOrWhiteSpace(descriptionEn) ? null : descriptionEn.Trim();
        DescriptionAr = string.IsNullOrWhiteSpace(descriptionAr) ? null : descriptionAr.Trim();
    }

    public void UpdateImageKey(string imageKey)
    {
        if (string.IsNullOrWhiteSpace(imageKey))
            throw new DomainException("Category image key is required.");

        ImageKey = imageKey.Trim();
    }
}
