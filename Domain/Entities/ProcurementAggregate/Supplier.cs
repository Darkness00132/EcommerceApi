using System.ComponentModel.DataAnnotations;
using Domain.Common;
using Domain.Exceptions;

namespace Domain.Entities.ProcurementAggregate;

public sealed class Supplier : AggregateRoot
{
    [MaxLength(200)]
    public string Name { get; private set; } = null!;

    [MaxLength(150)]
    public string? ContactName { get; private set; }

    [MaxLength(256)]
    public string? Email { get; private set; }

    [MaxLength(20)]
    public string? Phone { get; private set; }

    [MaxLength(250)]
    public string? Address { get; private set; }

    [MaxLength(100)]
    public string? City { get; private set; }

    [MaxLength(100)]
    public string? TaxNumber { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public ICollection<PurchaseOrder> PurchaseOrders { get; private set; }
        = new List<PurchaseOrder>();

    private Supplier() { }

    public Supplier(
        string name,
        string? contactName = null,
        string? email = null,
        string? phone = null,
        string? address = null,
        string? city = null,
        string? taxNumber = null)
        : base(Guid.NewGuid())
    {
        Name = ValidateRequiredText(name, 200, "Supplier name");
        ContactName = ValidateOptionalText(contactName, 150, "Contact name");
        Email = ValidateOptionalText(email, 256, "Email");
        Phone = ValidateOptionalText(phone, 20, "Phone");
        Address = ValidateOptionalText(address, 250, "Address");
        City = ValidateOptionalText(city, 100, "City");
        TaxNumber = ValidateOptionalText(taxNumber, 100, "Tax number");

        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(
        string name,
        string? contactName = null,
        string? email = null,
        string? phone = null,
        string? address = null,
        string? city = null,
        string? taxNumber = null)
    {
        Name = ValidateRequiredText(name, 200, "Supplier name");
        ContactName = ValidateOptionalText(contactName, 150, "Contact name");
        Email = ValidateOptionalText(email, 256, "Email");
        Phone = ValidateOptionalText(phone, 20, "Phone");
        Address = ValidateOptionalText(address, 250, "Address");
        City = ValidateOptionalText(city, 100, "City");
        TaxNumber = ValidateOptionalText(taxNumber, 100, "Tax number");

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

    private static string ValidateRequiredText(
        string value,
        int maxLength,
        string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException($"{fieldName} is required.");

        var trimmedValue = value.Trim();

        if (trimmedValue.Length > maxLength) {
            throw new DomainException(
                $"{fieldName} cannot exceed {maxLength} characters.");
        }

        return trimmedValue;
    }

    private static string? ValidateOptionalText(
        string? value,
        int maxLength,
        string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var trimmedValue = value.Trim();

        if (trimmedValue.Length > maxLength) {
            throw new DomainException(
                $"{fieldName} cannot exceed {maxLength} characters.");
        }

        return trimmedValue;
    }
}
