using Domain.Common;
using Domain.Exceptions;

namespace Domain.Entities.ProcurementAggregate;

public sealed class Supplier : AggregateRoot
{
    public string Name { get; private set; } = null!;

    public string? ContactName { get; private set; }

    public string? Email { get; private set; }

    public string? Phone { get; private set; }

    public string? Address { get; private set; }

    public string? City { get; private set; }

    public string? TaxNumber { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public ICollection<PurchaseOrder> PurchaseOrders { get; private set; } = new List<PurchaseOrder>();

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
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Supplier name is required.");

        Name = name.Trim();
        ContactName = string.IsNullOrWhiteSpace(contactName) ? null : contactName.Trim();
        Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
        Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
        Address = string.IsNullOrWhiteSpace(address) ? null : address.Trim();
        City = string.IsNullOrWhiteSpace(city) ? null : city.Trim();
        TaxNumber = string.IsNullOrWhiteSpace(taxNumber) ? null : taxNumber.Trim();
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
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Supplier name is required.");

        Name = name.Trim();
        ContactName = string.IsNullOrWhiteSpace(contactName) ? null : contactName.Trim();
        Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
        Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
        Address = string.IsNullOrWhiteSpace(address) ? null : address.Trim();
        City = string.IsNullOrWhiteSpace(city) ? null : city.Trim();
        TaxNumber = string.IsNullOrWhiteSpace(taxNumber) ? null : taxNumber.Trim();
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