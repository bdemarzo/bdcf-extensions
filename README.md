# BDCF Extensions

Reusable .NET extension libraries used by BDCF applications.

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

## Repository

Source code is available at https://github.com/bdemarzo/bdcf-extensions.
