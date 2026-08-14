using Domain.Exceptions;

namespace Domain.ValueObjects;

public sealed record DateRange
{
    public DateOnly StartDate { get; private init; }
    public DateOnly EndDate { get; private init; }

    private DateRange()
    {
    }

    public DateRange(DateOnly startDate, DateOnly endDate)
    {
        if (endDate < startDate)
            throw new DomainException("End date cannot be before start date.");

        StartDate = startDate;
        EndDate = endDate;
    }

    public bool Contains(DateOnly date)
    {
        return date >= StartDate && date <= EndDate;
    }
}