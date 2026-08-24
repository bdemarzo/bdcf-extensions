using System.ComponentModel;
using Bdcf.Extensions.Data;
using Microsoft.EntityFrameworkCore;

namespace Bdcf.Extensions.Data.Tests;

public sealed class DefaultValueConventionTests
{
	[Fact]
	public void DefaultValueIsAppliedToRelationalProperty()
	{
		using var context = CreateContext();

		var property = context.Model.FindEntityType(typeof(TestEntity))!.FindProperty(nameof(TestEntity.Count));

		Assert.Equal(7, property!.GetDefaultValue());
	}

	[Fact]
	public void DefaultValueSupportsStringValues()
	{
		using var context = CreateContext();

		var property = context.Model.FindEntityType(typeof(TestEntity))!.FindProperty(nameof(TestEntity.Status));

		Assert.Equal("Pending", property!.GetDefaultValue());
	}

	[Fact]
	public void DefaultValueSupportsEnumValues()
	{
		using var context = CreateContext();

		var property = context.Model.FindEntityType(typeof(TestEntity))!.FindProperty(nameof(TestEntity.Priority));

		Assert.Equal(TestPriority.High, property!.GetDefaultValue());
	}

	[Fact]
	public void FluentConfigurationOverridesDefaultValue()
	{
		using var context = CreateContext();

		var property = context.Model.FindEntityType(typeof(TestEntity))!.FindProperty(nameof(TestEntity.Explicit));

		Assert.Equal("Fluent", property!.GetDefaultValue());
	}

	[Fact]
	public void PropertyWithoutDefaultValueRemainsUnconfigured()
	{
		using var context = CreateContext();

		var property = context.Model.FindEntityType(typeof(TestEntity))!.FindProperty(nameof(TestEntity.NoDefault));

		Assert.Null(property!.GetDefaultValue());
	}

	private static TestContext CreateContext()
	{
		var options = new DbContextOptionsBuilder<TestContext>()
			.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=DefaultValueConventionTests;Trusted_Connection=True;")
			.Options;

		return new TestContext(options);
	}

	private sealed class TestContext(DbContextOptions<TestContext> options) : DbContext(options)
	{
		protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
		{
			configurationBuilder.AddDefaultValueConvention();
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<TestEntity>().Property(entity => entity.Explicit).HasDefaultValue("Fluent");
		}
	}

	private sealed class TestEntity
	{
		public int Id { get; set; }

		[DefaultValue(7)]
		public int Count { get; set; }

		[DefaultValue("Pending")]
		public string Status { get; set; } = string.Empty;

		[DefaultValue(TestPriority.High)]
		public TestPriority Priority { get; set; }

		[DefaultValue("Attribute")]
		public string Explicit { get; set; } = string.Empty;

		public string NoDefault { get; set; } = string.Empty;
	}

	private enum TestPriority
	{
		Low,
		High
	}
}
