using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Bdcf.Extensions.Web.Razor;

/// <summary>
/// A <see cref="TagHelper"/> that adds a CSS class to anchor elements when the current route matches the
/// anchor's target controller, action, and area.
/// </summary>
[HtmlTargetElement("a", Attributes = ACTIVE_ROUTE_CLASS)]
public class ActiveRouteClassTagHelper : TagHelper
{
	public const string ACTIVE_ROUTE_CLASS = "asp-active-route-class";

	private const string ACTION = "asp-action";
	private const string AREA = "asp-area";
	private const string CONTROLLER = "asp-controller";

	/// <summary>
	/// Gets or sets the current view context.
	/// </summary>
	[ViewContext]
	[HtmlAttributeNotBound]
	public ViewContext ViewContext { get; set; } = null!;

	/// <inheritdoc />
	public override void Process(TagHelperContext context, TagHelperOutput output)
	{
		ArgumentNullException.ThrowIfNull(context);
		ArgumentNullException.ThrowIfNull(output);

		output.Attributes.RemoveAll(ACTIVE_ROUTE_CLASS);

		if (!context.AllAttributes.TryGetAttribute(ACTIVE_ROUTE_CLASS, out var activeClassAttribute) ||
			activeClassAttribute.ValueStyle == HtmlAttributeValueStyle.Minimized)
		{
			return;
		}

		var activeClass = activeClassAttribute.Value?.ToString();
		if (string.IsNullOrWhiteSpace(activeClass))
		{
			return;
		}

		if (RouteMatches(context.AllAttributes))
		{
			output.AddClass(activeClass, HtmlEncoder.Default);
			output.Attributes.SetAttribute("aria-current", "page");
		}
	}

	private bool RouteMatches(ReadOnlyTagHelperAttributeList attributes)
	{
		var routeValues = ViewContext.RouteData.Values;

		return AttributeMatchesRouteValue(attributes, CONTROLLER, routeValues["controller"]?.ToString()) &&
			AttributeMatchesRouteValue(attributes, ACTION, routeValues["action"]?.ToString()) &&
			AttributeMatchesRouteValue(attributes, AREA, routeValues["area"]?.ToString());
	}

	private static bool AttributeMatchesRouteValue(ReadOnlyTagHelperAttributeList attributes, string attributeName, string? routeValue)
	{
		attributes.TryGetAttribute(attributeName, out var attribute);

		return string.Equals(
			attribute?.Value?.ToString() ?? string.Empty,
			routeValue ?? string.Empty,
			StringComparison.OrdinalIgnoreCase);
	}
}
