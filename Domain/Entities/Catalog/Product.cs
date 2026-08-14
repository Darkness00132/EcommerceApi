using Domain.Common;
using Domain.Exceptions;

namespace Domain.Entities.Catalog;

public sealed class Product : AggregateRoot
{
    public string NameEn { get; private set; } = null!;
    public string NameAr { get; private set; } = null!;

    public string SKU { get; private set; } = null!;

    public string? DescriptionEn { get; private set; }
    public string? DescriptionAr { get; private set; }

    public decimal Price { get; private set; }

    public bool IsActive { get; private set; }

    public Guid CategoryId { get; private set; }
    public Guid BrandId { get; private set; }
    public Guid? DiscountId { get; private set; }

    public Category Category { get; private set; } = null!;
    public Brand Brand { get; private set; } = null!;
    public Discount? Discount { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public byte[] RowVersion { get; private set; } = [];

    public ICollection<ProductImage> Images { get; private set; } = new List<ProductImage>();

    private Product() { }

    public Product(
        string nameEn,
        string nameAr,
        string sku,
        decimal price,
        Guid categoryId,
        Guid brandId,
        string? descriptionEn = null,
        string? descriptionAr = null)
    {
        if (string.IsNullOrWhiteSpace(nameEn))
            throw new DomainException("English product name is required.");

        if (string.IsNullOrWhiteSpace(nameAr))
            throw new DomainException("Arabic product name is required.");

        if (string.IsNullOrWhiteSpace(sku))
            throw new DomainException("Product SKU is required.");

        if (price < 0)
            throw new DomainException("Product price cannot be negative.");

        Id = Guid.NewGuid();
        NameEn = nameEn.Trim();
        NameAr = nameAr.Trim();
        SKU = sku.Trim();
        Price = price;
        CategoryId = categoryId;
        BrandId = brandId;
        DescriptionEn = string.IsNullOrWhiteSpace(descriptionEn) ? null : descriptionEn.Trim();
        DescriptionAr = string.IsNullOrWhiteSpace(descriptionAr) ? null : descriptionAr.Trim();
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateBasicInfo(
        string nameEn,
        string nameAr,
        string sku,
        Guid categoryId,
        Guid brandId,
        string? descriptionEn = null,
        string? descriptionAr = null)
    {
        if (string.IsNullOrWhiteSpace(nameEn))
            throw new DomainException("English product name is required.");

        if (string.IsNullOrWhiteSpace(nameAr))
            throw new DomainException("Arabic product name is required.");

        if (string.IsNullOrWhiteSpace(sku))
            throw new DomainException("Product SKU is required.");

        NameEn = nameEn.Trim();
        NameAr = nameAr.Trim();
        SKU = sku.Trim();
        CategoryId = categoryId;
        BrandId = brandId;
        DescriptionEn = string.IsNullOrWhiteSpace(descriptionEn) ? null : descriptionEn.Trim();
        DescriptionAr = string.IsNullOrWhiteSpace(descriptionAr) ? null : descriptionAr.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangePrice(decimal price)
    {
        if (price < 0)
            throw new DomainException("Product price cannot be negative.");

        Price = price;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignDiscount(Guid discountId)
    {
        DiscountId = discountId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveDiscount()
    {
        DiscountId = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddImage(string imageKey)
    {
        if (string.IsNullOrWhiteSpace(imageKey))
            throw new DomainException("Product image is required.");

        imageKey = imageKey.Trim();

        if (Images.Any(x => x.ImageKey == imageKey))
            return;

        Images.Add(new ProductImage(Id, imageKey));
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveImage(string imageKey)
    {
        var image = Images.FirstOrDefault(x => x.ImageKey == imageKey);

        if (image is null)
            return;

        Images.Remove(image);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}