using Bdcf.Extensions.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.ComponentModel.DataAnnotations;

namespace Bdcf.Extensions.Web.Razor;

[HtmlTargetElement("input", Attributes = "asp-for")]
[HtmlTargetElement("textarea", Attributes = "asp-for")]
public class Html5ValidationTagHelper : TagHelper
{
	[HtmlAttributeName("asp-for")]
	public required ModelExpression For { get; set; }

	public override void Process(TagHelperContext context, TagHelperOutput output)
	{
		base.Process(context, output);

		var dataType = For.Metadata.ValidatorMetadata.OfType<DataTypeAttribute>().SingleOrDefault()?.DataType;

		if (For.Metadata.ValidatorMetadata.OfType<RequiredAttribute>().Any())
		{
			output.Attributes.SetAttribute("required", "required");
		}

		var pattern = For.Metadata.ValidatorMetadata.OfType<RegularExpressionAttribute>().SingleOrDefault();
		if (pattern is not null)
		{
			output.Attributes.SetAttribute("pattern", pattern.Pattern);
		}
		else
		{
			var regex = For.Metadata.ValidatorMetadata.OfType<RegularExpressionAttribute>().SingleOrDefault();
			if (regex is not null)
			{
				output.Attributes.SetAttribute("pattern", regex.Pattern);
			}
		}

		if (dataType == DataType.Currency || For.Metadata.ModelType == typeof(decimal) || For.Metadata.ModelType == typeof(decimal?))
		{
			output.Attributes.SetAttribute("type", "number");

			var rangeAttribute = For.Metadata.ValidatorMetadata.OfType<RangeAttribute>().FirstOrDefault();
			if (rangeAttribute != null)
			{
				output.Attributes.SetAttribute("min", rangeAttribute.Minimum.ToString());
				output.Attributes.SetAttribute("max", rangeAttribute.Maximum.ToString());
			}

			var precisionAttribute = For.Metadata.ValidatorMetadata.OfType<PrecisionAttribute>().FirstOrDefault();
			if (precisionAttribute is not null)
			{
				if (precisionAttribute.DecimalPlaces > 0)
				{
					output.Attributes.SetAttribute("step", $"0.{new string('0', precisionAttribute.DecimalPlaces - 1)}1");
				}
			}
		}

		if (For.Metadata.ModelType == typeof(TimeSpan) || For.Metadata.ModelType == typeof(TimeSpan?))
		{
			var timeSpan = (TimeSpan)For.Model;
			var precisionAttribute = For.Metadata.ValidatorMetadata.OfType<PrecisionAttribute>().FirstOrDefault();

			output.Attributes.SetAttribute("type", "text");
			if (precisionAttribute is not null)
			{
				switch (precisionAttribute.TimeInterval)
				{
					case TimeInterval.Seconds:
						output.Attributes.SetAttribute("placeholder", "h:mm:ss");
						output.Attributes.SetAttribute("value", string.Format("{0}:{1:D2}:{2:D2}", (int)timeSpan.TotalHours, timeSpan.Minutes, timeSpan.Seconds));
						break;
					case TimeInterval.Minutes:
						output.Attributes.SetAttribute("placeholder", "h:mm");
						output.Attributes.SetAttribute("value", string.Format("{0}:{1:D2}", (int)timeSpan.TotalHours, timeSpan.Minutes));
						break;
					case TimeInterval.Hours:
						output.Attributes.SetAttribute("placeholder", "h");
						output.Attributes.SetAttribute("value", string.Format("{0}", (int)timeSpan.TotalHours));
						break;
				}
			}
		}
	}
}
