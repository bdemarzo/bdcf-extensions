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
	public void DefaultEnumMappingRejectsUndefinedValues()
	{
		using var context = CreateContext();
		var converter = GetRuntimeConverter(context, nameof(TestEntity.DefaultPriority));

		Assert.Equal(typeof(int), converter.ProviderClrType);
		Assert.Equal(3, converter.ConvertToProvider(TestPriority.Medium));
		Assert.Throws<InvalidOperationException>(() => converter.ConvertToProvider((TestPriority)99));
		Assert.Throws<InvalidOperationException>(() => converter.ConvertFromProvider(99));
	}

	[Fact]
	public void NullableEnumMappingPreservesNullAndRejectsUndefinedValues()
	{
		using var context = CreateContext();
		var converter = GetRuntimeConverter(context, nameof(TestEntity.NullablePriority));

		Assert.Null(converter.ConvertToProvider(null));
		Assert.Equal(3, converter.ConvertToProvider(TestPriority.Medium));
		Assert.Throws<InvalidOperationException>(() => converter.ConvertToProvider((TestPriority)99));
	}

	[Fact]
	public void FlagsEnumMappingIsNotValidatedAsADeclaredSingleValue()
	{
		using var context = CreateContext();
		var converter = GetRuntimeConverter(context, nameof(TestEntity.Flags));

		Assert.Equal(3, converter.ConvertToProvider(TestFlags.Read | TestFlags.Write));
	}

	[Fact]
	public void SignedAndUnsignedEnumMappingsRejectUndefinedValues()
	{
		using var context = CreateContext();
		var signedConverter = GetRuntimeConverter(context, nameof(TestEntity.SignedPriority));
		var unsignedConverter = GetRuntimeConverter(context, nameof(TestEntity.UnsignedPriority));

		Assert.Equal(7L, Convert.ToInt64(signedConverter.ConvertToProvider(SignedPriority.High)));
		Assert.Throws<InvalidOperationException>(() => signedConverter.ConvertToProvider((SignedPriority)99));
		Assert.Equal(7L, Convert.ToInt64(unsignedConverter.ConvertToProvider(UnsignedPriority.High)));
		Assert.Throws<InvalidOperationException>(() => unsignedConverter.ConvertToProvider((UnsignedPriority)99));
	}

	[Fact]
	public void MismatchedEnumDataTypeThrowsDuringModelFinalization()
	{
		using var context = new MismatchedValidationTestContext(CreateOptions<MismatchedValidationTestContext>());

		var exception = Assert.Throws<InvalidOperationException>(() => context.Model);

		Assert.Contains("specifies", exception.Message);
	}

	[Fact]
	public void ValidationConventionDoesNotRegisterDatabaseConstraints()
	{
		using var context = CreateContext();

		Assert.Empty(GetDesignModel(context).FindEntityType(typeof(TestEntity))!.GetCheckConstraints());
	}

	private static ValidationTestContext CreateContext()
	{
		return new ValidationTestContext(CreateOptions<ValidationTestContext>());
	}

	private static DbContextOptions<TContext> CreateOptions<TContext>()
		where TContext : DbContext
	{
		return new DbContextOptionsBuilder<TContext>()
			.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=EnumDataTypeValidationConventionTests;Trusted_Connection=True;")
			.Options;
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

		[EnumDataType(typeof(TestPriority))]
		public TestPriority DefaultPriority { get; set; }

		[EnumDataType(typeof(TestPriority))]
		public TestPriority? NullablePriority { get; set; }

		[EnumDataType(typeof(TestFlags))]
		public TestFlags Flags { get; set; }

		[EnumDataType(typeof(SignedPriority))]
		public SignedPriority SignedPriority { get; set; }

		[EnumDataType(typeof(UnsignedPriority))]
		public UnsignedPriority UnsignedPriority { get; set; }
	}

	private sealed class MismatchedValidationTestContext(DbContextOptions<MismatchedValidationTestContext> options) : DbContext(options)
	{
		protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
		{
			configurationBuilder.AddEnumDataTypeValidationConvention();
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<MismatchedEntity>().ToTable("MismatchedValidationEntities");
		}
	}

	private sealed class MismatchedEntity
	{
		public int Id { get; set; }

		[EnumDataType(typeof(TestPriority))]
		public TestFlags Priority { get; set; }
	}

	private enum TestPriority
	{
		Low = 0,
		Medium = 3,
		High = 7
	}

	[Flags]
	private enum TestFlags
	{
		None = 0,
		Read = 1,
		Write = 2
	}

	private enum SignedPriority : long
	{
		Low = -1,
		High = 7
	}

	private enum UnsignedPriority : uint
	{
		Low = 0,
		High = 7
	}
}
