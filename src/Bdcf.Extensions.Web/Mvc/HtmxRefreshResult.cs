using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Bdcf.Extensions.Web.Http;

namespace Bdcf.Extensions.Web.Mvc;

/// <summary>
/// An <see cref="IActionResult"/> that instructs HTMX to refresh the page by adding the
/// "HX-Refresh: true" response header and returns a 200 OK status code by default.
/// </summary>
[DefaultStatusCode(_defaultStatusCode)]
public class HtmxRefreshResult : StatusCodeResult
{
	/// <summary>
	/// The default HTTP status code returned by this result (200 OK).
	/// </summary>
	private const int _defaultStatusCode = StatusCodes.Status200OK;

	/// <summary>
	/// Initializes a new instance of the <see cref="HtmxRefreshResult"/> class with the default status code.
	/// </summary>
	public HtmxRefreshResult() : base(_defaultStatusCode)
	{
	}

	/// <summary>
	/// Executes the result operation. Adds the HTMX refresh header to the response and
	/// sets the configured status code on the response. This avoids resolving MVC executors
	/// from the request services which may be null in unit-test contexts.
	/// </summary>
	/// <param name="context">The <see cref="ActionContext"/> in which the result is executed.</param>
	/// <returns>A <see cref="Task"/> that represents the asynchronous execute operation.</returns>
	public override Task ExecuteResultAsync(ActionContext context)
	{
		var response = context.HttpContext.Response;
		response.ForceHtmxRefresh();
		response.StatusCode = _defaultStatusCode;
		return Task.CompletedTask;
	}
}
