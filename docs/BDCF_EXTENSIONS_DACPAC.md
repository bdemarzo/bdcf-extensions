# Bdcf.Extensions.DacPac

Deploy an embedded SQL Server DACPAC from a .NET host using DacFx.

## Setup

Embed the DACPAC in an assembly and set `DacPacName` to its **exact manifest resource name**, which may include the assembly's root namespace and folders. For example, set an explicit name in the project containing the file:

```xml
<EmbeddedResource Include="schema.dacpac" LogicalName="schema.dacpac" />
```

Register the service and options in the host. The options have init-only properties, so construct an options instance rather than assigning properties inside a `Configure` callback.

```csharp
using Bdcf.Extensions.DacPac;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IOptions<DacPacOptions>>(Options.Create(new DacPacOptions
{
    ConnectionStringName = "DefaultConnection",
    DacPacName = "schema.dacpac",
    DacPacResourceMarkerType = typeof(Program)
}));
builder.Services.AddDacPacService();
builder.AddDacPacApplyOnStartup(failOnError: true);

var app = builder.Build();
app.Run();
```

The hosted service applies the DACPAC during host startup. By default it logs deployment errors and allows startup to continue; `failOnError: true` propagates the error. Registering `AddDacPacApplyOnStartup` does not itself register `IDacPacService`.

Alternatively, omit the hosted service and call `app.ApplyDacPac(failOnError: true)` after building the host and before running it. This method does nothing when `IDacPacService` is not registered; it logs deployment errors and continues unless `failOnError` is true.

## Configuration

`ConnectionStringName` looks first in the host's `ConnectionStrings` configuration section, then in `ConfigurationManager.ConnectionStrings`. If no nonempty named value is found, `ConnectionString` is used. The connection string must contain `Database` or `Initial Catalog`.

Specify the resource assembly using `DacPacAssembly`, `DacPacResourceMarkerType`, or `AssemblyName`, in that order of precedence. `AssemblyName` is suitable for configuration binding:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MyDb;Integrated Security=true;TrustServerCertificate=true"
  },
  "DacPac": {
    "ConnectionStringName": "DefaultConnection",
    "AssemblyName": "MyApplication",
    "DacPacName": "schema.dacpac"
  }
}
```

```csharp
builder.Services.Configure<DacPacOptions>(builder.Configuration.GetSection("DacPac"));
builder.Services.AddDacPacService();
```

The deployment options exposed on `DacPacOptions` are:

| Property | Default |
| --- | --- |
| `BlockOnPossibleDataLoss` | `true` |
| `GenerateSmartDefaults` | `false` |
| `LongRunningCommandTimeout` | `0` (no timeout) |
| `DropObjectsNotInSource` | `false` |
| `VerifyDeployment` | `true` |

## Manual deployment

Inject `IDacPacService` and call `ApplyDacPac()` or `ApplyDacPacAsync()`:

```csharp
app.MapPost("/admin/deploy-schema", async (IDacPacService service, CancellationToken cancellationToken) =>
{
    await service.ApplyDacPacAsync(cancellationToken);
    return Results.Ok("Deployed");
});
```

The underlying DacFx deployment is synchronous. The async method schedules that work on the thread pool and honors cancellation before it starts; it does not cancel an ongoing deployment.

## Installation and requirements

```text
dotnet add package Bdcf.Extensions.DacPac
```

Requires .NET 10 or newer, a SQL Server database, and an embedded DACPAC.

## See also

- [Bdcf.Extensions](./BDCF_EXTENSIONS.md) — general-purpose .NET helpers
- [Bdcf.Extensions.Web](./BDCF_EXTENSIONS_WEB.md) — ASP.NET Core helpers
