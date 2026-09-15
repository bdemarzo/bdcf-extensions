# BDCF Extensions

Reusable .NET extension libraries used by BDCF applications.

## Framework Support

- .NET 10 and newer
- Earlier .NET and .NET Standard target frameworks are not supported

## Packages

### Bdcf.Extensions

General-purpose helpers for common .NET code:

- string, enum, object, type, and boolean extension methods
- claims principal helpers
- data annotation helpers for decimal and `TimeSpan` precision

### Bdcf.Extensions.Web

ASP.NET Core helpers for MVC and Razor applications:

- AJAX and HTMX request/response helpers
- MVC action results, filters, controller helpers, and TempData serialization
- Razor tag helpers
- kebab-case route token transformation
- `TimeSpan` model binding

### Bdcf.Extensions.DacPac

SQL Server DACPAC deployment helpers:

- apply embedded DACPAC resources with `DacServices`
- configure deployment through options
- apply a DACPAC at host startup

### Bdcf.Extensions.Data

Entity Framework Core 10 model-building conventions:

- map `DefaultValueAttribute` values to relational column defaults
- map `AutoIncludeAttribute` navigations to EF Core eager loading
- generate provider-aware enum check constraints from `EnumDataTypeAttribute`
- optionally validate enum values while preserving EF Core's numeric or string conversions
- preserve explicit Fluent API configuration when it conflicts with an attribute

## Documentation

Detailed documentation for each library is available in the [`/docs`](./docs/) folder:

- [Bdcf.Extensions](./docs/BDCF_EXTENSIONS.md)
- [Bdcf.Extensions.Web](./docs/BDCF_EXTENSIONS_WEB.md)
- [Bdcf.Extensions.DacPac](./docs/BDCF_EXTENSIONS_DACPAC.md)
- [Bdcf.Extensions.Data](./docs/BDCF_EXTENSIONS_DATA.md)

## Repository

Source code is available at https://github.com/bdemarzo/bdcf-extensions.

## License

This project is licensed under the MIT License. See `LICENSE` for details.

## Releases

Releases are created from semantic version tags such as `v1.2.3`. See `CHANGELOG.md` for release history and the release policy.
