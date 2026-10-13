# Repository overview

## Purpose

`NuciText.Normalisation` provides a single, focused text normalisation capability for .NET applications. It transforms raw sentence input into a consistently formatted output by applying a deterministic pipeline of whitespace handling, punctuation spacing, capitalisation, and terminal punctuation enforcement.

## Scope

**In scope:**
- Single public interface: `INuciTextNormaliser`
- Single implementation: `NuciTextNormaliser`
- Single method: `NormaliseSentence(string input) -> string`
- Input: a single sentence or short text fragment (string)
- Output: normalised string or `null` for null/whitespace input
- Target framework: .NET 10.0
- Distribution: NuGet package `NuciText.Normalisation`

**Out of scope:**
- Multi-paragraph or document-level normalisation
- Locale-specific capitalisation rules (uses invariant culture `ToUpper`)
- HTML/markdown parsing or sanitisation
- Streaming or chunked processing
- Configuration or extensibility points
- Logging, telemetry, or diagnostics
- Async API surface

## Entry points

| Entry point | Location | Description |
|-------------|----------|-------------|
| `INuciTextNormaliser` | `NuciText.Normalisation/INuciTextNormaliser.cs` | Public contract |
| `NuciTextNormaliser` | `NuciText.Normalisation/NuciTextNormaliser.cs` | Default implementation |
| `NormaliseSentence` | `NuciTextNormaliser.NormaliseSentence` | Sole public operation |

## Runtime topology

```
Consumer
    |
    v
INuciTextNormaliser (interface)
    |
    v
NuciTextNormaliser (sealed class)
    |
    v
System.Text.RegularExpressions (BCL)
```

No dependency injection, no external services, no configuration providers, no background threads.

## Major state

None. The implementation is stateless and thread-safe. Every call to `NormaliseSentence` is independent.

## Major integrations

- **NuGet** — package publishing (manual or CI)
- **GitHub Actions** — CI build and test (`.github/workflows/dotnet.yml`)
- **NUnit** — unit test framework (test project only)

## System-wide invariants

1. **Null/whitespace in → null out** — `NormaliseSentence` returns `null` for `null`, empty, or whitespace-only input.
2. **Deterministic output** — identical input always produces identical output.
3. **No exceptions** — the method never throws for any string input.
4. **Single sentence assumption** — the algorithm treats input as a single sentence; multiple sentences are capitalised individually but not separated.
5. **Invariant culture capitalisation** — `ToUpper()` uses current culture; no explicit culture is specified.

## Versioning

- Semantic versioning via `<Version>` in `NuciText.Normalisation.csproj` (currently `1.0.1`)
- No API versioning in the interface; breaking changes require major version bump

## License

GPL-3.0-or-later — see [LICENSE](../LICENSE).

## Author

Horațiu Mlendea — <https://github.com/hmlendea>