using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Test.ValueObjects;

public class DateRangeTests
{
    private static readonly DateOnly DefaultStart = new(2026, 1, 1);
    private static readonly DateOnly DefaultEnd = new(2026, 1, 31);

    [Fact]
    public void Constructor_WithValidDates_ShouldInitializePropertiesCorrectly()
    {
        // Act
        var dateRange = new DateRange(DefaultStart, DefaultEnd);

        // Assert
        Assert.Equal(DefaultStart, dateRange.StartDate);
        Assert.Equal(DefaultEnd, dateRange.EndDate);
    }

    [Fact]
    public void Constructor_WithSameStartAndEndDate_ShouldInitializePropertiesCorrectly()
    {
        // Act
        var dateRange = new DateRange(DefaultStart, DefaultStart);

        // Assert
        Assert.Equal(DefaultStart, dateRange.StartDate);
        Assert.Equal(DefaultStart, dateRange.EndDate);
    }

    [Fact]
    public void Constructor_WithEndDateBeforeStartDate_ShouldThrowDomainException()
    {
        // Arrange
        var startDate = new DateOnly(2026, 2, 1);
        var endDate = new DateOnly(2026, 1, 31);

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => new DateRange(startDate, endDate));
        Assert.Equal("End date cannot be before start date.", ex.Message);
    }

    [Theory]
    [InlineData(2026, 1, 1)]  // Start boundary
    [InlineData(2026, 1, 15)] // Inside range
    [InlineData(2026, 1, 31)] // End boundary
    public void Contains_WhenDateIsWithinOrOnBoundaries_ShouldReturnTrue(int year, int month, int day)
    {
        // Arrange
        var dateRange = new DateRange(DefaultStart, DefaultEnd);
        var dateToCheck = new DateOnly(year, month, day);

        // Act
        var result = dateRange.Contains(dateToCheck);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData(2025, 12, 31)] // Day before start
    [InlineData(2026, 2, 1)]   // Day after end
    public void Contains_WhenDateIsOutsideRange_ShouldReturnFalse(int year, int month, int day)
    {
        // Arrange
        var dateRange = new DateRange(DefaultStart, DefaultEnd);
        var dateToCheck = new DateOnly(year, month, day);

        // Act
        var result = dateRange.Contains(dateToCheck);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValueEquality_TwoInstancesWithSameDates_ShouldBeEqual()
    {
        // Arrange
        var range1 = new DateRange(DefaultStart, DefaultEnd);
        var range2 = new DateRange(DefaultStart, DefaultEnd);

        // Assert
        Assert.Equal(range1, range2);
        Assert.True(range1 == range2);
    }
}
