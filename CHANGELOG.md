# Changelog

All notable changes to BDCF Extensions packages should be recorded in this file.

This project follows tag-based releases. Create releases from tags named `vMAJOR.MINOR.PATCH` or `vMAJOR.MINOR.PATCH-prerelease`.

## Release Policy

- Update this changelog before creating a release tag.
- Keep package versions aligned across `Bdcf.Extensions`, `Bdcf.Extensions.Web`, `Bdcf.Extensions.DacPac`, and `Bdcf.Extensions.Data`.
- Use semantic versioning:
  - increment `MAJOR` for breaking public API or behavior changes
  - increment `MINOR` for backward-compatible features
  - increment `PATCH` for backward-compatible fixes
- GitHub release notes may be generated from merged pull requests, but this changelog is the durable package-facing release history.

## Unreleased

- Added an opt-in EF Core convention that generates provider-aware enum check constraints from `EnumDataTypeAttribute`.
- Added a separate opt-in EF Core convention that validates `EnumDataTypeAttribute` enum values while preserving EF Core's numeric or string value converters.
- Added opt-in kebab-case conventional MVC routing alongside the existing attribute-route token transformation. This is a backward-compatible minor-version feature.
- Breaking: all packages and test projects now target .NET 10; .NET versions before 10 are no longer supported.
- Improved NuGet package metadata, package readme content, SourceLink, XML documentation, symbol packages, and package validation.
- Moved package projects under `src/` and test projects under `tests/`.
- Split DACPAC LocalDB tests into a dedicated integration test project.
