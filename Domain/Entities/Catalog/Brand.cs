using System.ComponentModel.DataAnnotations;
using Domain.Common;
using Domain.Exceptions;

namespace Domain.Entities.Catalog;

public sealed class Brand : Entity
{
    private readonly List<Product> _products = new();

    [MaxLength(100)]
    public string NameEn { get; private set; } = null!;

    [MaxLength(100)]
    public string NameAr { get; private set; } = null!;

    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

    private Brand() { } // Required for EF Core

    public Brand(string nameEn, string nameAr)
    {
        UpdateEnglishName(nameEn);
        UpdateArabicName(nameAr);
        Id = Guid.NewGuid();
    }

    public void UpdateEnglishName(string nameEn)
    {
        if (string.IsNullOrWhiteSpace(nameEn))
            throw new DomainException("English brand name is required.");

        NameEn = nameEn.Trim();
    }

    public void UpdateArabicName(string nameAr)
    {
        if (string.IsNullOrWhiteSpace(nameAr))
            throw new DomainException("Arabic brand name is required.");

        NameAr = nameAr.Trim();
    }
}
