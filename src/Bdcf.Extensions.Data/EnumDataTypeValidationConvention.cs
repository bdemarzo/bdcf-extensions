using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Bdcf.Extensions.Data;

#pragma warning disable EF1001

/// <summary>
/// Wraps EF Core's resolved enum converter and rejects values that are not declared by the enum.
/// </summary>
public sealed class EnumDataTypeValidationConvention(
	IRelationalTypeMappingSource relationalTypeMappingSource)
	: IModelFinalizingConvention
{
	/// <summary>
	/// Applies enum validation to properties decorated with <see cref="EnumDataTypeAttribute"/>.
	/// </summary>
	/// <param name="modelBuilder">The convention model builder.</param>
	/// <param name="context">The convention execution context.</param>
	public void ProcessModelFinalizing(
		IConventionModelBuilder modelBuilder,
		IConventionContext<IConventionModelBuilder> context)
	{
		foreach (var entityType in modelBuilder.Metadata.GetEntityTypes())
		{
			foreach (var property in entityType.GetProperties())
			{
				var attribute = property.PropertyInfo?.GetCustomAttribute<EnumDataTypeAttribute>();
				if (attribute is null)
				{
					continue;
				}

				var enumType = Nullable.GetUnderlyingType(property.ClrType) ?? property.ClrType;
				if (!enumType.IsEnum || attribute.EnumType != enumType)
				{
					throw new InvalidOperationException(
						$"The EnumDataTypeAttribute on '{entityType.DisplayName()}.{property.Name}' specifies '{attribute.EnumType.FullName}', but the property's enum type is '{enumType.FullName}'.");
				}

				if (enumType.IsDefined(typeof(FlagsAttribute), inherit: false))
				{
					continue;
				}

				var typeMapping = relationalTypeMappingSource.FindMapping((IProperty)property);
				var converter = typeMapping?.Converter;
				if (converter is null)
				{
					throw new InvalidOperationException(
						$"No value converter was found for enum property '{entityType.DisplayName()}.{property.Name}'.");
				}

				var wrappedConverter = CreateValidatingConverter(converter, property.Name);
				property.SetValueConverter(wrappedConverter, fromDataAnnotation: false);
			}
		}
	}

	private static ValueConverter CreateValidatingConverter(ValueConverter converter, string propertyName)
	{
		var toProvider = WrapExpression(
			converter.ConvertToProviderExpression,
			nameof(EnumDataTypeValidationHelpers.ConvertToProvider),
			converter,
			propertyName);
		var fromProvider = WrapExpression(
			converter.ConvertFromProviderExpression,
			nameof(EnumDataTypeValidationHelpers.ConvertFromProvider),
			converter,
			propertyName);

		return (ValueConverter)typeof(EnumDataTypeValidationConvention)
			.GetMethod(nameof(CreateTypedConverter), BindingFlags.Static | BindingFlags.NonPublic)!
			.MakeGenericMethod(converter.ModelClrType, converter.ProviderClrType)
			.Invoke(null, [toProvider, fromProvider, converter.ConvertsNulls, converter.MappingHints])!;
	}

	private static ValueConverter CreateTypedConverter<TModel, TProvider>(
		LambdaExpression toProvider,
		LambdaExpression fromProvider,
		bool convertsNulls,
		ConverterMappingHints? mappingHints)
	{
		var typedToProvider = Expression.Lambda<Func<TModel, TProvider>>(
			toProvider.Body,
			toProvider.Parameters);
		var typedFromProvider = Expression.Lambda<Func<TProvider, TModel>>(
			fromProvider.Body,
			fromProvider.Parameters);

		return new ValidatingValueConverter<TModel, TProvider>(
			typedToProvider,
			typedFromProvider,
			convertsNulls,
			mappingHints);
	}

	private static LambdaExpression WrapExpression(
		LambdaExpression expression,
		string helperName,
		ValueConverter converter,
		string propertyName)
	{
		var parameter = expression.Parameters[0];
		var helper = typeof(EnumDataTypeValidationHelpers).GetMethod(
			helperName,
			BindingFlags.Static | BindingFlags.Public)!;
		var call = Expression.Call(
			helper,
			Expression.Constant(converter),
			Expression.Convert(parameter, typeof(object)),
			Expression.Constant(propertyName));

		return Expression.Lambda(
			Expression.Convert(call, expression.Body.Type),
			parameter);
	}

	private sealed class ValidatingValueConverter<TModel, TProvider>(
		Expression<Func<TModel, TProvider>> convertToProviderExpression,
		Expression<Func<TProvider, TModel>> convertFromProviderExpression,
		bool convertsNulls,
		ConverterMappingHints? mappingHints)
		: ValueConverter<TModel, TProvider>(
			convertToProviderExpression,
			convertFromProviderExpression,
			convertsNulls,
			mappingHints);
}

internal static class EnumDataTypeValidationHelpers
{
	public static object ConvertToProvider(ValueConverter converter, object value, string propertyName)
	{
		Validate(value, propertyName);
		return converter.ConvertToProvider(value)!;
	}

	public static object ConvertFromProvider(ValueConverter converter, object value, string propertyName)
	{
		var modelValue = converter.ConvertFromProvider(value);
		Validate(modelValue, propertyName);
		return modelValue!;
	}

	private static void Validate(object? value, string propertyName)
	{
		if (value is null)
		{
			return;
		}

		var enumType = Nullable.GetUnderlyingType(value.GetType()) ?? value.GetType();
		if (!enumType.IsEnum || !Enum.IsDefined(enumType, value))
		{
			throw new InvalidOperationException(
				$"The value '{value}' is not defined by enum '{enumType.FullName}' for property '{propertyName}'.");
		}
	}
}

#pragma warning restore EF1001
