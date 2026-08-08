using Bdcf.Extensions.Web.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;

namespace Bdcf.Extensions.Web.Razor.Tests;

public class ActiveRouteClassTagHelperTests
{
	[Fact]
	public void Process_AddsActiveClassAndAriaCurrent_WhenRouteMatches()
	{
		var tagHelper = CreateTagHelper(controller: "Songs", action: "Index");
		var context = CreateContext(
			new TagHelperAttribute("asp-controller", "Songs"),
			new TagHelperAttribute("asp-action", "Index"),
			new TagHelperAttribute(ActiveRouteClassTagHelper.ACTIVE_ROUTE_CLASS, "active"));
		var output = CreateOutput(context);

		tagHelper.Process(context, output);

		Assert.Equal("nav-link active", output.Attributes["class"].Value?.ToString());
		Assert.Equal("page", output.Attributes["aria-current"].Value);
		Assert.Null(output.Attributes[ActiveRouteClassTagHelper.ACTIVE_ROUTE_CLASS]);
	}

	[Fact]
	public void Process_DoesNotAddActiveClass_WhenActiveRouteClassIsMinimized()
	{
		var tagHelper = CreateTagHelper(controller: "Songs", action: "Index");
		var context = CreateContext(
			new TagHelperAttribute("asp-controller", "Songs"),
			new TagHelperAttribute("asp-action", "Index"),
			new TagHelperAttribute(
				ActiveRouteClassTagHelper.ACTIVE_ROUTE_CLASS,
				value: null,
				valueStyle: HtmlAttributeValueStyle.Minimized));
		var output = CreateOutput(context);

		tagHelper.Process(context, output);

		Assert.Equal("nav-link", output.Attributes["class"].Value);
		Assert.Null(output.Attributes["aria-current"]);
		Assert.Null(output.Attributes[ActiveRouteClassTagHelper.ACTIVE_ROUTE_CLASS]);
	}

	[Theory]
	[InlineData("")]
	[InlineData(" ")]
	public void Process_DoesNotAddActiveClass_WhenActiveRouteClassIsBlank(string activeClass)
	{
		var tagHelper = CreateTagHelper(controller: "Songs", action: "Index");
		var context = CreateContext(
			new TagHelperAttribute("asp-controller", "Songs"),
			new TagHelperAttribute("asp-action", "Index"),
			new TagHelperAttribute(ActiveRouteClassTagHelper.ACTIVE_ROUTE_CLASS, activeClass));
		var output = CreateOutput(context);

		tagHelper.Process(context, output);

		Assert.Equal("nav-link", output.Attributes["class"].Value);
		Assert.Null(output.Attributes["aria-current"]);
		Assert.Null(output.Attributes[ActiveRouteClassTagHelper.ACTIVE_ROUTE_CLASS]);
	}

	[Fact]
	public void Process_DoesNotAddActiveClass_WhenControllerDoesNotMatch()
	{
		var tagHelper = CreateTagHelper(controller: "Songs", action: "Index");
		var context = CreateContext(
			new TagHelperAttribute("asp-controller", "Bands"),
			new TagHelperAttribute("asp-action", "Index"),
			new TagHelperAttribute(ActiveRouteClassTagHelper.ACTIVE_ROUTE_CLASS, "active"));
		var output = CreateOutput(context);

		tagHelper.Process(context, output);

		Assert.Equal("nav-link", output.Attributes["class"].Value);
		Assert.Null(output.Attributes["aria-current"]);
	}

	[Fact]
	public void Process_DoesNotAddActiveClass_WhenActionDoesNotMatch()
	{
		var tagHelper = CreateTagHelper(controller: "Products", action: "Details");
		var context = CreateContext(
			new TagHelperAttribute("asp-controller", "Products"),
			new TagHelperAttribute("asp-action", "Edit"),
			new TagHelperAttribute(ActiveRouteClassTagHelper.ACTIVE_ROUTE_CLASS, "active"));
		var output = CreateOutput(context);

		tagHelper.Process(context, output);

		Assert.Equal("nav-link", output.Attributes["class"].Value);
		Assert.Null(output.Attributes["aria-current"]);
	}

	[Fact]
	public void Process_AddsActiveClass_WhenAreaMatches()
	{
		var tagHelper = CreateTagHelper(controller: "Songs", action: "Index", area: "Admin");
		var context = CreateContext(
			new TagHelperAttribute("asp-area", "Admin"),
			new TagHelperAttribute("asp-controller", "Songs"),
			new TagHelperAttribute("asp-action", "Index"),
			new TagHelperAttribute(ActiveRouteClassTagHelper.ACTIVE_ROUTE_CLASS, "active"));
		var output = CreateOutput(context);

		tagHelper.Process(context, output);

		Assert.Equal("nav-link active", output.Attributes["class"].Value?.ToString());
		Assert.Equal("page", output.Attributes["aria-current"].Value);
	}

	[Fact]
	public void Process_DoesNotAddActiveClass_WhenCurrentAreaIsMissingAndTargetAreaIsProvided()
	{
		var tagHelper = CreateTagHelper(controller: "Songs", action: "Index");
		var context = CreateContext(
			new TagHelperAttribute("asp-area", "Admin"),
			new TagHelperAttribute("asp-controller", "Songs"),
			new TagHelperAttribute("asp-action", "Index"),
			new TagHelperAttribute(ActiveRouteClassTagHelper.ACTIVE_ROUTE_CLASS, "active"));
		var output = CreateOutput(context);

		tagHelper.Process(context, output);

		Assert.Equal("nav-link", output.Attributes["class"].Value);
		Assert.Null(output.Attributes["aria-current"]);
	}

	[Fact]
	public void Process_DoesNotAddActiveClass_WhenCurrentAreaIsProvidedAndTargetAreaIsMissing()
	{
		var tagHelper = CreateTagHelper(controller: "Songs", action: "Index", area: "Admin");
		var context = CreateContext(
			new TagHelperAttribute("asp-controller", "Songs"),
			new TagHelperAttribute("asp-action", "Index"),
			new TagHelperAttribute(ActiveRouteClassTagHelper.ACTIVE_ROUTE_CLASS, "active"));
		var output = CreateOutput(context);

		tagHelper.Process(context, output);

		Assert.Equal("nav-link", output.Attributes["class"].Value);
		Assert.Null(output.Attributes["aria-current"]);
	}

	private static ActiveRouteClassTagHelper CreateTagHelper(string controller, string action, string? area = null)
	{
		var routeData = new RouteData();
		routeData.Values["controller"] = controller;
		routeData.Values["action"] = action;
		if (area is not null)
		{
			routeData.Values["area"] = area;
		}

		return new ActiveRouteClassTagHelper
		{
			ViewContext = new ViewContext
			{
				RouteData = routeData,
			},
		};
	}

	private static TagHelperContext CreateContext(params TagHelperAttribute[] attributes)
	{
		return new TagHelperContext(
			new TagHelperAttributeList(attributes),
			new Dictionary<object, object>(),
			uniqueId: "test");
	}

	private static TagHelperOutput CreateOutput(TagHelperContext context)
	{
		var attributes = new TagHelperAttributeList(context.AllAttributes)
		{
			new("class", "nav-link"),
		};

		return new TagHelperOutput(
			"a",
			attributes,
			(_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));
	}
}
