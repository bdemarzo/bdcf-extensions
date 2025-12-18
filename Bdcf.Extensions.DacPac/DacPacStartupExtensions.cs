using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bdcf.Extensions.DacPac;

public static class DacPacStartupExtensions
{
	/// <summary>
	/// Registers a hosted service that will apply the configured DACPAC when the host starts.
	/// By default errors are logged and startup continues. Set <paramref name="failOnError"/> to true to
	/// fail host startup when the DACPAC application fails.
	/// </summary>
	public static IHostApplicationBuilder AddDacPacApplyOnStartup(this IHostApplicationBuilder builder, bool failOnError = false)
	{
		// FailOnError is init-only on ApplyDacPacHostedServiceOptions, so register an options instance
		builder.Services.AddSingleton(Options.Create(new ApplyDacPacHostedServiceOptions { FailOnError = failOnError }));
		builder.Services.AddHostedService<ApplyDacPacHostedService>();
		return builder;
	}

	/// <summary>
	/// Applies the DACPAC using the registered <see cref="IDacPacService"/> immediately. Useful to call after building the host
	/// but before running it (for example: <c>host.ApplyDacPac().Run();</c>).
	/// </summary>
	public static IHost ApplyDacPac(this IHost host, bool failOnError = false)
	{
		using var scope = host.Services.CreateScope();
		var svc = scope.ServiceProvider.GetService<IDacPacService>();
		if (svc is null)
			return host;

		var logger = scope.ServiceProvider.GetService<ILogger<ApplyDacPacHostedService>>();

		try
		{
			logger?.LogInformation("Applying DACPAC on host instance...");
			svc.ApplyDacPac();
			logger?.LogInformation("DACPAC applied successfully on host instance.");
		}
		catch (Exception ex)
		{
			logger?.LogError(ex, "Error while applying DACPAC on host instance.");
			if (failOnError)
				throw;
		}

		return host;
	}
}
