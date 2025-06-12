using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Bdcf.Extensions.Web.Mvc;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class NoIndexAttribute : ActionFilterAttribute
{
	/// <summary>
	/// Executes before the action result is executed, allowing for custom processing of the response.
	/// </summary>
	/// <remarks>This method appends an "X-Robots-Tag" header with the value "noindex" to the HTTP response, 
	/// instructing search engines not to index the page. It then calls the base implementation to  ensure standard
	/// processing continues.</remarks>
	/// <param name="context">The <see cref="ResultExecutingContext"/> containing information about the current HTTP context and action result.</param>
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        context.HttpContext.Response.Headers.Append("X-Robots-Tag", "noindex");
        base.OnResultExecuting(context);
    }
}
