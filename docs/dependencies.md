# Dependencies

## External dependencies

### NuciText.Normalisation (library project)

| Dependency | Type | Version | Purpose |
|------------|------|---------|---------|
| `System.Text.RegularExpressions` | BCL (built-in) | .NET 10.0 | Regex operations for punctuation spacing and capitalisation |

**No NuGet package dependencies.** The library uses only .NET Base Class Library types.

### NuciText.Normalisation.UnitTests (test project)

| Dependency | Type | Version | Purpose |
|------------|------|---------|---------|
| `Microsoft.NET.Test.Sdk` | NuGet | 18.3.0 | Test host and discovery |
| `NUnit` | NuGet | 4.5.0 | Test framework |
| `NUnit3TestAdapter` | NuGet | 6.1.0 | VS Test Platform adapter for NUnit |
| `Moq` | NuGet | 4.20.72 | Mocking framework (available, not used) |
| `NuciText.Normalisation` | Project reference | Local | System under test |

## Internal dependencies

```
NuciText.Normalisation.UnitTests
    └── NuciText.Normalisation (project reference)
```

## Dependency graph

```
┌─────────────────────────────────────────────────────────────┐
│                    NuciText.Normalisation                   │
│  ┌─────────────────────────────────────────────────────┐   │
│  │ INuciTextNormaliser.cs                              │   │
│  │ NuciTextNormaliser.cs                               │   │
│  │   └── System.Text.RegularExpressions (BCL)          │   │
│  └─────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
                              ▲
                              │ ProjectReference
                              │
┌─────────────────────────────────────────────────────────────┐
│            NuciText.Normalisation.UnitTests                 │
│  ┌─────────────────────────────────────────────────────┐   │
│  │ NuciTextNormaliserTests.cs                          │   │
│  │   └── NuciText.Normalisation (project ref)          │   │
│  │   └── NUnit, NUnit3TestAdapter, Moq, Test.Sdk       │   │
│  └─────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
```

## Transitive dependencies

None for the library project. The test project's transitive dependencies are standard test infrastructure.

## Dependency management

| Aspect | Detail |
|--------|--------|
| Package restore | `dotnet restore` (via CI) |
| Version pinning | Explicit versions in test `.csproj` |
| Central package management | Not used |
| Dependency validation | None automated |
| License compliance | GPL-3.0-or-later (library); test deps MIT/Apache-2.0 |

## Vulnerability scanning

Not configured. Recommended: add `dotnet list package --vulnerable` to CI.

## Upgrade policy

| Dependency | Policy |
|------------|--------|
| .NET runtime | Follow .NET LTS/releases; currently net10.0 |
| Test packages | Update with major .NET upgrades or security fixes |
| BCL | Automatic with .NET SDK |

## Compatibility

| Target | Compatible |
|--------|------------|
| .NET 10.0 | Yes (target) |
| .NET 9.0 | Not tested; likely compatible (no 10.0-specific APIs) |
| .NET 8.0 | Not tested; likely compatible |
| .NET Framework | No (net10.0 only) |
| .NET Standard | No (net10.0 only) |

## Native dependencies

None. Pure managed code.

## Analyzer dependencies

None configured. Recommended: enable `Microsoft.CodeAnalysis.NetAnalyzers`.