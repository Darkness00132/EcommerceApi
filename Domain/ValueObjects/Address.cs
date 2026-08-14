using Domain.Exceptions;

namespace Domain.ValueObjects;

public sealed record Address
{
    public string Street { get; private init; }
    public string City { get; private init; }
    public string Phone { get; private init; }
    public string? Notes { get; private init; }

    private Address()
    {
        Street = null!;
        City = null!;
        Phone = null!;
    }

    public Address(
        string street,
        string city,
        string phone,
        string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new DomainException("Address is required.");

        if (string.IsNullOrWhiteSpace(city))
            throw new DomainException("City is required.");

        if (string.IsNullOrWhiteSpace(phone))
            throw new DomainException("Phone is required.");

        Street = street.Trim();
        City = city.Trim();
        Phone = phone.Trim();
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
    }
}