# Architecture

## High-level decomposition

```
┌─────────────────────────────────────────────────────────────┐
│                    NuciText.Normalisation                   │
│  ┌─────────────────────────────────────────────────────┐   │
│  │              INuciTextNormaliser (interface)        │   │
│  │  + NormaliseSentence(string): string                │   │
│  └─────────────────────────────────────────────────────┘   │
│                           ▲                                 │
│                           │ implements                      │
│  ┌─────────────────────────────────────────────────────┐   │
│  │            NuciTextNormaliser (sealed class)        │   │
│  │  + NormaliseSentence(string): string                │   │
│  │                                                     │   │
│  │  Pipeline:                                          │   │
│  │  1. Null/whitespace check → return null             │   │
│  │  2. Trim                                            │   │
│  │  3. Collapse repeated spaces (loop)                 │   │
│  │  4. Remove spaces before punctuation (regex)        │   │
│  │  5. Ensure space after punctuation (regex)          │   │
│  │  6. Capitalise sentence starts (regex)              │   │
│  │  7. Append period if missing (regex)                │   │
│  └─────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
```

## Component relationships

| Component | Type | Responsibility | Dependencies |
|-----------|------|----------------|--------------|
| `INuciTextNormaliser` | Interface | Public contract, single method | None |
| `NuciTextNormaliser` | Sealed class | Implementation of normalisation pipeline | `System.Text.RegularExpressions` |

## Dependency direction

```
Consumer code
      │
      ▼
INuciTextNormaliser (abstraction)
      ▲
      │ implements
      │
NuciTextNormaliser (concrete)
      │
      ▼
System.Text.RegularExpressions (BCL)
```

- **Abstraction depends on nothing** — `INuciTextNormaliser` has zero dependencies
- **Implementation depends on BCL only** — `Regex` from `System.Text.RegularExpressions`
- **Tests depend on implementation** — test project references library project

## Architectural style

- **Single-responsibility library** — one interface, one implementation, one method
- **Stateless functional core** — no instance state, no side effects, thread-safe
- **Interface-based contract** — enables mocking, substitution, DI registration
- **Sealed implementation** — prevents inheritance, guarantees behaviour
- **Zero external dependencies** — only BCL, no NuGet dependencies in library

## Runtime topology

```
┌─────────────┐     ┌──────────────────────┐     ┌──────────────────┐
│  Consumer   │────▶│ INuciTextNormaliser  │────▶│ NuciTextNormaliser│
│  (DI/ctor)  │     │   (interface)        │     │   (sealed class) │
└─────────────┘     └──────────────────────┘     └────────┬─────────┘
                                                          │
                                                          ▼
                                                ┌──────────────────┐
                                                │ System.Text.     │
                                                │ RegularExpressions│
                                                └──────────────────┘
```

## Cross-cutting concerns

| Concern | Handling |
|---------|----------|
| **Error handling** | No exceptions thrown; null input → null output |
| **Logging** | None |
| **Telemetry** | None |
| **Configuration** | None |
| **Localisation** | Invariant culture `ToUpper()`; no culture parameter |
| **Thread safety** | Stateless → inherently thread-safe |
| **Performance** | O(n) with small constant; regex allocations per call |

## Extension points

None by design. The library is intentionally closed:
- Interface has single method
- Implementation is `sealed`
- No virtual methods
- No events, callbacks, or delegates
- No configuration surface

## Integration boundaries

| Boundary | Type | Description |
|----------|------|-------------|
| Consumer ↔ Library | Interface | `INuciTextNormaliser` injected or instantiated |
| Library ↔ BCL | Static calls | `Regex.Replace`, `Regex.IsMatch`, `string` methods |
| Test ↔ Library | Project reference | Direct instantiation of `NuciTextNormaliser` |

## Deployment architecture

```
NuGet Package (NuciText.Normalisation)
    │
    ├── lib/net10.0/NuciText.Normalisation.dll
    ├── NuciText.Normalisation.pdb (optional)
    └── NuciText.Normalisation.xml (optional, doc comments)
```

Single assembly, no native assets, no content files, no build props/targets.

## Security architecture

- **No input validation beyond null check** — trusts caller to provide string
- **No file I/O, network, or process execution** — pure in-memory transformation
- **No deserialization** — no object graph construction from input
- **Regex patterns are constant** — no user-supplied patterns (ReDoS not applicable)
- **GPL-3.0-or-later** — copyleft; consumers must comply

## Observability

None built-in. Consumers may wrap calls for logging/metrics if needed.