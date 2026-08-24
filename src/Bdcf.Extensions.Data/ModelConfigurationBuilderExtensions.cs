using Microsoft.EntityFrameworkCore;

namespace Bdcf.Extensions.Data;

/// <summary>
/// Extensions for registering BDCF Entity Framework Core conventions.
/// </summary>
public static class ModelConfigurationBuilderExtensions
{
	/// <summary>
	/// Registers the convention that maps <see cref="System.ComponentModel.DefaultValueAttribute"/>
	/// values to relational column defaults.
	/// </summary>
	/// <param name="configurationBuilder">The model configuration builder.</param>
	/// <returns>The same configuration builder.</returns>
	public static ModelConfigurationBuilder AddDefaultValueConvention(this ModelConfigurationBuilder configurationBuilder)
	{
		configurationBuilder.Conventions.Add(_ => new DefaultValueConvention());

		return configurationBuilder;
	}
}
