# NuciTextNormaliser component

## Overview

`NuciTextNormaliser` is the sole implementation of `INuciTextNormaliser`. It is a `sealed`, stateless class that executes a seven-stage normalisation pipeline on input strings.

## Source location

- **File:** `NuciText.Normalisation/NuciTextNormaliser.cs`
- **Namespace:** `NuciText.Normalisation`
- **Assembly:** `NuciText.Normalisation.dll`

## Class definition

```csharp
public sealed class NuciTextNormaliser : INuciTextNormaliser
{
    public string NormaliseSentence(string input)
    {
        // ... implementation
    }
}
```

## Characteristics

| Property | Value |
|----------|-------|
| **Accessibility** | `public` |
| **Inheritance** | `sealed` (cannot be inherited) |
| **Interfaces** | `INuciTextNormaliser` |
| **Constructors** | Implicit parameterless only |
| **Fields** | None (no instance, no static) |
| **Properties** | None |
| **Methods** | `NormaliseSentence` (interface implementation) |
| **Thread safety** | Yes (stateless) |
| **State** | None (pure function) |

## Method: NormaliseSentence

### Signature

```csharp
public string NormaliseSentence(string input)
```

### Parameters

| Name | Type | Nullable | Description |
|------|------|----------|-------------|
| `input` | `string` | Yes | Raw sentence to normalise |

### Returns

| Type | Nullable | Description |
|------|----------|-------------|
| `string` | Yes | Normalised sentence, or `null` if input is null/whitespace |

### Implementation

```csharp
public string NormaliseSentence(string input)
{
    if (string.IsNullOrWhiteSpace(input))
    {
        return null;
    }

    string result = input.Trim();

    // Remove repeated spaces
    while (result.Contains("  "))
    {
        result = result.Replace("  ", " ");
    }

    // Remove spaces before punctuation
    result = Regex.Replace(result, @"\s+([.,!?;:])", "$1");

    // Ensure space after in-sentence punctuation
    result = Regex.Replace(result, @"([,!?;:])([^\s\d])", "$1 $2");

    // Capitalise first letter of each sentence
    result = Regex.Replace(result, @"(^\s*[a-z])|([.!?]\s+[a-z])", match =>
    {
        return match.Value.ToUpper();
    });

    // Append period if no ending punctuation
    if (!Regex.IsMatch(result, @"[.!?]$"))
    {
        result += ".";
    }

    return result;
}
```

### Pipeline stages (inline)

| Line | Stage | Operation |
|------|-------|-----------|
| 3-6 | 1 | Null/whitespace guard |
| 8 | 2 | `Trim()` |
| 11-14 | 3 | Collapse repeated spaces (while loop) |
| 17 | 4 | `Regex.Replace` — remove space before punctuation |
| 20 | 5 | `Regex.Replace` — ensure space after punctuation |
| 23-26 | 6 | `Regex.Replace` with lambda — capitalise sentence starts |
| 29-31 | 7 | `Regex.IsMatch` + append — terminal period |

## Dependencies

| Dependency | Type | Usage |
|------------|------|-------|
| `System.Text.RegularExpressions.Regex` | BCL static | Stages 4, 5, 6, 7 |
| `string` methods | BCL instance | Stages 2, 3, 7 |

**No injected dependencies.** No constructor parameters, no service location.

## Test mapping

| Test method | Pipeline stages covered |
|-------------|------------------------|
| `GivenANullOrWhitespaceString_WhenNormalisingSentence_ThenTheResultIsEmpty` | Stage 1 |
| `GivenAValidString_WhenNormalisingSentence_ThenTheResultIsNotNull` | Stages 1-7 (smoke) |
| `GivenAValidString_WhenNormalisingSentence_ThenTheResultIsNotEmpty` | Stages 1-7 (smoke) |
| `GivenAValidString_WhenNormalisingSentence_ThenTheResultHasBeenNormaliseSentenced` | All stages (7 parameterised cases) |

### Test cases → implementation lines

| Test case | Input | Expected | Key lines exercised |
|-----------|-------|----------|---------------------|
| `"   Hello   world!   "` | `"Hello world!"` | 8 (trim), 11-14 (collapse), 23-26 (capitalise) |
| `"Should we add a period here?"` | Same | 29-31 (no append, ends with `?`) |
| `"Test sentence is testy"` | `"Test sentence is testy."` | 29-31 (append `.`) |
| `"This is a test.  "` | `"This is a test."` | 8 (trim), 29-31 (append) |
| `"this one should be capitalised. yes it should!"` | `"This one should be capitalised. Yes it should!"` | 23-26 (capitalise after `.`) |
| `"Package names such as NuciText.Normalisation..."` | Same | 20 (no space after `.` before digit) |
| `"Punctuation not added after version numbers such as 1.2.3..."` | Same | 20 (version protection) |

## Instantiation patterns

### Direct instantiation

```csharp
var normaliser = new NuciTextNormaliser();
string result = normaliser.NormaliseSentence(input);
```

### Dependency injection

```csharp
// Registration
services.AddSingleton<INuciTextNormaliser, NuciTextNormaliser>();

// Consumption
public class MyService
{
    private readonly INuciTextNormaliser _normaliser;

    public MyService(INuciTextNormaliser normaliser)
    {
        _normaliser = normaliser;
    }
}
```

### Factory (if needed)

```csharp
// Not needed — stateless, no configuration
Func<INuciTextNormaliser> factory = () => new NuciTextNormaliser();
```

## Performance profile

| Metric | Value |
|--------|-------|
| **Allocations/call** | ~5-15 strings + regex objects |
| **Time complexity** | O(n) typical, O(n²) worst case (space collapse) |
| **GC pressure** | Low (short-lived strings, gen 0) |
| **Thread contention** | None |
| **Cache behaviour** | Good (small strings, sequential access) |

## Optimisation opportunities

| Stage | Current | Improved |
|-------|---------|----------|
| 3 | `while (Contains) Replace` | `Regex.Replace(input, @"\s{2,}", " ")` or `StringBuilder` |
| 4-6 | 3 regex passes | Single pass with `MatchEvaluator` |
| 7 | `Regex.IsMatch` | `result.EndsWith('.') \|\| result.EndsWith('!') \|\| result.EndsWith('?')` |

## Extensibility

**None by design.** The class is:
- `sealed` — no inheritance
- No virtual methods — no override points
- No events/callbacks — no extension hooks
- No configuration — fixed behaviour

To customise behaviour, consumers must:
1. Wrap/decorate the normaliser
2. Pre/post-process input/output
3. Fork the library

## Usage in tests

```csharp
// Direct instantiation (current test approach)
[SetUp]
public void Setup()
{
    normaliser = new NuciTextNormaliser();
}

// Via interface (for interface contract tests)
INuciTextNormaliser normaliser = new NuciTextNormaliser();
```

## Mocking

Not needed for unit tests (stateless, no dependencies). Moq available in test project for interface mocking if testing consumers.

```csharp
// Example: mocking for consumer tests
var mock = new Mock<INuciTextNormaliser>();
mock.Setup(x => x.NormaliseSentence("input")).Returns("Output");
```

## Diagnostics

**None built-in.** To add observability:

```csharp
public class LoggingNormaliser : INuciTextNormaliser
{
    private readonly INuciTextNormaliser _inner;
    private readonly ILogger _logger;

    public string NormaliseSentence(string input)
    {
        _logger.LogDebug("Normalising: {Input}", input);
        var result = _inner.NormaliseSentence(input);
        _logger.LogDebug("Normalised: {Result}", result);
        return result;
    }
}
```

## Version history

| Version | Changes |
|---------|---------|
| 1.0.0 | Initial release |
| 1.0.1 | Current (no code changes visible in git history) |

## Related documentation

- [INuciTextNormaliser interface](./inuci-text-normaliser.md)
- [Normalise sentence behaviour](../behaviour/normalise-sentence.md)
- [Normalise sentence flow](../flows/normalise-sentence.md)
- [Invariants](../invariants.md)
- [Testing](../testing.md)