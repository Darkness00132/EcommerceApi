using Domain.Exceptions;

namespace Domain.ValueObjects;

public sealed record FullName
{
    public string FirstName { get; private init; }
    public string LastName { get; private init; }

    private FullName()
    {
        FirstName = null!;
        LastName = null!;
    }

    public FullName(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("First name is required.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Last name is required.");

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
    }

    public override string ToString()
    {
        return $"{FirstName} {LastName}";
    }
}