using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Bdcf.Extensions.Web.Mvc;

/// <summary>
/// Represents an HTTP result with a status code indicating that the content has been reset (HTTP 205).
/// </summary>
[DefaultStatusCode(DefaultStatusCode)]
public class ResetContentResult : StatusCodeResult
{
    private const int DefaultStatusCode = StatusCodes.Status205ResetContent;

	/// <summary>
	/// Represents an HTTP result with a status code indicating that the content has been reset (HTTP 205).
	/// </summary>
	/// <remarks>This result is typically used to inform the client that the server has successfully processed the
	/// request and that the client should reset the view or clear the form used for the request.</remarks>
    public ResetContentResult() : base(DefaultStatusCode)
    {
    }
}
