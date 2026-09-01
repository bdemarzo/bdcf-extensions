using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

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

	/// <summary>
	/// Registers the convention that maps <see cref="System.ComponentModel.DataAnnotations.EnumDataTypeAttribute"/>
	/// values to relational enum check constraints.
	/// </summary>
	/// <param name="configurationBuilder">The model configuration builder.</param>
	/// <returns>The same configuration builder.</returns>
	public static ModelConfigurationBuilder AddEnumDataTypeCheckConstraintConvention(this ModelConfigurationBuilder configurationBuilder)
	{
		configurationBuilder.Conventions.Add(serviceProvider =>
			new EnumDataTypeCheckConstraintConvention(
				serviceProvider.GetRequiredService<ISqlGenerationHelper>(),
				serviceProvider.GetRequiredService<IRelationalTypeMappingSource>()));

		return configurationBuilder;
	}

	/// <summary>
	/// Registers the convention that validates enum values before and after EF Core value conversion.
	/// </summary>
	/// <param name="configurationBuilder">The model configuration builder.</param>
	/// <returns>The same configuration builder.</returns>
	public static ModelConfigurationBuilder AddEnumDataTypeValidationConvention(this ModelConfigurationBuilder configurationBuilder)
	{
		configurationBuilder.Conventions.Add(serviceProvider =>
			new EnumDataTypeValidationConvention(
				serviceProvider.GetRequiredService<IRelationalTypeMappingSource>()));

		return configurationBuilder;
	}
}
