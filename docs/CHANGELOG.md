# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Fixed

- **`UserAccessor` now stamps the caller's `AuthenticationBoundary` during user-state
  assembly.** The serverless host never resolved an `IAuthenticationBoundaryResolver`,
  so every authenticated Functions caller carried the unresolved default
  (`AuthenticationBoundary.None`) and grant providers gating on `Global`/`Tenant`
  could never pass — the same seam restored for the server host under ADR-0032.
  `AddCoreServices()` now `TryAdd`-registers the Kernel default resolver
  (authenticated → `Global`; an app-registered resolver wins), and the accessor
  resolves it from the invocation's services with a `null` scheme (a Functions
  binding context carries no ASP.NET authentication scheme). A missing resolver
  still leaves the unresolved default rather than failing the invocation.
- Removed the dead `ServerlessUser.SetAnonymous` method (no callers) and the stale
  exception-message reference to it.
- First test suite (`tests/Cirreum.Services.Serverless.Tests.slnx`): anonymous
  fallbacks, per-invocation caching, and boundary stamping with and without a
  registered resolver.

### Updated

- Updated NuGet packages.

## [1.0.50] - 2026-07-19

### Updated

- Updated NuGet packages.

## [1.0.49] - 2026-07-07

### Removed

- Removed the internal `EmptyTransportPublisher` / `IDistributedTransportPublisher<>` registration from `AddCoreServices`. That transport-publisher seam was dissolved in `Cirreum.Messaging.Distributed 1.2.0` — the no-op type no longer exists, so the registration was a compile error once this package re-pinned to 1.2.0. No effect on serverless hosts: the registration resolved to a no-op that nothing consumed (outbound distribution now runs through the Conductor bridge).

### Updated

- Updated NuGet packages.

## [1.0.46] - 2026-07-04

### Updated

- Updated NuGet packages.

## [1.0.45] - 2026-07-04

### Updated

- Updated NuGet packages.

## [1.0.44] - 2026-07-04

### Updated

- Updated NuGet packages.

## [1.0.43] - 2026-05-10

### Updated

- Updated NuGet packages.

## [1.0.42] - 2026-05-07

### Updated

- Updated NuGet packages.

## [1.0.41] - 2026-05-01

### Updated

- Updated NuGet packages.
