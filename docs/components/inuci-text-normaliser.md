# INuciTextNormaliser interface

## Overview

`INuciTextNormaliser` is the public contract for text normalisation in this library. It defines a single method for normalising sentence-level text input.

## Source location

- **File:** `NuciText.Normalisation/INuciTextNormaliser.cs`
- **Namespace:** `NuciText.Normalisation`
- **Assembly:** `NuciText.Normalisation.dll`

## Interface definition

```csharp
public interface INuciTextNormaliser
{
    string NormaliseSentence(string input);
}
```

## Characteristics

| Property | Value |
|----------|-------|
| **Accessibility** | `public` |
| **Members** | 1 method |
| **Properties** | None |
| **Events** | None |
| **Generic parameters** | None |
| **Inheritance** | None (no base interface) |

## Method: NormaliseSentence

### Signature

```csharp
string NormaliseSentence(string input);
```

### Parameters

| Name | Type | Nullable | Description |
|------|------|----------|-------------|
| `input` | `string` | Yes | The input sentence to normalise. Can be `null`, empty, whitespace, or any string. |

### Returns

| Type | Nullable | Description |
|------|----------|-------------|
| `string` | Yes | The normalised sentence, or `null` if input is null/whitespace. |

### Contract

#### Preconditions
- None. Accepts any `string` value including `null`.

#### Postconditions
1. **Null/whitespace in → null out:** If `string.IsNullOrWhiteSpace(input)` is true, returns `null`.
2. **Valid in → non-null out:** If input passes guard, returns non-null `string`.
3. **Valid in → non-empty out:** Return value is never `""`.
4. **Deterministic:** Same input always produces same output.
5. **No exceptions:** Method never throws for any string input.
6. **Normalised output:** Return value satisfies all normalisation invariants (trimmed, no repeated spaces, correct punctuation spacing, capitalised sentences, terminal punctuation).

#### Side effects
- None. Pure function.

#### Thread safety
- Implementations must be thread-safe (stateless or properly synchronised).

## Implementations

| Implementation | Location | Notes |
|----------------|----------|-------|
| `NuciTextNormaliser` | `NuciText.Normalisation/NuciTextNormaliser.cs` | Sealed, stateless, default implementation |

## Usage patterns

### Direct usage

```csharp
INuciTextNormaliser normaliser = new NuciTextNormaliser();
string result = normaliser.NormaliseSentence(input);
```

### Dependency injection

```csharp
// Registration
services.AddSingleton<INuciTextNormaliser, NuciTextNormaliser>();

// Constructor injection
public class Consumer
{
    private readonly INuciTextNormaliser _normaliser;

    public Consumer(INuciTextNormaliser normaliser)
    {
        _normaliser = normaliser;
    }
}
```

### Factory pattern

```csharp
// For scoped/lifetime control (not needed for stateless impl)
Func<INuciTextNormaliser> factory = () => new NuciTextNormaliser();
```

## Testing the interface

### Contract tests (recommended)

```csharp
[TestFixture]
public class INuciTextNormaliserContractTests
{
    private INuciTextNormaliser _normaliser;

    [SetUp]
    public void Setup() => _normaliser = new NuciTextNormaliser();

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void NullOrWhitespace_ReturnsNull(string input)
        => Assert.That(_normaliser.NormaliseSentence(input), Is.Null);

    [Test]
    public void ValidInput_ReturnsNonNull()
        => Assert.That(_normaliser.NormaliseSentence("test"), Is.Not.Null);

    [Test]
    public void ValidInput_ReturnsNonEmpty()
        => Assert.That(_normaliser.NormaliseSentence("test"), Is.Not.Empty);

    [Test]
    public void Deterministic_SameInputSameOutput()
    {
        var input = "hello world";
        var r1 = _normaliser.NormaliseSentence(input);
        var r2 = _normaliser.NormaliseSentence(input);
        Assert.That(r1, Is.EqualTo(r2));
    }

    [Test]
    public void NoExceptions_AnyStringInput()
    {
        // Should not throw
        _normaliser.NormaliseSentence(null);
        _normaliser.NormaliseSentence("");
        _normaliser.NormaliseSentence("x");
        _normaliser.NormaliseSentence(new string('a', 10000));
    }
}
```

### Mocking for consumer tests

```csharp
var mock = new Mock<INuciTextNormaliser>();
mock.Setup(x => x.NormaliseSentence("specific input")).Returns("expected output");
mock.Setup(x => x.NormaliseSentence(It.IsAny<string>())).Returns((string s) => s?.ToUpper());

// Verify interaction
mock.Verify(x => x.NormaliseSentence("input"), Times.Once);
```

## Design rationale

### Why an interface?

1. **Testability** — Consumers can mock the normaliser
2. **Substitutability** — Alternative implementations possible (logging, caching, different rules)
3. **DI friendly** — Standard .NET DI pattern
4. **Decoupling** — Consumers depend on abstraction, not concrete class

### Why single method?

- **Interface segregation** — One responsibility: sentence normalisation
- **Minimal surface** — Easy to implement, mock, verify
- **No configuration** — No `Configure`, `Initialize`, or options methods needed

### Why nullable return?

- **Semantic distinction** — `null` = "no valid input" vs `""` = "normalised empty" (impossible)
- **Null-coalescing friendly** — `normaliser.NormaliseSentence(input) ?? ""`
- **Explicit null handling** — Forces caller to consider empty case

## Versioning

| Version | Changes |
|---------|---------|
| 1.0.0 | Initial interface |
| Current | Unchanged since 1.0.0 |

**Breaking change policy:** Any change to method signature, return type nullability, or contract semantics requires major version bump.

## Related documentation

- [NuciTextNormaliser implementation](./nuci-text-normaliser.md)
- [Normalise sentence behaviour](../behaviour/normalise-sentence.md)
- [Normalise sentence flow](../flows/normalise-sentence.md)
- [Invariants](../invariants.md)
- [Architecture](../architecture.md)