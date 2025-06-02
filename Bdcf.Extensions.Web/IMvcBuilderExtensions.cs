using Bdcf.Extensions.Web.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;

namespace Bdcf.Extensions.Web;

public static class MvcBuilderExtensions
{
	/// <summary>
	/// Adds the kebab case routing transformer.
	/// </summary>
	/// <param name="builder">The MVC builder.</param>
	/// <returns>The MVC builder.</returns>
	public static IMvcBuilder AddKebabCaseRouting(this IMvcBuilder builder)
	{
		builder.Services.Configure<MvcOptions>(options =>
		{
			options.Conventions.Add(new RouteTokenTransformerConvention(new KebabCaseOutboundParameterTransformer()));
		});

		return builder;
	}
}
