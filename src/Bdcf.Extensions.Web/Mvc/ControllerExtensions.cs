using Bdcf.Extensions.Web.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bdcf.Extensions.Web.Mvc;

public static class ControllerExtensions
{
	/// <summary>
	/// HTMXs the refresh.
	/// </summary>
	/// <param name="controller">The controller.</param>
	/// <returns></returns>
	[NonAction]
	public static ResetContentResult HtmxRefresh(this Controller controller)
	{
		controller.Response.ForceHtmxRefresh();
		return new ResetContentResult();
	}

	/// <summary>
	/// HTMXs the redirect.
	/// </summary>
	/// <param name="controller">The controller.</param>
	/// <param name="redirectUrl">The redirect URL.</param>
	/// <returns></returns>
	[NonAction]
	public static OkResult HtmxRedirect(this Controller controller, string redirectUrl)
	{
		controller.Response.ForceHtmxRedirect(redirectUrl);
		return new OkResult();
	}
}
