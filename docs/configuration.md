# Configuration

## Configuration sources

This library has **no configuration**. There are no:
- `appsettings.json` files
- Environment variables
- Command-line arguments
- Configuration providers
- Options pattern
- Settings classes

## Configuration surface

| Setting | Type | Default | Description |
|---------|------|---------|-------------|
| None | — | — | — |

## Behaviour modifiers

None. The normalisation pipeline is entirely deterministic and hardcoded.

## Target framework

| Property | Value |
|----------|-------|
| Target framework | `net10.0` |
| Implicit usings | Disabled (`<ImplicitUsings>disable</ImplicitUsings>`) |
| Nullable reference types | Not explicitly enabled |

## Package metadata

| Property | Value |
|----------|-------|
| Package ID | `NuciText.Normalisation` |
| Version | `1.0.1` |
| Description | `NuciText Normalisation` |
| Authors | Horațiu Mlendea |
| Copyright | `Copyright 2026 © Horațiu Mlendea` |
| Repository URL | `https://github.com/hmlendea/nucitext.normalisation` |
| License | `GPL-3.0-or-later` |
| Package tags | `Text Normalisation` |

## Build configurations

| Configuration | Platform | Description |
|---------------|----------|-------------|
| Debug | Any CPU | Debug build |
| Release | Any CPU | Release build |

Defined in `NuciText.Normalisation.sln` and inherited from SDK defaults.

## CI configuration

File: `.github/workflows/dotnet.yml`

| Setting | Value |
|---------|-------|
| Trigger | Push and PR to `master` |
| Runner | `ubuntu-latest` |
| .NET version | `10.0.x` |
| Steps | Restore → Build → Test |

## Runtime configuration

None. The library is a pure function with no runtime state.

## Environment variables

None consumed.

## Extensibility via configuration

None. The library is intentionally closed and non-configurable.