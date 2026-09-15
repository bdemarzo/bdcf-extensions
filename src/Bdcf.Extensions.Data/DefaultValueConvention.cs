using System.ComponentModel;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Bdcf.Extensions.Data;

/// <summary>
/// Applies <see cref="DefaultValueAttribute"/> values to relational properties as column defaults.
/// </summary>
public sealed class DefaultValueConvention : IModelFinalizingConvention
{
	/// <summary>
	/// Applies default values declared on mapped CLR properties.
	/// </summary>
	/// <param name="modelBuilder">The model builder.</param>
	/// <param name="context">The convention context.</param>
	public void ProcessModelFinalizing(
		IConventionModelBuilder modelBuilder,
		IConventionContext<IConventionModelBuilder> context)
	{
		foreach (var property in modelBuilder.Metadata.GetEntityTypes().SelectMany(entityType => entityType.GetDeclaredProperties()))
		{
			var defaultValueAttribute = property.PropertyInfo?.GetCustomAttribute<DefaultValueAttribute>();
			if (defaultValueAttribute is null)
			{
				continue;
			}

			// Treat the attribute as data annotation configuration so explicit Fluent API
			// configuration remains authoritative.
			property.Builder.HasDefaultValue(defaultValueAttribute.Value, fromDataAnnotation: true);
		}
	}
}
