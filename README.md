# Cirreum.Services.Serverless

[![NuGet Version](https://img.shields.io/nuget/v/Cirreum.Services.Serverless.svg?style=flat-square&labelColor=1F1F1F&color=003D8F)](https://www.nuget.org/packages/Cirreum.Services.Serverless/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Cirreum.Services.Serverless.svg?style=flat-square&labelColor=1F1F1F&color=003D8F)](https://www.nuget.org/packages/Cirreum.Services.Serverless/)
[![GitHub Release](https://img.shields.io/github/v/release/cirreum/Cirreum.Services.Serverless?style=flat-square&labelColor=1F1F1F&color=FF3B2E)](https://github.com/cirreum/Cirreum.Services.Serverless/releases)
[![License](https://img.shields.io/github/license/cirreum/Cirreum.Services.Serverless?style=flat-square&labelColor=1F1F1F&color=F2F2F2)](https://github.com/cirreum/Cirreum.Services.Serverless/blob/main/LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-003D8F?style=flat-square&labelColor=1F1F1F)](https://dotnet.microsoft.com/)

**Infrastructure services for serverless applications built on Azure Functions**

## Overview

**Cirreum.Services.Serverless** provides essential infrastructure services for Azure Functions applications built on the Cirreum framework. This library offers security, file system operations, clock services, and user management specifically designed for serverless environments.

## Features

- **Function Context Management** - Thread-safe access to Azure Function context across middleware and services
- **User State Management** - Scoped user authentication and session handling with request-level caching
- **CSV File Operations** - Streamlined CSV generation and reading with configurable formatting
- **Cross-Platform Time Services** - Time zone handling with Windows-to-IANA conversion support
- **Serverless Security** - Authentication and authorization patterns optimized for function execution
- **Dependency Injection Integration** - Seamless integration with Microsoft.Extensions.DependencyInjection

## Getting Started

### Installation

```bash
dotnet add package Cirreum.Services.Serverless
```

### Basic Setup

```csharp
var host = new HostBuilder()
    .ConfigureFunctionsWebApplication(builder =>
    {
        builder.AddCoreServices(); // Registers all serverless infrastructure services
    })
    .Build();
```

### Using Function Context

```csharp
public class MyFunction
{
    private readonly IFunctionContextAccessor _contextAccessor;
    private readonly IUserStateAccessor _userAccessor;

    public MyFunction(IFunctionContextAccessor contextAccessor, IUserStateAccessor userAccessor)
    {
        _contextAccessor = contextAccessor;
        _userAccessor = userAccessor;
    }

    [Function("MyFunction")]
    public async Task<HttpResponseData> Run([HttpTrigger] HttpRequestData req)
    {
        var user = await _userAccessor.GetUser();
        var context = _contextAccessor.Context;
        
        // Function logic here
    }
}
```

### CSV File Operations

```csharp
public class DataExportFunction
{
    private readonly ICsvFileBuilder _csvBuilder;

    public DataExportFunction(ICsvFileBuilder csvBuilder)
    {
        _csvBuilder = csvBuilder;
    }

    [Function("ExportData")]
    public async Task<HttpResponseData> ExportData([HttpTrigger] HttpRequestData req)
    {
        var records = GetDataRecords();
        var csvBytes = _csvBuilder.BuildFile(records, delimiter: ",");
        
        // Return CSV file response
    }
}
```

## Architecture

This library extends the Cirreum.Core framework with serverless-specific services:

- **IServerlessDomainApplicationBuilder** - Extends domain application builder with Azure Functions support
- **Function Context Middleware** - Captures and stores function execution context
- **Scoped User Management** - Per-request user state with authentication caching
- **File System Abstractions** - CSV processing with configurable formatting options
- **Clock Services** - Cross-platform time zone handling for distributed environments

## Contribution Guidelines

1. **Be conservative with new abstractions**  
   The API surface must remain stable and meaningful.

2. **Limit dependency expansion**  
   Only add foundational, version-stable dependencies.

3. **Favor additive, non-breaking changes**  
   Breaking changes ripple through the entire ecosystem.

4. **Include thorough unit tests**  
   All primitives and patterns should be independently testable.

5. **Document architectural decisions**  
   Context and reasoning should be clear for future maintainers.

6. **Follow .NET conventions**  
   Use established patterns from Microsoft.Extensions.* libraries.

## Versioning

Cirreum.Services.Serverless follows [Semantic Versioning](https://semver.org/):

- **Major** - Breaking API changes
- **Minor** - New features, backward compatible
- **Patch** - Bug fixes, backward compatible

Given its foundational role, major version bumps are rare and carefully considered.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

**Cirreum Foundation Framework**  
*Layered simplicity for modern .NET*