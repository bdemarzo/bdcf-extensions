using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Bdcf.Extensions.Web.Mvc;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class NoIndexAttribute : ActionFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        context.HttpContext.Response.Headers.Append("X-Robots-Tag", "noindex");
        base.OnResultExecuting(context);
    }
}
