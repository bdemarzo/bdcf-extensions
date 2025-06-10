using System.ComponentModel.DataAnnotations;

namespace Bdcf.Extensions.DataAnnotations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public class PrecisionAttribute : ValidationAttribute
{
	public int DecimalPlaces { get; set; } = -1;

	public TimeSpanPrecision TimeSpanPrecision { get; set; } = TimeSpanPrecision.None;

	protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
	{
		if (value is null)
			return ValidationResult.Success;

		var property = validationContext.ObjectType.GetProperty(validationContext.MemberName!);
		if (property is null)
			return new ValidationResult($"Property '{validationContext.MemberName}' not found on type '{validationContext.ObjectType.Name}'.");

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

			TimeSpan truncated = timeSpan;
			switch (TimeSpanPrecision)
			{
				case TimeSpanPrecision.Hours:
					truncated = new TimeSpan(timeSpan.Hours, 0, 0);
					break;
				case TimeSpanPrecision.Minutes:
					truncated = new TimeSpan(timeSpan.Hours, timeSpan.Minutes, 0);
					break;
				case TimeSpanPrecision.Seconds:
					truncated = new TimeSpan(timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
					break;
			}
			property.SetValue(validationContext.ObjectInstance, truncated);

			return ValidationResult.Success;
		}
		else
		{
			return new ValidationResult("This attribute can only be used with decimal and TimeSpan values.");
		}
	}
}
