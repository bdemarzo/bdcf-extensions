using Microsoft.Extensions.DependencyInjection;

namespace Bdcf.Extensions.DacPac;

public static class IServiceCollectionExtensions
{
	/// <summary>
	/// Adds the DacPac service and its configuration to the specified <see cref="IServiceCollection"/>.
	/// Use this overload when you have already configured <see cref="DacPacOptions"/> via the options pattern
	/// (for example: services.Configure&lt;DacPacOptions&gt;(configuration.GetSection("DacPac"))).
	/// </summary>
	/// <param name="services">The <see cref="IServiceCollection"/> to which the DacPac service will be added.</param>
	/// <returns>The updated <see cref="IServiceCollection"/> to allow for method chaining.</returns>
	public static IServiceCollection AddDacPacService(this IServiceCollection services)
	{
		services.AddSingleton<IDacPacService, DacPacService>();

		return services;
	}

	/// <summary>
	/// Adds the DacPac service and its configuration to the specified <see cref="IServiceCollection"/>.
	/// Use this overload when you want to configure <see cref="DacPacOptions"/> inline via delegate.
	/// </summary>
	/// <param name="services">The <see cref="IServiceCollection"/> to which the DacPac service will be added.</param>
	/// <param name="options">A delegate to configure the <see cref="DacPacOptions"/> used by the DacPac service.</param>
	/// <returns>The updated <see cref="IServiceCollection"/> to allow for method chaining.</returns>
	public static IServiceCollection AddDacPacService(this IServiceCollection services, Action<DacPacOptions> options)
	{
		services.Configure(options);
		services.AddSingleton<IDacPacService, DacPacService>();

		return services;
	}
}
