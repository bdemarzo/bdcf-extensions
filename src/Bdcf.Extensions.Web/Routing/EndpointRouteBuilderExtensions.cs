using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Bdcf.Extensions.Web.Routing;

/// <summary>
/// Provides endpoint route builder extensions for MVC routing.
/// </summary>
public static class EndpointRouteBuilderExtensions
{
	/// <summary>
	/// Maps the default MVC controller route with kebab-case controller and action parameters.
	/// </summary>
	/// <param name="endpoints">The endpoint route builder.</param>
	/// <param name="name">The route name.</param>
	/// <returns>The controller action endpoint convention builder.</returns>
	public static ControllerActionEndpointConventionBuilder MapKebabCaseControllerRoute(
		this IEndpointRouteBuilder endpoints,
		string name = "default")
	{
		return endpoints.MapControllerRoute(
			name,
			"{controller:kebab=Home}/{action:kebab=Index}/{id?}");
	}
}
