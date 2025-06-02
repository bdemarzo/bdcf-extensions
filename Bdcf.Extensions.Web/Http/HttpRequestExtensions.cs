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
		if (request.Headers is not null)
		{
			return request.Headers.XRequestedWith == "XMLHttpRequest" || request.Headers["Hx-Request"] == "true";
		}

		return false;
	}
}
