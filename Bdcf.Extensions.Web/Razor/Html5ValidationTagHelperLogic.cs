using Bdcf.Extensions.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Bdcf.Extensions.Web.Razor;

public class Html5ValidationTagHelperLogic
{
	public void ApplyValidationAttributes(ModelMetadata metadata, object? model, IDictionary<string, string> attributes)
	{
		ApplyRequired(metadata, attributes);
		ApplyPattern(metadata, attributes);
		ApplyNumberAttributes(metadata, attributes);
		ApplyTimeSpanAttributes(metadata, model, attributes);
	}

	private void ApplyRequired(ModelMetadata metadata, IDictionary<string, string> attributes)
	{
		if (metadata.ValidatorMetadata.OfType<RequiredAttribute>().Any())
		{
			attributes["required"] = "required";
		}
	}

	private void ApplyPattern(ModelMetadata metadata, IDictionary<string, string> attributes)
	{
		var pattern = metadata.ValidatorMetadata.OfType<RegularExpressionAttribute>().SingleOrDefault();
		if (pattern is not null)
		{
			attributes["pattern"] = pattern.Pattern;
		}
	}

	private void ApplyNumberAttributes(ModelMetadata metadata, IDictionary<string, string> attributes)
	{
		var dataType = metadata.ValidatorMetadata.OfType<DataTypeAttribute>().SingleOrDefault()?.DataType;

		if (dataType == DataType.Currency || metadata.ModelType == typeof(decimal) || metadata.ModelType == typeof(decimal?))
		{
			attributes["type"] = "number";

			var rangeAttribute = metadata.ValidatorMetadata.OfType<RangeAttribute>().FirstOrDefault();
			if (rangeAttribute != null)
			{
				attributes["min"] = rangeAttribute.Minimum.ToString()!;
				attributes["max"] = rangeAttribute.Maximum.ToString()!;
			}

			var precisionAttribute = metadata.ValidatorMetadata.OfType<PrecisionAttribute>().FirstOrDefault();
			if (precisionAttribute is not null && precisionAttribute.DecimalPlaces > 0)
			{
				attributes["step"] = $"0.{new string('0', precisionAttribute.DecimalPlaces - 1)}1";
			}
		}
	}

	private void ApplyTimeSpanAttributes(ModelMetadata metadata, object? model, IDictionary<string, string> attributes)
	{
		if (metadata.ModelType == typeof(TimeSpan) || metadata.ModelType == typeof(TimeSpan?))
		{
			if (model is not TimeSpan timeSpan) return;

			var precisionAttribute = metadata.ValidatorMetadata.OfType<PrecisionAttribute>().FirstOrDefault();
			attributes["type"] = "text";

			if (precisionAttribute is not null)
			{
				switch (precisionAttribute.TimeInterval)
				{
					case TimeInterval.Seconds:
						attributes["placeholder"] = "h:mm:ss";
						attributes["value"] = string.Format("{0}:{1:D2}:{2:D2}", (int)timeSpan.TotalHours, timeSpan.Minutes, timeSpan.Seconds);
						break;
					case TimeInterval.Minutes:
						attributes["placeholder"] = "h:mm";
						attributes["value"] = string.Format("{0}:{1:D2}", (int)timeSpan.TotalHours, timeSpan.Minutes);
						break;
					case TimeInterval.Hours:
						attributes["placeholder"] = "h";
						attributes["value"] = ((int)timeSpan.TotalHours).ToString();
						break;
				}
			}
		}
	}
}