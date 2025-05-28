using System.ComponentModel.DataAnnotations;

namespace Bdcf.Extensions.DataAnnotations.Tests;

public class PrecisionAttributeTests
{
	private class DecimalTestModel
	{
		[Precision(2)]
		public decimal Value { get; set; }
	}

	private class TimeSpanTestModel
	{
		[Precision(TimeInterval.Minutes)]
		public TimeSpan Value { get; set; }
	}

	[Theory]
	[InlineData(123.4567, 123.45, 2)]
	[InlineData(78.999, 78.99, 2)]
	[InlineData(10.555, 10.55, 2)]
	[InlineData(50.6789, 50.678, 3)]
	[InlineData(100, 100, 2)]
	[InlineData(null, null, 2)]
	public void ValidationShouldTruncateDecimalPlaces(object? input, object? expected, int decimalPlaces)
	{
		decimal? inputValue = input as decimal?;
		decimal? expectedValue = expected as decimal?;

		var attribute = new PrecisionAttribute(decimalPlaces);
		var model = new DecimalTestModel { Value = inputValue ?? 0m };
		var context = new ValidationContext(model) { MemberName = nameof(model.Value) };

		var result = attribute.GetValidationResult(inputValue, context)!;

		Assert.Equal(ValidationResult.Success, result);
		Assert.Equal(expectedValue ?? 0m, model.Value);
	}

    [Theory]
    [InlineData(1, 30, 45, TimeInterval.Hours, 1, 0, 0)]
    [InlineData(2, 45, 59, TimeInterval.Minutes, 2, 45, 0)]
    [InlineData(3, 15, 20, TimeInterval.Seconds, 3, 15, 20)]
    [InlineData(0, 0, 0, TimeInterval.Minutes, 0, 0, 0)]
    [InlineData(null, null, null, TimeInterval.Minutes, null, null, null)]
	public void ValidationShouldAdjustTimeSpanPrecision(
        int? hours, int? minutes, int? seconds,
        TimeInterval precision,
        int? expectedHours, int? expectedMinutes, int? expectedSeconds)
    {
        TimeSpan? input = hours.HasValue ? new TimeSpan(hours.Value, minutes!.Value, seconds!.Value) : (TimeSpan?)null;
        var expected = expectedHours.HasValue ? new TimeSpan(expectedHours.Value, expectedMinutes!.Value, expectedSeconds!.Value) : (TimeSpan?)null;

        var attribute = new PrecisionAttribute(precision);
        var model = new TimeSpanTestModel { Value = input ?? TimeSpan.Zero };
		var context = new ValidationContext(model) { MemberName = nameof(model.Value) };

        var result = attribute.GetValidationResult(input, context);

        // Assert
        Assert.Equal(ValidationResult.Success, result);
        Assert.Equal(expected ?? TimeSpan.Zero, model.Value);
    }
}
