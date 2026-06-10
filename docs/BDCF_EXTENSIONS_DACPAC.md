# Bdcf.Extensions.DacPac

SQL Server DACPAC deployment helpers for automated schema migrations at application startup or runtime.

## Overview

`Bdcf.Extensions.DacPac` simplifies deploying embedded DACPAC files to SQL Server databases. It integrates with dependency injection and can run automatically during application startup or be invoked manually.

## Key Features

### DacPacService

Core service for deploying DACPAC files to a database.

```csharp
// Register in Program.cs
services.Configure<DacPacOptions>(options =>
{
	options.ConnectionString = "Server=localhost;Database=MyDb;...";
	options.DacPacName = "database.dacpac";
	options.DacPacResourceMarkerType = typeof(Program);  // Assembly containing the DACPAC
});

services.AddScoped<IDacPacService, DacPacService>();
```

### Automatic Startup Deployment

**`ApplyDacPacHostedService`** — Deploy DACPAC automatically when the application starts.

```csharp
services.Configure<DacPacOptions>(options =>
{
	options.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
	options.DacPacName = "schema.dacpac";
	options.DacPacResourceMarkerType = typeof(Program);
});

services.AddDacPacDeployment();  // Adds hosted service
```

The service will automatically:
1. Load the embedded DACPAC resource
2. Connect to the database using the connection string
3. Deploy the schema
4. Log the operation

### Configuration Options

**`DacPacOptions`** — Configure deployment behavior.

```csharp
public record DacPacOptions
{
	/// Connection string to the target database
	public string ConnectionString { get; init; }

	/// Connection string name from configuration (takes precedence)
	public string ConnectionStringName { get; init; }

	/// Name of the embedded DACPAC file
	public required string DacPacName { get; init; }

	/// Assembly containing the DACPAC (set via code or by type marker)
	public Assembly? DacPacAssembly { get; init; }
	public Type? DacPacResourceMarkerType { get; init; }

	/// Optional deployment options (compare options, scripts, etc.)
	public DacDeployOptions? DeployOptions { get; init; }
}
```

### Manual Deployment

Inject and use the service directly:

```csharp
public class DatabaseMigrationController
{
	private readonly IDacPacService _dacService;

	public DatabaseMigrationController(IDacPacService dacService)
	{
		_dacService = dacService;
	}

	[HttpPost]
	public async Task<IActionResult> DeploySchema()
	{
		try
		{
			await _dacService.DeployAsync();
			return Ok("Schema deployed successfully");
		}
		catch (Exception ex)
		{
			return BadRequest($"Deployment failed: {ex.Message}");
		}
	}
}
```

### Configuration from appsettings.json

```json
{
  "DacPac": {
	"ConnectionStringName": "DefaultConnection",
	"DacPacName": "schema.dacpac"
  }
}
```

Then configure in Program.cs:

```csharp
services.Configure<DacPacOptions>(configuration.GetSection("DacPac"));
```

## Setup Steps

1. **Embed DACPAC in Project** — Add your DACPAC file as an embedded resource in your assembly.

2. **Configure Options**:
   ```csharp
   services.Configure<DacPacOptions>(options =>
   {
	   options.ConnectionString = "your-connection-string";
	   options.DacPacName = "database.dacpac";
	   options.DacPacResourceMarkerType = typeof(Program);
   });
   ```

3. **Register Service**:
   ```csharp
   services.AddDacPacDeployment();  // For automatic startup
   // OR
   services.AddScoped<IDacPacService, DacPacService>();  // For manual use
   ```

4. **Deploy**:
   - Automatic: Application starts and deploys
   - Manual: Inject `IDacPacService` and call `DeployAsync()`

## Common Scenarios

### Scenario: Deploy on Application Startup

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DacPacOptions>(options =>
{
	options.ConnectionStringName = "DefaultConnection";
	options.DacPacName = "schema.dacpac";
	options.DacPacResourceMarkerType = typeof(Program);
});

builder.Services.AddDacPacDeployment();

var app = builder.Build();
// DACPAC is deployed during app startup before the first request
app.Run();
```

### Scenario: Deploy on Admin Command

```csharp
app.MapPost("/admin/deploy-schema", async (IDacPacService service) =>
{
	try
	{
		await service.DeployAsync();
		return Results.Ok("Deployed");
	}
	catch (Exception ex)
	{
		return Results.BadRequest(ex.Message);
	}
});
```

## Installation

Install via NuGet:

```
dotnet add package Bdcf.Extensions.DacPac
```

## Requirements

- .NET 8 or newer
- SQL Server (local or remote)
- DACPAC file embedded in your assembly

## See Also

- [Bdcf.Extensions](./BDCF_EXTENSIONS.md) — General-purpose .NET helpers
- [Bdcf.Extensions.Web](./BDCF_EXTENSIONS_WEB.md) — ASP.NET Core helpers
