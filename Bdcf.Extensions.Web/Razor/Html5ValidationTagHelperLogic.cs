using Bdcf.Extensions.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Bdcf.Extensions.Web.Razor;

/// <summary>
/// Provides logic for applying HTML5 validation attributes to form elements based on model metadata.
/// </summary>
/// <remarks>This class contains methods to add appropriate HTML5 validation attributes, such as <c>required</c>,
/// <c>pattern</c>, <c>min</c>, <c>max</c>, and <c>step</c>, to a dictionary of attributes. These attributes are derived
/// from the metadata and validation attributes associated with a model property.</remarks>
public static class Html5ValidationTagHelperLogic
{
	/// <summary>
	/// Applies validation-related attributes to the specified HTML attributes dictionary based on the provided model
	/// metadata and model instance.
	/// </summary>
	/// <remarks>This method processes the model metadata and applies validation attributes such as required fields,
	/// patterns, numeric constraints, and time span constraints to the provided HTML attributes dictionary. The resulting
	/// attributes can be used to render client-side validation rules in an HTML form.</remarks>
	/// <param name="metadata">The metadata describing the model's properties and validation rules.</param>
	/// <param name="model">The model instance to which the validation attributes apply. Can be <see langword="null"/>.</param>
	/// <param name="attributes">A dictionary of HTML attributes to which validation attributes will be added or updated.</param>
	public static void ApplyValidationAttributes(ModelMetadata metadata, object? model, IDictionary<string, string> attributes)
	{
		ApplyRequired(metadata, attributes);
		ApplyPattern(metadata, attributes);
		ApplyNumberAttributes(metadata, attributes);
		ApplyTimeSpanAttributes(metadata, model, attributes);
	}

	private static void ApplyRequired(ModelMetadata metadata, IDictionary<string, string> attributes)
	{
		if (metadata.ValidatorMetadata.OfType<RequiredAttribute>().Any())
		{
			attributes["required"] = "required";
		}
	}

	private static void ApplyPattern(ModelMetadata metadata, IDictionary<string, string> attributes)
	{
		var pattern = metadata.ValidatorMetadata.OfType<RegularExpressionAttribute>().SingleOrDefault();
		if (pattern is not null)
		{
			attributes["pattern"] = pattern.Pattern;
		}
	}

	private static void ApplyNumberAttributes(ModelMetadata metadata, IDictionary<string, string> attributes)
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

	private static void ApplyTimeSpanAttributes(ModelMetadata metadata, object? model, IDictionary<string, string> attributes)
	{
		if (metadata.ModelType == typeof(TimeSpan) || metadata.ModelType == typeof(TimeSpan?))
		{
			if (model is not TimeSpan timeSpan)
			{
				return;
			}

			var precisionAttribute = metadata.ValidatorMetadata.OfType<PrecisionAttribute>().FirstOrDefault();
			attributes["type"] = "text";

			if (precisionAttribute is not null)
			{
				switch (precisionAttribute.TimeSpanPrecision)
				{
					case TimeSpanPrecision.Seconds:
						attributes["placeholder"] = "h:mm:ss";
						attributes["value"] = string.Format("{0}:{1:D2}:{2:D2}", (int)timeSpan.TotalHours, timeSpan.Minutes, timeSpan.Seconds);
						break;
					case TimeSpanPrecision.Minutes:
						attributes["placeholder"] = "h:mm";
						attributes["value"] = string.Format("{0}:{1:D2}", (int)timeSpan.TotalHours, timeSpan.Minutes);
						break;
					case TimeSpanPrecision.Hours:
						attributes["placeholder"] = "h";
						attributes["value"] = ((int)timeSpan.TotalHours).ToString();
						break;
				}
			}
		}
	}
}
