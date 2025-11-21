# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Development Commands

### Build and Package
```bash
# Restore dependencies
dotnet restore Cirreum.Services.Serverless.slnx

# Build the solution
dotnet build Cirreum.Services.Serverless.slnx --configuration Release

# Pack for NuGet (creates packages in ./artifacts)
dotnet pack Cirreum.Services.Serverless.slnx --configuration Release --output ./artifacts
```

### Local Development
```bash
# Build in debug mode
dotnet build Cirreum.Services.Serverless.slnx --configuration Debug
```

## Architecture Overview

This is a .NET 10.0 serverless infrastructure library designed for Azure Functions, built on the Cirreum framework foundation. The library provides core serverless services and middleware for Azure Functions applications.

### Key Components

**Core Interfaces & Builders**
- `IServerlessDomainApplicationBuilder` - Main builder interface extending `IDomainApplicationBuilder` with Azure Functions support
- Wraps `FunctionsApplicationBuilder` and provides access to configuration and hosting environment

**Security & User Management**
- `FunctionContextAccessor` - Thread-safe accessor for Azure Function context using `AsyncLocal<T>`
- `UserAccessor` - Scoped user state management with request-level caching
- `ServerlessUser` - User state implementation for serverless environments
- `FunctionContextAccessorMiddleware` - Middleware to capture and store function context

**File System Operations**
- `CsvFileBuilder` - Creates CSV files from record collections with configurable delimiters and class maps
- `CsvFileReader` - Reads CSV files (implementation follows same pattern)
- `NotImplementedFileSystem` - Placeholder file system implementation

**Time Services**
- `DateTimeService` - Provides time zone conversion with Windows-to-IANA mapping for cross-platform compatibility

### Service Registration Pattern

The library follows the extension method pattern for service registration:

```csharp
builder.AddCoreServices(); // Registers all core serverless services
```

This registers:
- Function context middleware and accessor
- User state management (scoped)
- File system services (CSV operations)
- Clock services with system TimeProvider
- Empty transport publisher (placeholder)

### Framework Dependencies

- **Microsoft.Azure.Functions.Worker** (v2.51.0) - Core Azure Functions runtime
- **Cirreum.Core** (v1.0.15) - Foundation framework providing base interfaces and services

### Build Configuration

- **Target Framework**: .NET 10.0 with latest C# language features
- **Nullable Reference Types**: Enabled
- **Implicit Usings**: Enabled for common namespaces
- **Documentation**: XML documentation generation enabled
- **CI/CD**: GitHub Actions workflow for automated NuGet publishing on release tags

### Global Usings

The project includes global usings for:
- `Microsoft.Azure.Functions.Worker`
- `Microsoft.Extensions.Logging`
- `Cirreum` namespaces (Core, Exceptions, FileSystem)

### Development Notes

- The library uses scoped dependency injection for user context to ensure proper isolation per function invocation
- Function context is captured via middleware and made available through dependency injection
- User authentication state is cached per request to avoid repeated principal resolution
- Time zone handling includes fallback mechanisms for cross-platform compatibility