# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Development Commands

### Build and Package
```bash
# Restore dependencies
dotnet restore Cirreum.Services.Serverless.slnx

# Build the solution
dotnet build Cirreum.Services.Serverless.slnx --configuration Release

# Run tests (dedicated test solution — tests are NOT in the main slnx)
dotnet test tests/Cirreum.Services.Serverless.Tests.slnx

# Pack for NuGet (creates packages in ./artifacts)
dotnet pack Cirreum.Services.Serverless.slnx --configuration Release --output ./artifacts
```

## Architecture Overview

This is a .NET 10.0 serverless infrastructure library for Azure Functions (isolated
worker), built on the Cirreum framework foundation. It provides the serverless host's
implementations of core Cirreum services and middleware.

### Key Components

**Core Interfaces & Builders**
- `IServerlessDomainApplicationBuilder` — main builder interface extending `IDomainApplicationBuilder` with Azure Functions support; wraps `FunctionsApplicationBuilder` and provides access to configuration and hosting environment

**Security & User Management**
- `FunctionContextAccessor` — thread-safe accessor for the current `FunctionContext` using `AsyncLocal<T>` (the Functions analog of `HttpContextAccessor`)
- `FunctionContextAccessorMiddleware` — captures the function context per invocation
- `UserAccessor` — `IUserStateAccessor` implementation; assembles `ServerlessUser` per invocation, cached on `FunctionContext.Items`. User states are mutable and therefore **never shared across invocations** — each context gets its own instance
- **Authentication-boundary stamping**: during user-state assembly the accessor resolves `IAuthenticationBoundaryResolver` (from the invocation's `InstanceServices`) and stamps the caller's `AuthenticationBoundary`, passing a `null` scheme — a Functions binding context carries no ASP.NET authentication scheme. `AddCoreServices()` `TryAdd`-registers the Kernel default resolver (authenticated → `Global`); an app-registered resolver wins. A missing resolver leaves the unresolved default (`None`) rather than failing the invocation

**File System Operations**
- `CsvFileBuilder` / `CsvFileReader` — CSV creation and reading with configurable delimiters and class maps
- `NotImplementedFileSystem` — deliberate placeholder `IFileSystem` (general file-system operations are unsupported in the serverless host)

**Time Services**
- `DateTimeService` — time zone conversion with Windows-to-IANA mapping for cross-platform compatibility

### Service Registration Pattern

```csharp
builder.AddCoreServices(); // Registers all core serverless services
```

This registers: the function-context middleware and accessor, user-state management
(scoped) plus the default authentication-boundary resolver (`TryAdd`), CSV file
services, the placeholder file system, and clock services with the system
`TimeProvider`.

### Framework Dependencies

- **Microsoft.Azure.Functions.Worker** — Azure Functions isolated worker runtime
- **Cirreum.Domain** — the framework spine (brings `Cirreum.Contracts`, `Cirreum.Kernel`, `Cirreum.Result`, `Cirreum.Exceptions` transitively)
- **Cirreum.Messaging** / **Cirreum.Messaging.Distributed** — messaging surface for serverless heads

### Testing

`tests/Cirreum.Services.Serverless.Tests.slnx` — xUnit + FluentAssertions +
NSubstitute. `FunctionContext` is substituted (abstract members: `Items`,
`InstanceServices`); `IFunctionContextAccessor` is substituted directly so tests
never need the Functions host. Covers the anonymous fallbacks, per-invocation
caching and isolation, and boundary stamping with and without a registered
resolver.

### Development Notes

- Target framework: .NET 10.0, latest C#, nullable enabled, tabs + K&R (see `.editorconfig`)
- Scoped DI for user context ensures isolation per function invocation
- User authentication state is cached per invocation on `FunctionContext.Items`
- Time zone handling includes fallback mechanisms for cross-platform compatibility
- Global usings: `Microsoft.Azure.Functions.Worker`, `Microsoft.Extensions.Logging`, `Cirreum`, `Cirreum.Exceptions`, `Cirreum.FileSystem`
