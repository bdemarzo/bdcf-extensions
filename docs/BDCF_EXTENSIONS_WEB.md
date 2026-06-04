# Bdcf.Extensions.Web

ASP.NET Core helpers for MVC/Razor applications, with first-class support for HTMX.

## Overview

`Bdcf.Extensions.Web` provides utilities for building dynamic ASP.NET Core applications with request/response helpers, action filters, tag helpers, and model binding extensions.

## Key Features

### HTTP Request Helpers

**`IsAjax()`** — Detect AJAX or XMLHttpRequest.

```csharp
if (request.IsAjax())
{
	return PartialView("_PartialResult");
}
```

**`IsHtmx()`** — Detect HTMX requests (checks for `HX-Request: true` header).

```csharp
if (request.IsHtmx())
{
	return Content("Updated content", "text/html");
}
```

### HTTP Response Helpers

**`ForceHtmxRefresh()`** — Send HTMX refresh command to client.

```csharp
response.ForceHtmxRefresh();  // HTMX client will refresh the page
```

**`ForceHtmxRedirect(string url)`** — Send HTMX redirect command.

```csharp
response.ForceHtmxRedirect("/dashboard");  // HTMX client redirects to URL
```

### Action Method Attributes

**`[HtmxOnly]`** — Restrict action to HTMX requests only.

```csharp
[HttpPost]
[HtmxOnly]
public IActionResult UpdatePartial(int id)
{
	// Only callable from HTMX requests
	return PartialView("_Updated");
}
```

**`[AjaxOnly]`** — Restrict action to AJAX requests only.

```csharp
[HttpGet]
[AjaxOnly]
public IActionResult GetData()
{
	return Json(data);
}
```

### Action Results

**`ResetContentResult`** — Return HTTP 205 (Reset Content) to tell clients to clear forms/state.

```csharp
[HttpPost]
public IActionResult Submit(FormData form)
{
	_service.Process(form);
	return new ResetContentResult();  // Client clears form
}
```

### Tag Helpers

**`include-if`/`exclude-if`** — Conditionally render elements in Razor views.

```html
<!-- Include element only if condition is true -->
<div include-if="@User.IsInRole(\"Admin\")">
	Admin Panel
</div>

<!-- Exclude element if condition is true -->
<nav exclude-if="@User.Identity?.IsAuthenticated ?? false">
	Login / Sign Up
</nav>
```

**`Html5ValidationTagHelper`** — Add HTML5 validation attributes based on data annotations.

```html
<!-- Automatically renders data-val, data-val-required, etc. -->
<input asp-for="Email" />
```

### Model Binding

**`TimeSpanModelBinder`** — Bind `TimeSpan` from query strings and form data.

```csharp
// URL: ?duration=01:30:00
public IActionResult Process(TimeSpan duration)
{
	// duration = 1 hour, 30 minutes
}
```

### Route Transformation

**`KebabCaseOutboundParameterTransformer`** — Convert route parameter names to kebab-case in URLs.

```csharp
// Configure in Program.cs
services.AddRouting(options =>
{
	options.ConstraintMap["kebab"] = typeof(KebabCaseOutboundParameterTransformer);
});

// Usage in route
[Route("api/[controller]/[action:kebab]")]
public IActionResult MyActionMethod() { }

// Generated URL: /api/controller/my-action-method
```

### MVC Helpers

**`HtmlHelperExtensions`** — Utilities for working with HTML helpers.

**`ControllerExtensions`** — Controller helper methods.

**`TempDataExtensions`** — Serialize/deserialize complex objects in TempData.

### Startup Integration

**`IMvcBuilderExtensions`** — Fluent extension methods for configuring MVC.

```csharp
services.AddMvc()
	.AddHtmxSupport()
	.AddCustomTagHelpers();
```

## Installation

Install via NuGet:

```
dotnet add package Bdcf.Extensions.Web
```

## Requirements

- .NET 8 or newer
- ASP.NET Core 8.0 or newer

## See Also

- [Bdcf.Extensions](./BDCF_EXTENSIONS.md) — General-purpose .NET helpers
- [Bdcf.Extensions.DacPac](./BDCF_EXTENSIONS_DACPAC.md) — SQL Server DACPAC deployment
