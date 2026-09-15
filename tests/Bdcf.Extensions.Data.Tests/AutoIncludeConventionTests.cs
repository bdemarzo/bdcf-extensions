using Bdcf.Extensions.Data;
using Bdcf.Extensions.Data.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Bdcf.Extensions.Data.Tests;

public sealed class AutoIncludeConventionTests
{
	[Fact]
	public void AttributeAutoIncludesReferenceNavigation()
	{
		using var context = CreateContext();

		var navigation = context.Model.FindEntityType(typeof(Post))!
			.FindNavigation(nameof(Post.Blog));

		Assert.NotNull(navigation);
		Assert.True(navigation.IsEagerLoaded);
	}

	[Fact]
	public void AttributeAutoIncludesCollectionNavigation()
	{
		using var context = CreateContext();

		var navigation = context.Model.FindEntityType(typeof(Blog))!
			.FindNavigation(nameof(Blog.Posts));

		Assert.NotNull(navigation);
		Assert.True(navigation.IsEagerLoaded);
	}

	[Fact]
	public void AttributeAutoIncludesSkipNavigation()
	{
		using var context = CreateContext();

		var navigation = context.Model.FindEntityType(typeof(Blog))!
			.FindSkipNavigation(nameof(Blog.Tags));

		Assert.NotNull(navigation);
		Assert.True(navigation.IsEagerLoaded);
	}

	[Fact]
	public void UnmarkedNavigationIsNotAutoIncluded()
	{
		using var context = CreateContext();

		var navigation = context.Model.FindEntityType(typeof(Blog))!
			.FindNavigation(nameof(Blog.Category));

		Assert.NotNull(navigation);
		Assert.False(navigation.IsEagerLoaded);
	}

	[Fact]
	public void FluentConfigurationOverridesAttribute()
	{
		using var context = CreateContext();

		var navigation = context.Model.FindEntityType(typeof(Blog))!
			.FindNavigation(nameof(Blog.Owner));

		Assert.NotNull(navigation);
		Assert.False(navigation.IsEagerLoaded);
	}

	private static TestContext CreateContext()
	{
		var options = new DbContextOptionsBuilder<TestContext>()
			.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=AutoIncludeConventionTests;Trusted_Connection=True;")
			.Options;

		return new TestContext(options);
	}

	private sealed class TestContext(DbContextOptions<TestContext> options) : DbContext(options)
	{
		protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
		{
			configurationBuilder.AddAutoIncludeConvention();
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Blog>()
				.HasMany(blog => blog.Posts)
				.WithOne(post => post.Blog)
				.HasForeignKey(post => post.BlogId);

			modelBuilder.Entity<Blog>()
				.HasOne(blog => blog.Owner)
				.WithMany(owner => owner.Blogs)
				.HasForeignKey(blog => blog.OwnerId);

			modelBuilder.Entity<Blog>()
				.HasOne(blog => blog.Category)
				.WithMany(category => category.Blogs)
				.HasForeignKey(blog => blog.CategoryId);

			modelBuilder.Entity<Blog>()
				.HasMany(blog => blog.Tags)
				.WithMany(tag => tag.Blogs);

			modelBuilder.Entity<Blog>()
				.Navigation(blog => blog.Owner)
				.AutoInclude(false);
		}
	}

	private sealed class Blog
	{
		public int Id { get; set; }

		public int OwnerId { get; set; }

		public int CategoryId { get; set; }

		[AutoInclude]
		public ICollection<Post> Posts { get; set; } = [];

		[AutoInclude]
		public BlogOwner Owner { get; set; } = null!;

		public BlogCategory Category { get; set; } = null!;

		[AutoInclude]
		public ICollection<BlogTag> Tags { get; set; } = [];
	}

	private sealed class Post
	{
		public int Id { get; set; }

		public int BlogId { get; set; }

		[AutoInclude]
		public Blog Blog { get; set; } = null!;
	}

	private sealed class BlogOwner
	{
		public int Id { get; set; }

		public ICollection<Blog> Blogs { get; set; } = [];
	}

	private sealed class BlogCategory
	{
		public int Id { get; set; }

		public ICollection<Blog> Blogs { get; set; } = [];
	}

	private sealed class BlogTag
	{
		public int Id { get; set; }

		public ICollection<Blog> Blogs { get; set; } = [];
	}
}
