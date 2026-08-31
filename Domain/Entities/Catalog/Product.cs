using System.ComponentModel.DataAnnotations;
using Domain.Common;
using Domain.Entities.InventoryAggregate;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities.Catalog;

public sealed class Product : AggregateRoot
{
    private readonly List<ProductImage> _images = new();

    [MaxLength(200)]
    public string NameEn { get; private set; } = null!;

    [MaxLength(200)]
    public string NameAr { get; private set; } = null!;

    [MaxLength(100)]
    public string SKU { get; private set; } = null!;

    [MaxLength(2000)]
    public string DescriptionEn { get; private set; } = null!;

    [MaxLength(2000)]
    public string DescriptionAr { get; private set; } = null!;

    [Precision(18, 2)]
    public decimal Price { get; private set; }

    public bool IsActive { get; private set; }

    public Guid CategoryId { get; private set; }
    public Guid BrandId { get; private set; }
    public Guid? DiscountId { get; private set; }

    public Category Category { get; private set; } = null!;
    public Brand Brand { get; private set; } = null!;
    public Discount? Discount { get; private set; }
    public Inventory Inventory { get; private set; } = null!;

    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    [Timestamp]
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();

    private Product() { } // Required for EF Core

    public Product(
        string nameEn,
        string nameAr,
        string descriptionEn,
        string descriptionAr,
        string sku,
        decimal price,
        Guid categoryId,
        Guid brandId)
    {
        UpdateDetails(nameEn, nameAr, descriptionEn, descriptionAr, sku);
        ChangePrice(price);
        SetCategory(categoryId);
        SetBrand(brandId);

        Id = Guid.NewGuid();
        IsActive = false;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(
        string nameEn,
        string nameAr,
        string descriptionEn,
        string descriptionAr,
        string sku)
    {
        if (string.IsNullOrWhiteSpace(nameEn))
            throw new DomainException("English product name is required.");

        if (string.IsNullOrWhiteSpace(nameAr))
            throw new DomainException("Arabic product name is required.");

        if (string.IsNullOrWhiteSpace(descriptionEn))
            throw new DomainException("English product description is required.");

        if (string.IsNullOrWhiteSpace(descriptionAr))
            throw new DomainException("Arabic product description is required.");

        if (string.IsNullOrWhiteSpace(sku))
            throw new DomainException("Product SKU is required.");

        NameEn = nameEn.Trim();
        NameAr = nameAr.Trim();
        DescriptionEn = descriptionEn.Trim();
        DescriptionAr = descriptionAr.Trim();
        SKU = sku.Trim();
        Touch();
    }

    public void ChangePrice(decimal price)
    {
        if (price < 0)
            throw new DomainException("Product price cannot be negative.");

        Price = price;
        Touch();
    }

    public void SetCategory(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
            throw new DomainException("Category ID cannot be empty.");

        CategoryId = categoryId;
        Touch();
    }

    public void SetBrand(Guid brandId)
    {
        if (brandId == Guid.Empty)
            throw new DomainException("Brand ID cannot be empty.");

        BrandId = brandId;
        Touch();
    }

    public void AssignDiscount(Guid discountId)
    {
        if (discountId == Guid.Empty)
            throw new DomainException("Discount ID cannot be empty.");

        DiscountId = discountId;
        Touch();
    }

    public void RemoveDiscount()
    {
        DiscountId = null;
        Touch();
    }

    public void AddImage(string imageKey)
    {
        if (string.IsNullOrWhiteSpace(imageKey))
            throw new DomainException("Product image is required.");

        var trimmedKey = imageKey.Trim();

        if (_images.Any(x => x.ImageKey == trimmedKey))
            return;

        _images.Add(new ProductImage(Id, trimmedKey));
        Touch();
    }

    public void RemoveImage(string imageKey)
    {
        if (string.IsNullOrWhiteSpace(imageKey)) return;

        var trimmedKey = imageKey.Trim();
        var image = _images.FirstOrDefault(x => x.ImageKey == trimmedKey);

        if (image is null)
            return;

        _images.Remove(image);
        Touch();
    }

    public void Activate()
    {
        IsActive = true;
        Touch();
    }

    public void Deactivate()
    {
        IsActive = false;
        Touch();
    }

    internal void SetInventory(Inventory inventory)
    {
        Inventory = inventory ?? throw new DomainException("Inventory cannot be null.");
    }

    private void Touch()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
