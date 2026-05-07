using System.ComponentModel.DataAnnotations;

namespace Bdcf.Extensions.DataAnnotations;

/// <summary>
/// Validation attribute that normalizes decimal and <see cref="TimeSpan"/> values to a configured precision.
/// </summary>
/// <remarks>
/// This attribute mutates the validated model property by assigning the truncated value during validation. It is intended
/// for normalization, not pure validation.
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public class PrecisionAttribute : ValidationAttribute
{
	/// <summary>
	/// Gets or sets the number of decimal places to preserve. A negative value disables decimal truncation.
	/// </summary>
	public int DecimalPlaces { get; set; } = -1;

	/// <summary>
	/// Gets or sets the precision to preserve for <see cref="TimeSpan"/> values.
	/// </summary>
	public TimeSpanPrecision TimeSpanPrecision { get; set; } = TimeSpanPrecision.None;

	/// <inheritdoc />
	protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
	{
		if (value is null)
			return ValidationResult.Success;

		var property = validationContext.ObjectType.GetProperty(validationContext.MemberName!);
		if (property is null)
			return new ValidationResult($"Property '{validationContext.MemberName}' not found on type '{validationContext.ObjectType.Name}'.");
		if (property.CanWrite == false)
			return new ValidationResult($"Property '{validationContext.MemberName}' can not use PrecisionAttribute as it does not have a setter.");

		if (value is decimal decimalValue && DecimalPlaces >= 0)
		{
			var truncated = Math.Truncate(decimalValue * (decimal)Math.Pow(10, DecimalPlaces)) / (decimal)Math.Pow(10, DecimalPlaces);
			property.SetValue(validationContext.ObjectInstance, truncated);

			return ValidationResult.Success;
		}
		else if (value is TimeSpan timeSpan)
		{
			if (TimeSpanPrecision == TimeSpanPrecision.None)
				return ValidationResult.Success; // No truncation needed

			TimeSpan truncated = TruncateTimeSpan(timeSpan, TimeSpanPrecision);
			property.SetValue(validationContext.ObjectInstance, truncated);

			return ValidationResult.Success;
		}
		else
		{
			return new ValidationResult("This attribute can only be used with decimal and TimeSpan values.");
		}
	}

	private static TimeSpan TruncateTimeSpan(TimeSpan timeSpan, TimeSpanPrecision precision)
	{
		long unitTicks = precision switch
		{
			TimeSpanPrecision.Hours => TimeSpan.TicksPerHour,
			TimeSpanPrecision.Minutes => TimeSpan.TicksPerMinute,
			TimeSpanPrecision.Seconds => TimeSpan.TicksPerSecond,
			_ => 1
		};

		return TimeSpan.FromTicks(timeSpan.Ticks - (timeSpan.Ticks % unitTicks));
	}
}
