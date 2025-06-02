using Microsoft.AspNetCore.Http;

namespace Bdcf.Extensions.Web.Http;

public static class HttpResponseExtensions
{
	/// <summary>
	/// Adds an HTMX refresh command to the response headers.
	/// </summary>
	/// <param name="response">The response.</param>
	public static void ForceHtmxRefresh(this HttpResponse response)
	{
		response.Headers.Append("HX-Refresh", "true");
	}

	/// <summary>
	/// Adds an HTMX redirect command to the response headers.
	/// </summary>
	/// <param name="response">The response.</param>
	/// <param name="redirectUrl">The redirect URL.</param>
	public static void ForceHtmxRedirect(this HttpResponse response, string redirectUrl)
	{
		response.Headers.Append("HX-Redirect", redirectUrl);
	}
}
