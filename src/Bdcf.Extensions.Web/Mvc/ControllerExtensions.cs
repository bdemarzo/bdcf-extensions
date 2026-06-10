using Bdcf.Extensions.Web.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bdcf.Extensions.Web.Mvc;

public static class ControllerExtensions
{
	/// <summary>
	/// Adds the HTMX "HX-Refresh" response header and returns a <see cref="ResetContentResult"/>.
	/// The header is added immediately for scenarios (like unit tests) that inspect the response.
	/// </summary>
	/// <param name="controller">The controller instance from which the response is obtained.</param>
	/// <returns>A <see cref="ResetContentResult"/> that signals an HTMX refresh.</returns>
	[NonAction]
	public static ResetContentResult HtmxRefresh(this Controller controller)
	{
		// Also set the header immediately so calling code that inspects the Response in unit tests sees it.
		controller.Response.ForceHtmxRefresh();
		return new ResetContentResult();
	}

	/// <summary>
	/// Adds the HTMX "HX-Redirect" response header with the specified URL and returns an <see cref="OkResult"/>.
	/// The header is added immediately so calling code that inspects the response sees it.
	/// </summary>
	/// <param name="controller">The controller instance from which the response is obtained.</param>
	/// <param name="redirectUrl">The URL to which HTMX should redirect the client.</param>
	/// <returns>An <see cref="OkResult"/>.</returns>
	[NonAction]
	public static OkResult HtmxRedirect(this Controller controller, string redirectUrl)
	{
		controller.Response.ForceHtmxRedirect(redirectUrl);
		return new OkResult();
	}
}
