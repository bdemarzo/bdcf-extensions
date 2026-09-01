using System.ComponentModel.DataAnnotations;
using Bdcf.Extensions.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Bdcf.Extensions.Data.Tests;

public sealed class EnumDataTypeCheckConstraintConventionTests
{
	[Fact]
	public void NonContiguousEnumValuesCreateDeterministicConstraint()
	{
		using var context = CreateContext();

		var constraint = GetConstraint(context, nameof(TestEntity.Priority));

		Assert.Equal("CK_MappedEntities_Priority_Enum", constraint.Name);
		Assert.Equal("[Priority] IN (0, 3, 7)", constraint.Sql);
	}

	[Fact]
	public void EnumToNumericConversionUsesNumericLiterals()
	{
		using var context = CreateContext();

		var constraint = GetConstraint(context, nameof(TestEntity.NumericPriority));

		Assert.Equal("[NumericPriority] IN (0, 3, 7)", constraint.Sql);
	}

	[Fact]
	public void DuplicateEnumValuesAreIncludedOnce()
	{
		using var context = CreateContext();

		var constraint = GetConstraint(context, nameof(TestEntity.AliasedPriority));

		Assert.Equal("[AliasedPriority] IN (0, 3)", constraint.Sql);
	}

	[Fact]
	public void NullableEnumUsesTheSameConstraintAndAllowsNull()
	{
		using var context = CreateContext();

		var constraint = GetConstraint(context, nameof(TestEntity.OptionalPriority));

		Assert.Equal("[OptionalPriority] IN (0, 3, 7)", constraint.Sql);
	}

	[Fact]
	public void FlagsEnumIsSkipped()
	{
		using var context = CreateContext();

		Assert.DoesNotContain(
			GetDesignModel(context).FindEntityType(typeof(TestEntity))!.GetCheckConstraints(),
			constraint => constraint.Name == "CK_MappedEntities_Flags_Enum");
	}

	[Fact]
	public void IncorrectEnumTypeFailsModelBuilding()
	{
		using var context = CreateInvalidContext();

		var exception = Assert.Throws<InvalidOperationException>(() => _ = context.Model);

		Assert.Contains("WrongPriority", exception.Message);
	}

	[Fact]
	public void ExplicitConstraintWithTheGeneratedNameIsPreserved()
	{
		using var context = CreateContext();

		var constraints = GetDesignModel(context).FindEntityType(typeof(TestEntity))!.GetCheckConstraints()
		.Where(constraint => constraint.Name == "CK_MappedEntities_ExplicitPriority_Enum")
			.ToArray();

		var constraint = Assert.Single(constraints);
		Assert.Equal("[ExplicitPriority] IN (99)", constraint.Sql);
	}

	[Fact]
	public void CustomTableAndColumnNamesAreUsed()
	{
		using var context = CreateContext();

		var constraint = Assert.Single(
			GetDesignModel(context).FindEntityType(typeof(TestEntity))!.GetCheckConstraints(),
			candidate => candidate.Name == "CK_MappedEntities_PriorityCode_Enum");

		Assert.Equal("CK_MappedEntities_PriorityCode_Enum", constraint.Name);
		Assert.Equal("[PriorityCode] IN (0, 3, 7)", constraint.Sql);
	}

	[Fact]
	public void EnumToStringConversionUsesStringLiterals()
	{
		using var context = CreateContext();

		var constraint = GetConstraint(context, nameof(TestEntity.StringPriority));

		Assert.Equal("[StringPriority] IN (N'Low', N'Medium', N'High')", constraint.Sql);
	}

	[Fact]
	public void TablePerTypeMappingsReceiveConstraintsForEachTable()
	{
		using var context = CreateContext();

		var constraints = GetDesignModel(context).GetEntityTypes()
			.SelectMany(entityType => entityType.GetCheckConstraints())
			.Where(constraint => constraint.Name is "CK_BaseEntities_Priority_Enum" or "CK_DerivedEntities_Priority_Enum")
			.ToArray();

		Assert.Equal(2, constraints.Length);
	}

	[Fact]
	public void LongConstraintNamesAreTruncatedDeterministically()
	{
		using var firstContext = CreateLongNameContext();
		using var secondContext = CreateLongNameContext();

		var firstName = Assert.Single(GetDesignModel(firstContext).GetEntityTypes()
			.SelectMany(entityType => entityType.GetCheckConstraints())).Name;
		var secondName = Assert.Single(GetDesignModel(secondContext).GetEntityTypes()
			.SelectMany(entityType => entityType.GetCheckConstraints())).Name;

		Assert.NotNull(firstName);
		Assert.Equal(firstName, secondName);
		Assert.True(firstName!.Length <= 32);
		Assert.Matches("_[0-9A-F]{8}$", firstName);
	}

	private static TestContext CreateContext()
	{
		var options = new DbContextOptionsBuilder<TestContext>()
			.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=EnumDataTypeCheckConstraintConventionTests;Trusted_Connection=True;")
			.Options;

		return new TestContext(options);
	}

	private static InvalidTestContext CreateInvalidContext()
	{
		var options = new DbContextOptionsBuilder<InvalidTestContext>()
			.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=InvalidEnumDataTypeCheckConstraintConventionTests;Trusted_Connection=True;")
			.Options;

		return new InvalidTestContext(options);
	}

	private static LongNameContext CreateLongNameContext()
	{
		var options = new DbContextOptionsBuilder<LongNameContext>()
			.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=LongEnumDataTypeCheckConstraintConventionTests;Trusted_Connection=True;")
			.Options;

		return new LongNameContext(options);
	}

	private static IReadOnlyCheckConstraint GetConstraint(DbContext context, string propertyName)
	{
		return Assert.Single(
			GetDesignModel(context).FindEntityType(typeof(TestEntity))!.GetCheckConstraints(),
			constraint => constraint.Name?.EndsWith($"_{propertyName}_Enum", StringComparison.Ordinal) == true);
	}

	private static IModel GetDesignModel(DbContext context)
	{
		return context.GetService<IDesignTimeModel>().Model;
	}

	private sealed class TestContext(DbContextOptions<TestContext> options) : DbContext(options)
	{
		protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
		{
			configurationBuilder.AddEnumDataTypeCheckConstraintConvention();
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<TestEntity>().ToTable("MappedEntities");
			modelBuilder.Entity<TestEntity>().Property(entity => entity.MappedPriority).HasColumnName("PriorityCode");
			modelBuilder.Entity<TestEntity>().ToTable(table =>
				table.HasCheckConstraint("CK_MappedEntities_ExplicitPriority_Enum", "[ExplicitPriority] IN (99)"));
			modelBuilder.Entity<TestEntity>().Property(entity => entity.StringPriority)
				.HasConversion<EnumToStringConverter<TestPriority>>();
			modelBuilder.Entity<TestEntity>().Property(entity => entity.NumericPriority)
				.HasConversion<int>();
			modelBuilder.Entity<TptBaseEntity>().ToTable("BaseEntities");
			modelBuilder.Entity<TptDerivedEntity>().ToTable("DerivedEntities");
		}
	}

	private sealed class InvalidTestContext(DbContextOptions<InvalidTestContext> options) : DbContext(options)
	{
		protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
		{
			configurationBuilder.AddEnumDataTypeCheckConstraintConvention();
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<InvalidEntity>();
		}
	}

	private sealed class LongNameContext(DbContextOptions<LongNameContext> options) : DbContext(options)
	{
		protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
		{
			configurationBuilder.AddEnumDataTypeCheckConstraintConvention();
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Model.SetMaxIdentifierLength(32);
			modelBuilder.Entity<LongNameEntity>().ToTable("VeryLongEntityTableName");
			modelBuilder.Entity<LongNameEntity>().Property(entity => entity.Priority)
				.HasColumnName("VeryLongPriorityColumnName");
		}
	}

	private sealed class TestEntity
	{
		public int Id { get; set; }

		[EnumDataType(typeof(TestPriority))]
		public TestPriority Priority { get; set; }

		[EnumDataType(typeof(AliasedPriority))]
		public AliasedPriority AliasedPriority { get; set; }

		[EnumDataType(typeof(TestPriority))]
		public TestPriority? OptionalPriority { get; set; }

		[EnumDataType(typeof(TestFlags))]
		public TestFlags Flags { get; set; }

		[EnumDataType(typeof(TestPriority))]
		public TestPriority ExplicitPriority { get; set; }

		[EnumDataType(typeof(TestPriority))]
		public TestPriority MappedPriority { get; set; }

		[EnumDataType(typeof(TestPriority))]
		public TestPriority StringPriority { get; set; }

		[EnumDataType(typeof(TestPriority))]
		public TestPriority NumericPriority { get; set; }
	}

	private sealed class InvalidEntity
	{
		public int Id { get; set; }

		[EnumDataType(typeof(WrongPriority))]
		public TestPriority Priority { get; set; }
	}

	private class TptBaseEntity
	{
		public int Id { get; set; }

		[EnumDataType(typeof(TestPriority))]
		public TestPriority Priority { get; set; }
	}

	private sealed class TptDerivedEntity : TptBaseEntity
	{
		public string Name { get; set; } = string.Empty;
	}

	private sealed class LongNameEntity
	{
		public int Id { get; set; }

		[EnumDataType(typeof(TestPriority))]
		public TestPriority Priority { get; set; }
	}

	private enum TestPriority
	{
		Low = 0,
		Medium = 3,
		High = 7
	}

	private enum AliasedPriority
	{
		Low = 0,
		AlternativeLow = 0,
		High = 3
	}

	[Flags]
	private enum TestFlags
	{
		None = 0,
		Read = 1,
		Write = 2
	}

	private enum WrongPriority
	{
		Invalid
	}
}
