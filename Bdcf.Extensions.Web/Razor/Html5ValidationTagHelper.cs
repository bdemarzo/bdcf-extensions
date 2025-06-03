using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

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

		var attributes = new Dictionary<string, string>();
		Html5ValidationTagHelperLogic.ApplyValidationAttributes(For.Metadata, For.Model, attributes);

		foreach (var attr in attributes)
		{
			output.Attributes.SetAttribute(attr.Key, attr.Value);
		}
	}
}

