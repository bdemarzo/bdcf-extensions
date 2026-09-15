using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Numerics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Storage;

namespace Bdcf.Extensions.Data;

/// <summary>
/// Applies <see cref="EnumDataTypeAttribute"/> values to relational enum check constraints.
/// </summary>
public sealed class EnumDataTypeCheckConstraintConvention(
	ISqlGenerationHelper sqlGenerationHelper,
	IRelationalTypeMappingSource relationalTypeMappingSource)
	: IModelFinalizingConvention
{
	private const string ConstraintSuffix = "_Enum";

	/// <summary>
	/// Applies enum check constraints declared on mapped CLR properties.
	/// </summary>
	/// <param name="modelBuilder">The model builder.</param>
	/// <param name="context">The convention context.</param>
	public void ProcessModelFinalizing(
		IConventionModelBuilder modelBuilder,
		IConventionContext<IConventionModelBuilder> context)
	{
		var existingConstraintNames = modelBuilder.Metadata.GetEntityTypes()
			.SelectMany(entityType => entityType.GetCheckConstraints())
			.Select(constraint => constraint.Name)
			.ToHashSet(StringComparer.Ordinal);

		foreach (var entityType in modelBuilder.Metadata.GetEntityTypes())
		{
			var tableName = entityType.GetTableName();
			if (tableName is null)
			{
				continue;
			}

			var storeObject = StoreObjectIdentifier.Table(tableName, entityType.GetSchema());
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

				var enumValues = GetDistinctEnumValues(enumType);
				if (enumValues.Count == 0)
				{
					continue;
				}

				var typeMapping = relationalTypeMappingSource.FindMapping((IProperty)property)
					?? throw new InvalidOperationException(
						$"No relational type mapping was found for '{entityType.DisplayName()}.{property.Name}'.");
				var columnName = property.GetColumnName(storeObject);
				if (columnName is null)
				{
					continue;
				}

				var constraintName = CreateConstraintName(
					$"CK_{tableName}_{columnName}{ConstraintSuffix}",
					modelBuilder.Metadata.GetMaxIdentifierLength());

				if (!existingConstraintNames.Add(constraintName))
				{
					continue;
				}

				var sql = $"{sqlGenerationHelper.DelimitIdentifier(columnName)} IN ({string.Join(", ", enumValues.Select(value => GenerateSqlLiteral(typeMapping, value.Value)))})";
				entityType.Builder.HasCheckConstraint(constraintName, sql, fromDataAnnotation: true);
			}
		}
	}

	private static IReadOnlyList<EnumValue> GetDistinctEnumValues(Type enumType)
	{
		var underlyingType = Enum.GetUnderlyingType(enumType);

		return Enum.GetValues(enumType)
			.Cast<object>()
			.Select(value => new EnumValue(
				value,
				ConvertToBigInteger(Convert.ChangeType(value, underlyingType, CultureInfo.InvariantCulture)!, underlyingType)))
			.GroupBy(value => value.NumericValue)
			.Select(group => group.First())
			.OrderBy(value => value.NumericValue)
			.ToArray();
	}

	private static BigInteger ConvertToBigInteger(object value, Type underlyingType)
	{
		if (underlyingType == typeof(sbyte)) return (sbyte)value;
		if (underlyingType == typeof(byte)) return (byte)value;
		if (underlyingType == typeof(short)) return (short)value;
		if (underlyingType == typeof(ushort)) return (ushort)value;
		if (underlyingType == typeof(int)) return (int)value;
		if (underlyingType == typeof(uint)) return (uint)value;
		if (underlyingType == typeof(long)) return (long)value;
		if (underlyingType == typeof(ulong)) return new BigInteger((ulong)value);

		throw new InvalidOperationException($"Unsupported enum underlying type '{underlyingType.FullName}'.");
	}

	private static string GenerateSqlLiteral(RelationalTypeMapping typeMapping, object enumValue)
	{
		return typeMapping.GenerateSqlLiteral(enumValue);
	}

	private static string CreateConstraintName(string name, int? maxIdentifierLength)
	{
		if (maxIdentifierLength is not int maxLength || name.Length <= maxLength)
		{
			return name;
		}

		const int hashLength = 8;
		var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(name)))[..hashLength];
		var suffix = $"_{hash}";
		var prefixLength = maxLength - suffix.Length;
		if (prefixLength <= 0)
		{
			throw new InvalidOperationException($"The generated enum check constraint name cannot fit within the provider's maximum identifier length of {maxLength}.");
		}

		return name[..prefixLength] + suffix;
	}

	private sealed record EnumValue(object Value, BigInteger NumericValue);
}
