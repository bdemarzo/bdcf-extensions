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

## Auto-include navigations

`AddAutoIncludeConvention()` registers a navigation convention that maps `AutoIncludeAttribute` on navigation properties to EF Core's AutoInclude configuration, enabling eager-loading of the decorated navigation by default.

Register the convention in `DbContext.ConfigureConventions`:

```csharp
using Bdcf.Extensions.Data;
using Microsoft.EntityFrameworkCore;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
	protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
	{
		configurationBuilder.AddAutoIncludeConvention();
	}
}
```

Annotate navigation properties to opt them into automatic eager loading:

```csharp
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Bdcf.Extensions.Data.DataAnnotations;

public sealed class Order
{
	public int Id { get; set; }

	[ForeignKey(nameof(CustomerId))]
	public Customer Customer { get; set; } = null!;
	public int? CustomerId { get; set; }
}

public sealed class Customer
{
	public int Id { get; set; }

	[AutoInclude]
	public ICollection<Order> Orders { get; set; } = null!;
}
```

Notes and behavior:
- The convention applies to reference, collection, and skip navigations mapped by EF Core.
- Explicit Fluent API configuration (for example, `navigationBuilder.AutoInclude()`) takes precedence over the attribute.
- Use this convention to centralize simple eager-loading intent in code via attributes while preserving the option to override behavior via the Fluent API.
- The convention is provider-agnostic because AutoInclude is an EF Core runtime behavior rather than a database-specific feature.

## Enum check constraints

`AddEnumDataTypeCheckConstraintConvention()` registers an opt-in model-finalizing convention that maps `EnumDataTypeAttribute` on enum properties to relational check constraints. The attribute's `EnumType` must match the property's enum type, including the underlying type of nullable enums.

```csharp
using System.ComponentModel.DataAnnotations;
using Bdcf.Extensions.Data;

public enum OrderPriority
{
	Low = 0,
	Standard = 3,
	Urgent = 7
}

public sealed class Order
{
	public int Id { get; set; }

	[EnumDataType(typeof(OrderPriority))]
	public OrderPriority? Priority { get; set; }
}

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
	protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
	{
		configurationBuilder.AddEnumDataTypeCheckConstraintConvention();
	}
}
```

For a numeric enum column, this creates a deterministic constraint such as `CK_Orders_Priority_Enum` with SQL similar to `[Priority] IN (0, 3, 7)`. SQL identifiers and literals are generated using the active relational provider's mapping services, so enum-to-string conversions generate string literals instead.

Nullable enum properties retain their normal nullable behavior because a check constraint containing `IN (...)` does not reject `NULL`. Explicit Fluent API configuration remains authoritative. For example, EF Core 10's preferred syntax is:

```csharp
modelBuilder.Entity<Order>().ToTable(table =>
	table.HasCheckConstraint("CK_Orders_Priority_Enum", "[Priority] IN (0, 3)"));
```

Properties decorated with an incorrectly matching `EnumDataTypeAttribute` fail model building. `[Flags]` enums are currently skipped because their valid values require bitmask semantics rather than a simple `IN` predicate.

## Enum value validation

`AddEnumDataTypeValidationConvention()` is a separate opt-in convention for applications that also need runtime protection. It preserves EF Core's normal enum mapping—including numeric mappings, `HasConversion<string>()`, and custom converters—and wraps the resolved converter so undefined enum values are rejected both before they are written and after they are read.

```csharp
protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
{
	configurationBuilder
		.AddEnumDataTypeCheckConstraintConvention()
		.AddEnumDataTypeValidationConvention();
}
```

The two conventions can be enabled independently. The validation convention does not add database constraints, and the check-constraint convention does not replace EF Core's value converter. Use both when the database and application runtime should enforce the same declared enum values. The validation convention applies the same `EnumType` matching rules and skips `[Flags]` enums.

## Installation

```text
dotnet add package Bdcf.Extensions.Data
```

## Requirements

- .NET 10 or newer
- Entity Framework Core 10
