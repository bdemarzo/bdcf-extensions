using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bdcf.Extensions.Web.Mvc;

public static class HtmlHelperExtensions
{
	/// <summary>
	/// Fluent extension method for <see cref="IHtmlHelper"/> to conditionally render content based on a boolean condition.
	/// </summary>
	/// <param name="htmlHelper">The HTML helper.</param>
	/// <param name="condition">The condition.</param>
	/// <returns>The condition, or false if the condition is null.</returns>
	public static bool If(this IHtmlHelper htmlHelper, bool? condition)
	{
		return condition ?? false;
	}

	/// <summary>
	/// Fluent extension method for <see cref="IHtmlHelper"/> to conditionally render content based on a boolean condition.
	/// </summary>
	/// <param name="htmlHelper">The HTML helper.</param>
	/// <param name="condition">The condition.</param>
	/// <returns>The not of the condition.</returns>
	public static bool IfNot(this IHtmlHelper htmlHelper, bool condition)
	{
		return !condition;
	}

	/// <summary>
	/// Fluent extension method for <see cref="IHtmlHelper"/> to conditionally render content based on whether an object is not null.
	/// </summary>
	/// <param name="htmlHelper">The HTML helper.</param>
	/// <param name="obj">The object.</param>
	/// <returns>True if the object is not null, false otherwise.</returns>
	public static bool IfHasValue(this IHtmlHelper htmlHelper, object obj)
	{
		return obj is not null;
	}

	/// <summary>
	/// Fluent extension method for <see cref="IHtmlHelper"/> to conditionally render content based on whether a string is not null or whitespace.
	/// </summary>
	/// <param name="htmlHelper">The HTML helper.</param>
	/// <param name="str">The string.</param>
	/// <returns>True if the string is not null and not whitespace, false otherwise.</returns>
	public static bool IfHasValue(this IHtmlHelper htmlHelper, string? str)
	{
		return !string.IsNullOrWhiteSpace(str);
	}

	/// <summary>
	/// Fluent extension method for <see cref="IHtmlHelper"/> to conditionally render content based on whether an object is null.
	/// </summary>
	/// <param name="htmlHelper">The HTML helper.</param>
	/// <param name="obj">The object.</param>
	/// <returns>True if the object is null, false otherwise.</returns>
	public static bool IfNull(this IHtmlHelper htmlHelper, object? obj)
	{
		return obj is null;
	}

	/// <summary>
	/// Fluent extension method for <see cref="IHtmlHelper"/> to conditionally render content based on whether the current route
	/// matches the specified controller and action.
	/// </summary>
	/// <param name="htmlHelper">The HTML helper.</param>
	/// <param name="action">The action.</param>
	/// <param name="controller">The controller.</param>
	/// <returns>True if the route matches, false otherwise.</returns>
	public static bool IfActive(this IHtmlHelper htmlHelper, string action, string controller)
	{
		var routeData = htmlHelper.ViewContext.RouteData;

		var routeAction = routeData.Values["action"]!.ToString();
		var routeController = routeData.Values["controller"]!.ToString();

		return controller == routeController && (action == routeAction || routeAction == "Details");
	}
}
