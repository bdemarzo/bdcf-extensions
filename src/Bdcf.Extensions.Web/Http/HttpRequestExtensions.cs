using Microsoft.AspNetCore.Http;

namespace Bdcf.Extensions.Web.Http;

public static class HttpRequestExtensions
{
	/// <summary>
	/// Determines whether the specified HTTP request is an AJAX request.
	/// </summary>
	/// <returns>
	/// true if the specified HTTP request is an AJAX request; otherwise, false.
	/// </returns>
	/// <param name="request">The HTTP request.</param><exception cref="T:System.ArgumentNullException">The <paramref name="request"/> parameter is null (Nothing in Visual Basic).</exception>
	public static bool IsAjax(this HttpRequest request)
	{
		if (request.Headers is null)
		{
			return false;
		}

		return request.Headers.XRequestedWith == "XMLHttpRequest" || request.Headers["Hx-Request"] == "true";
	}

	/// <summary>
	/// Determines whether the specified HTTP request is an HTMX request.
	/// </summary>
	/// <param name="request">The HTTP request.</param>
	/// <returns>true if the specified HTTP request is an HTMX request; otherwise, false.</returns>
	public static bool IsHtmxRequest(this HttpRequest request)
	{
		if (request.Headers is null)
		{
			return false;
		}

		return string.Equals(request.Headers["HX-Request"], "true", StringComparison.OrdinalIgnoreCase);
	}
}
