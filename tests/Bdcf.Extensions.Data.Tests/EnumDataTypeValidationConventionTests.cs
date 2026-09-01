using System.ComponentModel.DataAnnotations;
using Bdcf.Extensions.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Bdcf.Extensions.Data.Tests;

public sealed class EnumDataTypeValidationConventionTests
{
	[Fact]
	public void NumericProviderConversionRejectsUndefinedValues()
	{
		using var context = CreateContext();
		var converter = GetRuntimeConverter(context);

		Assert.Equal(typeof(int), converter.ProviderClrType);
		Assert.Equal(3, converter.ConvertToProvider(TestPriority.Medium));
		Assert.Throws<InvalidOperationException>(() => converter.ConvertToProvider((TestPriority)99));
		Assert.Throws<InvalidOperationException>(() => converter.ConvertFromProvider(99));
	}

	[Fact]
	public void StringProviderConversionRejectsUndefinedValues()
	{
		using var context = CreateContext();
		var converter = GetRuntimeConverter(context, nameof(TestEntity.StringPriority));

		Assert.Equal(typeof(string), converter.ProviderClrType);
		Assert.Equal("Medium", converter.ConvertToProvider(TestPriority.Medium));
		Assert.Throws<InvalidOperationException>(() => converter.ConvertToProvider((TestPriority)99));
		Assert.Throws<InvalidOperationException>(() => converter.ConvertFromProvider("Unknown"));
	}

	[Fact]
	public void ValidationConventionDoesNotRegisterDatabaseConstraints()
	{
		using var context = CreateContext();

		Assert.Empty(GetDesignModel(context).FindEntityType(typeof(TestEntity))!.GetCheckConstraints());
	}

	private static ValidationTestContext CreateContext()
	{
		var options = new DbContextOptionsBuilder<ValidationTestContext>()
			.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=EnumDataTypeValidationConventionTests;Trusted_Connection=True;")
			.Options;

		return new ValidationTestContext(options);
	}

	private static ValueConverter GetRuntimeConverter(
		DbContext context,
		string propertyName = nameof(TestEntity.NumericPriority))
	{
		return context.Model.FindEntityType(typeof(TestEntity))!
			.FindProperty(propertyName)!
			.GetTypeMapping()
			.Converter!;
	}

	private static IModel GetDesignModel(DbContext context)
	{
		return context.GetService<IDesignTimeModel>().Model;
	}

	private sealed class ValidationTestContext(DbContextOptions<ValidationTestContext> options) : DbContext(options)
	{
		protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
		{
			configurationBuilder.AddEnumDataTypeValidationConvention();
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<TestEntity>().ToTable("ValidationEntities");
			modelBuilder.Entity<TestEntity>().Property(entity => entity.StringPriority)
				.HasConversion<EnumToStringConverter<TestPriority>>();
			modelBuilder.Entity<TestEntity>().Property(entity => entity.NumericPriority)
				.HasConversion<int>();
		}
	}

	private sealed class TestEntity
	{
		public int Id { get; set; }

		[EnumDataType(typeof(TestPriority))]
		public TestPriority NumericPriority { get; set; }

		[EnumDataType(typeof(TestPriority))]
		public TestPriority StringPriority { get; set; }
	}

	private enum TestPriority
	{
		Low = 0,
		Medium = 3,
		High = 7
	}
}
