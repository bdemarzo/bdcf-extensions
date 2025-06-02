using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Bdcf.Extensions.Web.Mvc;

/// <summary>
/// An <see cref="StatusCodeResult"/> that when executed will produce an empty
/// <see cref="StatusCodes.Status205ResetContent"/> response.
/// </summary>
[DefaultStatusCode(DefaultStatusCode)]
public class ResetContentResult : StatusCodeResult
{
    private const int DefaultStatusCode = StatusCodes.Status205ResetContent;

    /// <summary>
    /// Initializes a new instance of the <see cref="OkResult"/> class.
    /// </summary>
    public ResetContentResult() : base(DefaultStatusCode)
    {
    }
}
