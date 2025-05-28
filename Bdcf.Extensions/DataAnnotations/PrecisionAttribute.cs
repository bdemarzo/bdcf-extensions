using System.ComponentModel.DataAnnotations;

namespace Bdcf.Extensions.DataAnnotations;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public class PrecisionAttribute : ValidationAttribute
{
	public int DecimalPlaces { get; } = 0;
	public TimeInterval TimeInterval { get; } = TimeInterval.Seconds;

	public PrecisionAttribute(int decimalPlaces)
	{
		DecimalPlaces = decimalPlaces;
	}

	public PrecisionAttribute(TimeInterval timeInterval)
	{
		TimeInterval = timeInterval;
	}

	protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
	{
		if (value is null)
		{
			return ValidationResult.Success;
		}

		if (value is decimal decimalValue)
		{
			var multiplier = (decimal)Math.Pow(10, DecimalPlaces);
			var truncatedValue = Math.Truncate(decimalValue * multiplier) / multiplier;

			// Use reflection to set the truncated value back to the property
			var property = validationContext.ObjectType.GetProperty(validationContext.MemberName!);
			if (property is not null && property.CanWrite)
			{
				property.SetValue(validationContext.ObjectInstance, truncatedValue);
			}

			return ValidationResult.Success;
		}
		else if (value is TimeSpan timeSpan)
		{
			var adjustedTimeSpan = timeSpan;

			switch (TimeInterval)
			{
				case TimeInterval.Hours:
					adjustedTimeSpan = new TimeSpan(timeSpan.Hours, 0, 0);
					break;
				case TimeInterval.Minutes:
					adjustedTimeSpan = new TimeSpan(timeSpan.Hours, timeSpan.Minutes, 0);
					break;
				case TimeInterval.Seconds:
					// No need to adjust for seconds precision
					break;
			}

			var property = validationContext.ObjectType.GetProperty(validationContext.MemberName!);
			if (property is not null && property.CanWrite)
			{
				property.SetValue(validationContext.ObjectInstance, adjustedTimeSpan);
			}

			return ValidationResult.Success;
		}
		else
		{
			return new ValidationResult("This attribute can only be used with decimal and TimeSpan values.");
		}
	}
}
