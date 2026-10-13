# Build and deployment

## Build pipeline

### Local build

```bash
# Restore dependencies
dotnet restore

# Build solution (Debug)
dotnet build

# Build solution (Release)
dotnet build --configuration Release

# Build specific project
dotnet build NuciText.Normalisation/NuciText.Normalisation.csproj
```

### CI build (GitHub Actions)

File: `.github/workflows/dotnet.yml`

```yaml
name: .NET

on:
  push:
    branches: [ master ]
  pull_request:
    branches: [ master ]

jobs:
  build:
    name: Build
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v2
    - name: Setup .NET
      uses: actions/setup-dotnet@v1
      with:
        dotnet-version: 10.0.x
    - name: Restore dependencies
      run: dotnet restore
    - name: Build
      run: dotnet build --no-restore
    - name: Test
      run: dotnet test --no-build --verbosity normal
```

**Triggers:** Push to `master`, PR to `master`
**Runner:** `ubuntu-latest`
**.NET SDK:** `10.0.x` (latest 10.0 patch)
**Steps:** Checkout → Setup .NET → Restore → Build → Test

## Build outputs

| Configuration | Project | Output directory |
|---------------|---------|------------------|
| Debug | NuciText.Normalisation | `NuciText.Normalisation/bin/Debug/net10.0/` |
| Release | NuciText.Normalisation | `NuciText.Normalisation/bin/Release/net10.0/` |
| Debug | NuciText.Normalisation.UnitTests | `NuciText.Normalisation.UnitTests/bin/Debug/net10.0/` |
| Release | NuciText.Normalisation.UnitTests | `NuciText.Normalisation.UnitTests/bin/Release/net10.0/` |

### Library artefacts (Release)

| File | Description |
|------|-------------|
| `NuciText.Normalisation.dll` | Main assembly |
| `NuciText.Normalisation.pdb` | Debug symbols (if `<DebugType>portable</DebugType>`) |
| `NuciText.Normalisation.xml` | XML doc comments (if `<GenerateDocumentationFile>true</GenerateDocumentationFile>`) |
| `NuciText.Normalisation.deps.json` | Dependency manifest |

**Note:** XML docs and PDB not currently enabled in csproj.

## NuGet package

### Package creation

```bash
# From library project directory
dotnet pack --configuration Release --output ./nupkg
```

### Package contents

```
NuciText.Normalisation.1.0.1.nupkg
├── lib/net10.0/
│   ├── NuciText.Normalisation.dll
│   └── NuciText.Normalisation.pdb (optional)
├── NuciText.Normalisation.nuspec (embedded)
└── _rels/, package/ (NuGet metadata)
```

### Package metadata (from csproj)

| Property | Value |
|----------|-------|
| Package ID | `NuciText.Normalisation` |
| Version | `1.0.1` |
| Title | (not set, uses ID) |
| Description | `NuciText Normalisation` |
| Authors | `Horațiu Mlendea` |
| Copyright | `Copyright 2026 © Horațiu Mlendea` |
| Repository URL | `https://github.com/hmlendea/nucitext.normalisation` |
| License | `GPL-3.0-or-later` |
| Tags | `Text Normalisation` |
| Target framework | `net10.0` |

### Publishing to NuGet

```bash
# Requires API key (set via NUGET_API_KEY env var or nuget.config)
dotnet nuget push ./nupkg/NuciText.Normalisation.1.0.1.nupkg --source https://api.nuget.org/v3/index.json
```

**Current status:** Manual publish. No automated release workflow.

## Versioning

### Scheme

Semantic Versioning (SemVer) via `<Version>` in `NuciText.Normalisation.csproj`:

```xml
<Version>1.0.1</Version>
```

### Version bump rules

| Change type | Version bump | Example |
|-------------|--------------|---------|
| Bug fix, internal improvement | Patch | `1.0.1` → `1.0.2` |
| New feature, non-breaking | Minor | `1.0.1` → `1.1.0` |
| Breaking change (output change, API change) | Major | `1.0.1` → `2.0.0` |

### Pre-release

Not used. All releases are stable.

## Deployment environments

| Environment | Purpose | Trigger |
|-------------|---------|---------|
| Local development | Developer machine | Manual `dotnet build` |
| CI (GitHub Actions) | Validation, test | Push/PR to master |
| NuGet.org | Public distribution | Manual `dotnet nuget push` |

## Release process

### Current (manual)

1. Update `<Version>` in `NuciText.Normalisation.csproj`
2. Update changelog (if exists)
3. Commit and push to `master`
4. CI builds and tests
5. Tag release: `git tag v1.0.1 && git push origin v1.0.1`
6. `dotnet pack --configuration Release`
7. `dotnet nuget push` to NuGet.org
7. Create GitHub Release with notes

### Recommended automation

Add `.github/workflows/nuget-release.yml`:

```yaml
name: NuGet Release

on:
  push:
    tags: [ 'v*' ]

jobs:
  release:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v4
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: 10.0.x
    - name: Restore
      run: dotnet restore
    - name: Pack
      run: dotnet pack NuciText.Normalisation/NuciText.Normalisation.csproj -c Release -o ./nupkg
    - name: Push to NuGet
      run: dotnet nuget push ./nupkg/*.nupkg --source https://api.nuget.org/v3/index.json --api-key ${{ secrets.NUGET_API_KEY }}
```

## Build configuration

### Solution configurations

| Configuration | Platforms |
|---------------|-----------|
| Debug | Any CPU, x64, x86 |
| Release | Any CPU, x64, x86 |

Defined in `NuciText.Normalisation.sln`. All map to `Any CPU` build.

### Project properties (NuciText.Normalisation.csproj)

```xml
<PropertyGroup>
  <TargetFramework>net10.0</TargetFramework>
  <RootNamespace>NuciText.Normalisation</RootNamespace>
  <Version>1.0.1</Version>
  <Description>NuciText Normalisation</Description>
  <Authors>Horațiu Mlendea</Authors>
  <Copyright>Copyright 2026 © Horațiu Mlendea</Copyright>
  <RepositoryUrl>https://github.com/hmlendea/nucitext.normalisation</RepositoryUrl>
  <PackageLicenseExpression>GPL-3.0-or-later</PackageLicenseExpression>
  <PackageTags>Text Normalisation</PackageTags>
  <ImplicitUsings>disable</ImplicitUsings>
</PropertyGroup>
```

### Recommended additions

| Property | Value | Reason |
|----------|-------|--------|
| `<GenerateDocumentationFile>true</GenerateDocumentationFile>` | Enable XML docs | IntelliSense for consumers |
| `<DebugType>portable</DebugType>` | Portable PDB | Debugging in production |
| `<ContinuousIntegrationBuild>true</ContinuousIntegrationBuild>` | Deterministic builds | Reproducible artefacts |
| `<Deterministic>true</Deterministic>` | Deterministic builds | Same output for same source |

## Build validation

### Required checks

| Check | Command | CI |
|-------|---------|----|
| Restore | `dotnet restore` | ✅ |
| Build | `dotnet build --no-restore` | ✅ |
| Test | `dotnet test --no-build` | ✅ |

### Recommended checks

| Check | Command | CI |
|-------|---------|----|
| Format | `dotnet format --verify-no-changes` | ❌ |
| Analyze | `dotnet build /p:RunAnalyzers=true` | ❌ |
| Vulnerable packages | `dotnet list package --vulnerable` | ❌ |
| Outdated packages | `dotnet list package --outdated` | ❌ |

## Performance considerations

| Aspect | Current | Recommendation |
|--------|---------|----------------|
| Build time | ~5-10s | Acceptable |
| Test time | ~2-3s | Acceptable |
| Package size | ~20 KB | Minimal |
| Cold start | N/A (library) | N/A |

## Rollback procedure

1. Identify problematic version on NuGet.org
2. `dotnet nuget delete NuciText.Normalisation <version>` (unlist, not delete)
3. Push previous version if needed
4. Communicate via GitHub Release notes

## Security

| Aspect | Status |
|--------|--------|
| Signed packages | No (not strong-named) |
| Reproducible builds | Not enforced |
| SBOM generation | No |
| Dependency scanning | No |

## Troubleshooting build

| Issue | Resolution |
|-------|------------|
| `dotnet restore` fails | Check network, NuGet.config, feed credentials |
| `dotnet build` fails | Check .NET 10 SDK installed, csproj syntax |
| `dotnet test` fails | Check test project references, NUnit adapter |
| `dotnet pack` fails | Check `<Version>` format, required properties |
| `dotnet nuget push` fails | Check API key, package ID conflicts, version exists |