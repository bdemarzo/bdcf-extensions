# Bdcf.Extensions.Data

Entity Framework Core 10 model-building helpers for BDCF applications.

## Default value attributes

`AddDefaultValueConvention()` registers a model-finalizing convention that maps `DefaultValueAttribute` values on mapped CLR properties to relational column defaults. Explicit Fluent API configuration takes precedence over an attribute.

Register it in `DbContext.ConfigureConventions`:

```csharp
using Bdcf.Extensions.Data;
using Microsoft.EntityFrameworkCore;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
	protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
	{
		configurationBuilder.AddDefaultValueConvention();
	}
}
```

Then annotate entity properties with the value that should be used by the database column:

```csharp
using System.ComponentModel;

public sealed class Order
{
	public int Id { get; set; }

	[DefaultValue("Pending")]
	public string Status { get; set; } = string.Empty;
}
```

The convention is provider-agnostic but requires a relational EF Core provider because it configures relational column metadata, just like `Property(...).HasDefaultValue(...)`.

## Installation

```text
dotnet add package Bdcf.Extensions.Data
```

## Requirements

- .NET 10 or newer
- Entity Framework Core 10
