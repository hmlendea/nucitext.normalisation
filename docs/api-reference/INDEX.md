# API reference index

## Overview

This library exposes a single public interface with one method. There are no HTTP endpoints, controllers, or web APIs.

## Public API surface

| Type | Kind | Members |
|------|------|---------|
| `INuciTextNormaliser` | Interface | `NormaliseSentence(string): string` |
| `NuciTextNormaliser` | Class (sealed) | Implements `INuciTextNormaliser` |

## Method reference

### `INuciTextNormaliser.NormaliseSentence`

**Signature:**
```csharp
string NormaliseSentence(string input);
```

**Parameters:**

| Name | Type | Required | Description |
|------|------|----------|-------------|
| `input` | `string` | Yes | The input sentence to normalise. Accepts `null`, empty, whitespace, or any string. |

**Returns:**

| Type | Nullable | Description |
|------|----------|-------------|
| `string` | Yes | Normalised sentence, or `null` if input is null/whitespace. |

**Behaviour:**
- Returns `null` for `null`, `""`, or whitespace-only input
- Otherwise applies normalisation pipeline:
  1. Trim whitespace
  2. Collapse repeated spaces
  3. Remove spaces before punctuation (`. , ! ? ; :`)
  4. Ensure space after punctuation (except before digits)
  5. Capitalise first letter of each sentence
  6. Append period if missing terminal punctuation

**Exceptions:** Never throws for any string input.

**Thread safety:** Implementations are thread-safe (stateless).

**Idempotence:** `NormaliseSentence(NormaliseSentence(x)) == NormaliseSentence(x)`

## Usage examples

### Basic usage

```csharp
using NuciText.Normalisation;

INuciTextNormaliser normaliser = new NuciTextNormaliser();
string result = normaliser.NormaliseSentence("  hello   world!  ");
// result = "Hello world!"
```

### Dependency injection

```csharp
// Startup
services.AddSingleton<INuciTextNormaliser, NuciTextNormaliser>();

// Consumer
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

### Null handling

```csharp
string? result = normaliser.NormaliseSentence(null);      // null
string? result = normaliser.NormaliseSentence("");        // null
string? result = normaliser.NormaliseSentence("   ");     // null
string safe = normaliser.NormaliseSentence(input) ?? "";  // never null
```

## Related documentation

| Document | Description |
|----------|-------------|
| [components/inuci-text-normaliser.md](../components/inuci-text-normaliser.md) | Interface deep dive |
| [components/nuci-text-normaliser.md](../components/nuci-text-normaliser.md) | Implementation deep dive |
| [behaviour/normalise-sentence.md](../behaviour/normalise-sentence.md) | Detailed behaviour specification |
| [flows/normalise-sentence.md](../flows/normalise-sentence.md) | Execution flow trace |
| [invariants.md](../invariants.md) | Contract invariants |
| [quick-start.md](../quick-start.md) | Getting started guide |

## Versioning

| Version | API changes |
|---------|-------------|
| 1.0.0 | Initial release |
| 1.0.1 | No API changes |

**Breaking changes:** Any modification to method signature, return nullability, or behavioural contract requires major version bump.

## Package information

| Property | Value |
|----------|-------|
| Package ID | `NuciText.Normalisation` |
| Current version | `1.0.1` |
| Target framework | `net10.0` |
| License | `GPL-3.0-or-later` |
| Repository | `https://github.com/hmlendea/nucitext.normalisation` |