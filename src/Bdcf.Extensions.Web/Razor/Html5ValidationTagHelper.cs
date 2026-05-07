using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Bdcf.Extensions.Web.Razor;

/// <summary>
/// A <see cref="TagHelper"/> that applies HTML5 validation attributes to input and textarea elements based on the
/// metadata of the model property specified by the <c>asp-for</c> attribute.
/// </summary>
/// <remarks>This tag helper automatically adds validation attributes such as <c>required</c>, <c>maxlength</c>,
/// and <c>pattern</c> to the rendered HTML elements, ensuring that client-side validation is consistent with the
/// server-side model validation.</remarks>
[HtmlTargetElement("input", Attributes = "asp-for")]
[HtmlTargetElement("textarea", Attributes = "asp-for")]
public class Html5ValidationTagHelper : TagHelper
{
	/// <summary>
	/// Gets or sets the <see cref="ModelExpression"/> that identifies the model property to be rendered.
	/// </summary>
	[HtmlAttributeName("asp-for")]
	public required ModelExpression For { get; set; }

	/// <summary>
	/// Processes the current tag helper to apply HTML5 validation attributes based on the specified model metadata.
	/// </summary>
	/// <remarks>This method adds HTML5 validation attributes to the tag based on the metadata and model associated
	/// with the <see cref="For"/> property. The attributes are applied to ensure client-side validation is consistent with
	/// the model's validation rules.</remarks>
	/// <param name="context">The <see cref="TagHelperContext"/> containing information about the current tag helper execution context.</param>
	/// <param name="output">The <see cref="TagHelperOutput"/> used to modify the output of the tag helper.</param>
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

