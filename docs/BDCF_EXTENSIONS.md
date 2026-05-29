# Bdcf.Extensions

General-purpose helper methods for common .NET tasks.

## Overview

`Bdcf.Extensions` provides reusable extension methods and attributes to simplify everyday .NET development. It's designed for utility code that doesn't fit into framework-specific libraries.

## Key Features

### String Extensions

**`Left(int length)`** — Get the first N characters of a string.

```csharp
"Hello World".Left(5)  // "Hello"
"Hi".Left(10)          // "Hi"
```

### Enum Extensions

**`GetDisplayName()`** — Get the display name from a `DisplayAttribute`, or the enum name as fallback.

```csharp
public enum Status
{
	[Display(Name = "In Progress")]
	InProgress,

	Completed
}

Status.InProgress.GetDisplayName()  // "In Progress"
Status.Completed.GetDisplayName()   // "Completed"
```

### Boolean Extensions

**`Then(string output)`** — Return a string if condition is true, empty string otherwise.

```csharp
isAdmin.Then("Admin Panel")  // "Admin Panel" if true, "" if false
```

**`NotThen(string output)`** — Return a string if condition is false, empty string otherwise.

```csharp
isGuest.NotThen("Sign In")  // "Sign In" if false, "" if true
```

### Object Extensions

**`In<T>(params T[] values)`** — Check if an object is in a set of values.

```csharp
status.In(Status.Active, Status.Pending)  // true if status matches either
```

**`NotIn<T>(params T[] values)`** — Check if an object is not in a set of values.

```csharp
role.NotIn(Role.Admin, Role.SuperAdmin)  // true if role matches neither
```

**`Between<T>(T start, T end)`** — Check if a value is between two values (inclusive).

```csharp
5.Between(1, 10)      // true
DateTime.Now.Between(startDate, endDate)  // inclusive range check
```

### Type Extensions

**`IsDirectDescendantOf(Type baseType)`** — Check if a type directly inherits from a base type.

```csharp
typeof(SpecificService).IsDirectDescendantOf(typeof(BaseService))  // true if direct inheritance
```

### Data Annotations

**`PrecisionAttribute`** — Normalize decimal and TimeSpan values to a specified precision during validation.

```csharp
public class Order
{
	[Precision(DecimalPlaces = 2)]
	public decimal Total { get; set; }

	[Precision(TimeSpanPrecision = TimeSpanPrecision.Hours)]
	public TimeSpan Duration { get; set; }
}

// ValidationContext will truncate Total to 2 decimal places and Duration to hours precision
```

## Installation

Install via NuGet:

```
dotnet add package Bdcf.Extensions
```

## Requirements

- .NET 8 or newer

## See Also

- [Bdcf.Extensions.Web](./BDCF_EXTENSIONS_WEB.md) — ASP.NET Core helpers
- [Bdcf.Extensions.DacPac](./BDCF_EXTENSIONS_DACPAC.md) — SQL Server DACPAC deployment
