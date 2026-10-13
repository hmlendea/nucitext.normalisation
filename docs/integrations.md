# Integrations

## Overview

`NuciText.Normalisation` is a minimal, self-contained library with few integration points. It integrates primarily through its public interface and is designed to be easily consumed by .NET applications.

## Integration points

### 1. Public API (Primary integration)

```
Consumer code
    │
    ▼
INuciTextNormaliser (interface)
    │
    ▼
NuciTextNormaliser (implementation)
    │
    ▼
System.Text.RegularExpressions (BCL)
```

#### Interface contract
- **Type:** `INuciTextNormaliser`
- **Method:** `NormaliseSentence(string): string`
- **Location:** `NuciText.Normalisation/INuciTextNormaliser.cs`

#### Implementation
- **Type:** `NuciTextNormaliser` (sealed class)
- **Location:** `NuciText.Normalisation/NuciTextNormaliser.cs`

### 2. Dependency injection

Designed for DI registration:
```csharp
services.AddSingleton<INuciTextNormaliser, NuciTextNormaliser>();
```

### 3. NuGet package

Distributed as a NuGet package:
- **Package ID:** `NuciText.Normalisation`
- **Target framework:** `net10.0`
- **Dependencies:** None (BCL only)

### 4. Test project integration

Test project references library project:
```xml
<ItemGroup>
  <ProjectReference Include="..\NuciText.Normalisation\NuciText.Normalisation.csproj" />
</ItemGroup>
```

## Integration patterns

### Direct instantiation

```csharp
var normaliser = new NuciTextNormaliser();
string result = normaliser.NormaliseSentence(input);
```

### Constructor injection

```csharp
public class MyService
{
    private readonly INuciTextNormaliser _normaliser;

    public MyService(INuciTextNormaliser normaliser)
    {
        _normaliser = normaliser;
    }

    public string Process(string input)
        => _normaliser.NormaliseSentence(input) ?? "";
}
```

### Method injection

```csharp
public void Process(string input, INuciTextNormaliser normaliser)
{
    string result = normaliser.NormaliseSentence(input);
    // ...
}
```

### Factory pattern

```csharp
Func<INuciTextNormaliser> factory = () => new NuciTextNormaliser();
// or with DI container
Func<INuciTextNormaliser> factory = () => serviceProvider.GetRequiredService<INuciTextNormaliser>();
```

### Decorator pattern (for cross-cutting concerns)

```csharp
public class LoggingNormaliser : INuciTextNormaliser
{
    private readonly INuciTextNormaliser _inner;
    private readonly ILogger _logger;

    public LoggingNormaliser(INuciTextNormaliser inner, ILogger logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public string NormaliseSentence(string input)
    {
        _logger.LogDebug("Normalising: {Input}", input);
        var result = _inner.NormaliseSentence(input);
        _logger.LogDebug("Result: {Result}", result);
        return result;
    }
}
```

## External integrations

### None by design

The library intentionally avoids:
- External service calls
- Database access
- File system operations
- Network communication
- Third-party APIs
- Event publishing/subscribing
- Message queues
- Caching layers (Redis, etc.)

## Build system integration

### MSBuild/SDK-style project

Uses standard .NET SDK project format:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <!-- ... -->
  </PropertyGroup>
</Project>
```

### CI integration

GitHub Actions workflow:
```yaml
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v2
    - uses: actions/setup-dotnet@v1
      with:
        dotnet-version: 10.0.x
    - run: dotnet restore
    - run: dotnet build --no-restore
    - run: dotnet test --no-build --verbosity normal
```

## Runtime integration

### Process model

Loads into consumer's process:
- No separate service
- No out-of-process communication
- No inter-process communication (IPC)
- Shared memory only (references)

### AppDomain

Runs in default AppDomain unless consumer creates isolated domains.

### Assembly loading

Loaded as part of consumer's assembly resolution:
```csharp
// When consumer references NuciText.Normalisation
Assembly.Load("NuciText.Normalisation");
```

## Versioning integration

### Semantic versioning

Follows SemVer via `<Version>` in csproj:
```xml
<Version>1.0.1</Version>
```

### NuGet version resolution

Consumers specify version:
```xml
<PackageReference Include="NuciText.Normalisation" Version="1.0.1" />
```

### Breaking changes

Any change to normalisation output requires major version bump.

## Testing integration

### Test frameworks

Uses NUnit:
- **Framework:** NUnit 4.5.0
- **Adapter:** NUnit3TestAdapter 6.1.0
- **Test host:** Microsoft.NET.Test.Sdk 18.3.0

### Test project structure

References library project:
```xml
<ItemGroup>
  <ProjectReference Include="..\NuciText.Normalisation\NuciText.Normalisation.csproj" />
</ItemGroup>
```

### Mocking

Moq available for interface mocking:
```csharp
var mock = new Mock<INuciTextNormaliser>();
mock.Setup(x => x.NormaliseSentence("input")).Returns("expected");
```

## IDE/tooling integration

### Visual Studio

- Standard C# project
- IntelliSense works (XML docs if enabled)
- Debugging supported
- Unit test explorer integration via NUnit adapter

### VS Code

- C# extension support
- Debugging with .NET Core debugger
- Test discovery via NUnit

### ReSharper/Rider

- Full support for .NET projects
- Code analysis, refactoring, unit testing

## Security integration

### No security features

The library does not:
- Perform authentication
- Perform authorisation
- Encrypt/decrypt data
- Hash passwords
- Validate tokens
- Check permissions

### Consumer responsibility

Consumers must handle:
- Input validation (if from untrusted source)
- Output encoding (for HTML/JS contexts)
- Authentication/authorisation boundaries

## Observability integration

### None built-in

The library has:
- No logging
- No metrics
- No tracing
- No health checks
- No performance counters

### Consumer can add

```csharp
public class ObservableNormaliser : INuciTextNormaliser
{
    private readonly INuciTextNormaliser _inner;
    private readonly IMetrics _metrics;

    public string NormaliseSentence(string input)
    {
        _metrics.Counter("normalisation_calls_total");
        var result = _inner.NormaliseSentence(input);
        _metrics.Counter("normalisation_success_total");
        return result;
    }
}
```

## Ecosystem integration

### .NET ecosystem

- Targets .NET 10.0
- Uses BCL only
- Compatible with any .NET 10.0+ application
- Works with ASP.NET Core, WPF, WinForms, Console, MAUI, Blazor

### Package ecosystem

- Available on NuGet.org
- No dependencies → no dependency conflicts
- Compatible with any package manager (dotnet CLI, Paket, NuGet.exe)

## Integration testing

### Contract tests

Verify interface contract:
```csharp
[Test]
public void NullInput_ReturnsNull()
    => Assert.That(normaliser.NormaliseSentence(null), Is.Null);
```

### Behaviour tests

Verify normalisation behaviour:
```csharp
[TestCase("hello world", "Hello world.")]
public void GivenInput_ReturnsExpected(string input, string expected)
    => Assert.That(normaliser.NormaliseSentence(input), Is.EqualTo(expected));
```

### Integration tests (consumer side)

Test consumer's use of normaliser:
```csharp
[Test]
public void MyService_ProcessesInputCorrectly()
{
    // Arrange
    var normaliser = new Mock<INuciTextNormaliser>();
    normaliser.Setup(x => x.NormaliseSentence("input")).Returns("processed");
    var service = new MyService(normaliser.Object);

    // Act
    var result = service.Process("input");

    // Assert
    Assert.That(result, Is.EqualTo("processed"));
}
```

## Integration boundaries

| Boundary | Type | Description |
|----------|------|-------------|
| Consumer ↔ Library | Interface | `INuciTextNormaliser` |
| Library ↔ BCL | Static calls | `Regex.Replace`, `string` methods |
| Test ↔ Library | Project reference | Direct instantiation |
| Library ↔ NuGet | Package | `NuciText.Normalisation.nupkg` |
| Library ↔ CI | Workflow | `.github/workflows/dotnet.yml` |

## Integration risks

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| Version conflict | Low | Medium | Use exact version in PackageReference |
| Interface change | Low | High | Major version bump on breaking change |
| BCL API change | Very Low | Medium | .NET guarantees backward compatibility |
| Test framework change | Low | Low | Update test project csproj |
| NuGet feed issue | Low | Medium | Verify feed availability |

## Integration best practices

### For consumers

1. **Depend on abstraction** — Use `INuciTextNormaliser`, not concrete class
2. **Use DI** — Register as singleton (stateless)
3. **Handle null** — Check for null return or use null-coalescing
4. **Don't cache** — No benefit (stateless, fast)
5. **Validate input** — If from untrusted source, check length/content
6. **Encode output** — For HTML/JS contexts, encode the result

### For maintainers

1. **Keep interface stable** — Avoid adding methods
2. **Keep implementation pure** — No side effects, no state
3. **Document behaviour** — Clearly specify transformations
4. **Version carefully** — Bump major on behaviour change
5. **Test thoroughly** — Maintain test coverage
6. **Keep dependency-free** — Avoid adding NuGet dependencies

## Summary

`NuciText.Normalisation` integrates through:
- **Primary:** Public interface `INuciTextNormaliser.NormaliseSentence`
- **Secondary:** DI registration, NuGet package, test project reference
- **None:** External services, databases, networks, files
- **Characteristics:** Minimal coupling, zero configuration, pure function

The library is designed to be a simple, dependable building block in larger .NET applications.