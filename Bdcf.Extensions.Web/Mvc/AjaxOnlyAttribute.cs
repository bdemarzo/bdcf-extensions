using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Bdcf.Extensions.Web.Http;

namespace Bdcf.Extensions.Web.Mvc;

public class AjaxOnlyAttribute : ActionMethodSelectorAttribute
{
	public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
	{
		return routeContext.HttpContext.Request.IsAjax();
	}
}
