# Repository structure

## Source tree

```
nucitext.normalisation/
├── .github/
│   ├── FUNDING.yml
│   └── workflows/
│       └── dotnet.yml
├── NuciText.Normalisation/
│   ├── INuciTextNormaliser.cs
│   ├── NuciTextNormaliser.cs
│   └── NuciText.Normalisation.csproj
├── NuciText.Normalisation.UnitTests/
│   ├── NuciTextNormaliserTests.cs
│   └── NuciText.Normalisation.UnitTests.csproj
├── NuciText.Normalisation.sln
├── LICENSE
├── README.md
└── .gitignore
```

## Module organisation

### NuciText.Normalisation (library)

| File | Purpose |
|------|---------|
| `INuciTextNormaliser.cs` | Public interface defining the normalisation contract |
| `NuciTextNormaliser.cs` | Sealed implementation of the normalisation logic |
| `NuciText.Normalisation.csproj` | Project file targeting net10.0, package metadata |

**Namespace:** `NuciText.Normalisation`
**Target framework:** `net10.0`
**Output:** `NuciText.Normalisation.dll` (NuGet package)

### NuciText.Normalisation.UnitTests (test project)

| File | Purpose |
|------|---------|
| `NuciTextNormaliserTests.cs` | NUnit test suite covering normalisation behaviour |
| `NuciText.Normalisation.UnitTests.csproj` | Test project referencing library project and test dependencies |

**Target framework:** `net10.0`
**Test framework:** NUnit 4.5.0 with NUnit3TestAdapter
**Mocking:** Moq 4.20.72 (available but not used in current tests)

### Solution

| File | Purpose |
|------|---------|
| `NuciText.Normalisation.sln` | VS solution referencing both projects |

## Build artefacts

| Location | Description |
|----------|-------------|
| `NuciText.Normalisation/bin/{Configuration}/net10.0/` | Library build output |
| `NuciText.Normalisation.UnitTests/bin/{Configuration}/net10.0/` | Test build output |
| `NuciText.Normalisation/obj/` | Intermediate build files |
| `NuciText.Normalisation.UnitTests/obj/` | Intermediate test build files |

## Configuration files

| File | Purpose |
|------|---------|
| `.github/workflows/dotnet.yml` | CI pipeline: restore, build, test on push/PR to master |
| `.github/FUNDING.yml` | GitHub funding links |
| `.gitignore` | Standard Visual Studio ignore patterns |

## Documentation

| File | Purpose |
|------|---------|
| `README.md` | Installation and usage (NuGet badges, CLI commands) |
| `LICENSE` | GPL-3.0-or-later license text |
| `docs/` | This documentation tree |

## Naming conventions

- **Projects:** PascalCase with `NuciText.` prefix
- **Interfaces:** `I` prefix + PascalCase (`INuciTextNormaliser`)
- **Implementations:** PascalCase, no suffix (`NuciTextNormaliser`)
- **Methods:** PascalCase (`NormaliseSentence`)
- **Test classes:** `{TypeUnderTest}Tests` (`NuciTextNormaliserTests`)
- **Test methods:** `Given{Context}_When{Action}_Then{Outcome}` pattern

## Dependency graph

```
NuciText.Normalisation.UnitTests
    |
    +-- NuciText.Normalisation (project reference)
    |
    +-- Microsoft.NET.Test.Sdk
    +-- NUnit
    +-- NUnit3TestAdapter
    +-- Moq

NuciText.Normalisation
    |
    +-- System.Text.RegularExpressions (BCL, implicit)
```

No third-party dependencies in the library project.