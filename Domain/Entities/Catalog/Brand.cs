using Domain.Common;
using Domain.Exceptions;

namespace Domain.Entities.Catalog;

public sealed class Brand : Entity
{
    public string NameEn { get; private set; } = null!;
    public string NameAr { get; private set; } = null!;

    public ICollection<Product> Products { get; private set; } = new List<Product>();

    private Brand() { }
    public Brand(string nameEn, string nameAr)
    {
        if (string.IsNullOrWhiteSpace(nameEn))
            throw new DomainException("English brand name is required.");

        if (string.IsNullOrWhiteSpace(nameAr))
            throw new DomainException("Arabic brand name is required.");

        Id = Guid.NewGuid();
        NameEn = nameEn.Trim();
        NameAr = nameAr.Trim();
    }

    public void Update(string? nameEn, string? nameAr)
    {
        if(nameEn is not null) NameEn = nameEn.Trim();
        if (nameAr is not null) NameAr = nameAr.Trim();
    }
}