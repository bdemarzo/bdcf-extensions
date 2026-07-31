using Bdcf.Extensions.Web.Mvc;
using Bdcf.Extensions.Web.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Routing;
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
		builder.Services.Configure<RouteOptions>(options =>
		{
			options.ConstraintMap["kebab"] = typeof(KebabCaseOutboundParameterTransformer);
		});

		return builder;
	}

	/// <summary>
	/// Adds a custom model binder for <see cref="TimeSpan"/> types to the MVC framework, at the front of the provider list.
	/// </summary>
	/// <param name="builder">The <see cref="IMvcBuilder"/> used to configure MVC services.</param>
	/// <returns>The <see cref="IMvcBuilder"/> instance, allowing for further configuration.</returns>
	public static IMvcBuilder AddTimeSpanModelBinder(this IMvcBuilder builder)
	{
		builder.Services.Configure<MvcOptions>(options =>
		{
			options.ModelBinderProviders.Insert(0, new TimeSpanModelBinderProvider());
		});
		return builder;
	}
}
