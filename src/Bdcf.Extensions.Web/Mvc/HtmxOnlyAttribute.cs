using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Bdcf.Extensions.Web.Http;

namespace Bdcf.Extensions.Web.Mvc;

/// <summary>
/// Specifies that an action method is only valid for HTMX requests.
/// </summary>
/// <remarks>This attribute ensures that the action method can only be invoked if the incoming HTTP request is an
/// HTMX request. Use this attribute to restrict access to actions that are designed to be called asynchronously via
/// JavaScript using the HTMX library.</remarks>
public class HtmxOnlyAttribute : ActionMethodSelectorAttribute
{
	/// <summary>
	/// Determines whether the current request is valid for the specified route and action.
	/// </summary>
	/// <param name="routeContext">The context of the current route, which provides access to the HTTP context and route data.</param>
	/// <param name="action">The action descriptor representing the action being invoked.</param>
	/// <returns><see langword="true"/> if the request is an HTMX request; otherwise, <see langword="false"/>.</returns>
	public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
	{
		return routeContext.HttpContext.Request.IsHtmx();
	}
}
