using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Bdcf.Extensions.Web.Razor;

/// <summary>
/// <see cref="ITagHelper"/> implementation targeting any element that include include-if or 
/// exclude-if elements.  
/// </summary>
[HtmlTargetElement(Attributes = $"{INCLUDE_IF}")]
[HtmlTargetElement(Attributes = $"{EXCLUDE_IF}")]
public class IfAttributeTagHelper : TagHelper
{
	public const string INCLUDE_IF = "include-if";
	public const string EXCLUDE_IF = "exclude-if";

	/// <inheritdoc />
	public override int Order => -1000;

	/// <summary>
	/// A value indicating whether to render the content inside the element.
	/// If <see cref="Exclude"/> is also true, the content will not be rendered.
	/// </summary>
	[HtmlAttributeName(INCLUDE_IF)]
	public bool? Include { get; set; }

	/// <summary>
	/// A value indicating whether to render the content inside the element.
	/// If <see cref="Exclude"/> is also true, the content will not be rendered.
	/// </summary>
	[HtmlAttributeName(EXCLUDE_IF)]
	public bool? Exclude { get; set; }

	/// <inheritdoc />
	public override void Process(TagHelperContext context, TagHelperOutput output)
	{
		ArgumentNullException.ThrowIfNull(context);
		ArgumentNullException.ThrowIfNull(output);

		output.Attributes.RemoveAll(INCLUDE_IF);
		output.Attributes.RemoveAll(EXCLUDE_IF);

		if (DontRender)
		{
			output.TagName = null;
			output.SuppressOutput();
		}
	}

	private bool DontRender => !(Include ?? true) || (Exclude ?? false);
}
