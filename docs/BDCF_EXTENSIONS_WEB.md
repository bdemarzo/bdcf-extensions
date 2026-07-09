# Bdcf.Extensions.Web

ASP.NET Core enhancements and extensions for MVC/Razor applications, with first-class support for HTMX.

## Table of Contents

- [Overview](#overview)
- [Key Features](#key-features)
  - [HTTP Request Helpers](#http-request-helpers)
  - [HTTP Response Helpers](#http-response-helpers)
  - [Action Method Attributes](#action-method-attributes)
  - [Action Results](#action-results)
  - [Tag Helpers](#tag-helpers)
  - [Model Binding](#model-binding)
  - [TempData Extensions](#tempdata-extensions)
  - [Route Transformation](#route-transformation)
  - [HTML Helper Extensions](#html-helper-extensions)
- [Installation](#installation)
- [Requirements](#requirements)
- [See Also](#see-also)

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

**`[NoIndex]`** — Add an `X-Robots-Tag: noindex` response header to prevent search engines from indexing a controller or action.

```csharp
[NoIndex]
public IActionResult Preview()
{
	return View();
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

**`HtmxRefresh()`** — Add the `HX-Refresh: true` response header and return a `ResetContentResult`.

```csharp
[HttpPost]
public IActionResult Save(SettingsForm form)
{
	_service.Save(form);
	return this.HtmxRefresh();
}
```

**`HtmxRedirect(string redirectUrl)`** — Add the `HX-Redirect` response header and return an `OkResult`.

```csharp
[HttpPost]
public IActionResult Create(ProjectForm form)
{
	var project = _service.Create(form);
	return this.HtmxRedirect($"/projects/{project.Id}");
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

**`asp-active-route-class`** — Add a CSS class to an anchor when the current route matches its `asp-area`, `asp-controller`, and `asp-action`.

Enable the tag helper in `_ViewImports.cshtml`:

```cshtml
@addTagHelper *, Bdcf.Extensions.Web
```

```html
<a class="nav-link"
   asp-controller="Songs"
   asp-action="Index"
   asp-active-route-class="active">
	Songs
</a>
```

When active, the helper adds the supplied class and `aria-current="page"`. A minimized or blank `asp-active-route-class` value is ignored.

**`Html5ValidationTagHelper`** — Add HTML5 validation attributes based on data annotations.

Enable the tag helper in `_ViewImports.cshtml`:

```cshtml
@addTagHelper *, Bdcf.Extensions.Web
```

```html
<!-- Automatically renders data-val, data-val-required, etc. -->
<input asp-for="Email" />
```

### Model Binding

**`TimeSpanModelBinder`** — Bind `TimeSpan` from query strings and form data.

```csharp
// Configure in Program.cs
builder.Services.AddControllersWithViews()
	.AddTimeSpanModelBinder();

// URL: ?duration=01:30:00
public IActionResult Process(TimeSpan duration)
{
	// duration = 1 hour, 30 minutes
}
```

### TempData Extensions

**`Put<T>(string key, T value)`** — Serialize an object and store it in `TempData`.

```csharp
TempData.Put("message", new AlertMessage
{
	Title = "Saved",
	Body = "Your changes were saved."
});
```

**`Get<T>(string key)`** — Deserialize an object from `TempData`, or return `null` when the key is missing.

```csharp
AlertMessage? message = TempData.Get<AlertMessage>("message");
```

### Route Transformation

**`KebabCaseOutboundParameterTransformer`** — Convert route parameter names to kebab-case in URLs.

```csharp
// Configure in Program.cs
builder.Services.AddControllersWithViews()
	.AddKebabCaseRouting();

// Usage in route
[Route("api/[controller]/[action]")]
public IActionResult MyActionMethod() { }

// Generated URL: /api/controller/my-action-method
```

### HTML Helper Extensions

**`If(bool? condition)`** — Return the condition value, or `false` when the condition is `null`.

```cshtml
@if (Html.If(Model.ShowDetails))
{
	<section>Details</section>
}
```

**`IfNot(bool condition)`** — Return the inverse of a condition.

```cshtml
@if (Html.IfNot(User.Identity?.IsAuthenticated ?? false))
{
	<a asp-controller="Account" asp-action="Login">Sign in</a>
}
```

**`IfHasValue(object obj)`** — Return `true` when an object is not `null`.

```cshtml
@if (Html.IfHasValue(Model.Customer))
{
	<p>@Model.Customer.Name</p>
}
```

**`IfHasValue(string? str)`** — Return `true` when a string is not null, empty, or whitespace.

```cshtml
@if (Html.IfHasValue(Model.Description))
{
	<p>@Model.Description</p>
}
```

**`IfNull(object? obj)`** — Return `true` when an object is `null`.

```cshtml
@if (Html.IfNull(Model.Customer))
{
	<p>No customer selected.</p>
}
```

**`IfActive(string action, string controller)`** — Return `true` when the current route matches the specified controller and action. The `Details` action also matches the specified action for the same controller.

```cshtml
<li class="@(Html.IfActive("Index", "Projects") ? "active" : "")">
	<a asp-controller="Projects" asp-action="Index">Projects</a>
</li>
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
