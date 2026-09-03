using Domain.Exceptions;
using Domain.ValueObjects;
using FluentAssertions;

namespace Domain.Test.ValueObjects;

public class DateRangeTests
{
    [Fact]
    public void A_Date_Range_Can_Be_Created_When_The_End_Date_Is_Not_Before_The_Start_Date()
    {
        // Arrange
        var startDate = new DateOnly(2026, 9, 1);
        var endDate = new DateOnly(2026, 9, 7);

        // Act
        var dateRange = new DateRange(startDate, endDate);

        // Assert
        dateRange.StartDate.Should().Be(startDate);
        dateRange.EndDate.Should().Be(endDate);
    }

    [Fact]
    public void A_Date_Range_Cannot_End_Before_It_Starts()
    {
        // Arrange
        var startDate = new DateOnly(2026, 9, 7);
        var endDate = new DateOnly(2026, 9, 1);

        // Act
        var act = () => new DateRange(startDate, endDate);

        // Assert
        act.Should()
            .Throw<DomainException>()
            .WithMessage("End date cannot be before start date.");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(7)]
    public void A_Date_Within_The_Range_Is_Valid(int day)
    {
        // Arrange
        var dateRange = new DateRange(
            new DateOnly(2026, 9, 1),
            new DateOnly(2026, 9, 7));

        // Act
        var result = dateRange.IsValidOn(
            new DateOnly(2026, 9, day));

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(31, 8)]
    [InlineData(8, 9)]
    public void A_Date_Outside_The_Range_Is_Not_Valid(int day, int month)
    {
        // Arrange
        var dateRange = new DateRange(
            new DateOnly(2026, 9, 1),
            new DateOnly(2026, 9, 7));

        // Act
        var result = dateRange.IsValidOn(
            new DateOnly(2026, month, day));

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void A_Date_Range_Can_Start_And_End_On_The_Same_Day()
    {
        // Arrange & Act
        var date = new DateOnly(2026, 9, 1);
        var dateRange = new DateRange(date, date);

        // Assert
        dateRange.IsValidOn(date).Should().BeTrue();
    }
}
