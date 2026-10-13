# Design decisions

## 1. Single method interface

**Decision:** `INuciTextNormaliser` exposes only `NormaliseSentence(string): string`.

**Rationale:**
- Minimal surface area reduces coupling
- Single responsibility: sentence normalisation only
- Easy to mock, test, and substitute
- No configuration or lifecycle methods needed

**Alternatives considered:**
- Multiple methods for different normalisation levels (rejected: YAGNI)
- `Normalise(string, NormalisationOptions)` (rejected: no options exist)

## 2. Sealed implementation

**Decision:** `NuciTextNormaliser` is `sealed`.

**Rationale:**
- Guarantees behavioural consistency — no subclass can override pipeline
- Enables compiler optimisations (devirtualisation)
- Communicates intent: this is a final algorithm, not a base class
- Prevents fragile base class problems

**Alternatives considered:**
- `virtual` methods for extensibility (rejected: no extension points needed)
- Abstract base class (rejected: single implementation)

## 3. Null/whitespace returns null

**Decision:** `NormaliseSentence` returns `null` for null, empty, or whitespace input.

**Rationale:**
- Distinguishes "no input" from "empty result"
- Avoids allocating empty string for invalid input
- Caller can use null-coalescing: `NormaliseSentence(input) ?? ""`

**Alternatives considered:**
- Return empty string (rejected: loses distinction)
- Throw `ArgumentException` (rejected: violates "no exceptions" principle)

## 4. Iterative space collapsing (while loop)

**Decision:** Stage 3 uses `while (result.Contains("  ")) result = result.Replace("  ", " ")`.

**Rationale:**
- Simple, readable, no regex allocation
- Handles arbitrary repetition counts
- Performance adequate for typical sentence lengths

**Alternatives considered:**
- `Regex.Replace(result, @"\s{2,}", " ")` (rejected: regex allocation per call)
- Single-pass StringBuilder (rejected: premature optimisation)

**Known issue:** O(n²) worst case for pathological input (many spaces). Acceptable for sentence-scale input.

## 5. Regex for punctuation spacing

**Decision:** Stages 4, 5, 6, 7 use `Regex.Replace`/`Regex.IsMatch`.

**Rationale:**
- Declarative patterns are readable and maintainable
- Complex character-class logic (e.g., "not digit") expressible in regex
- BCL regex is well-optimised for these patterns

**Alternatives considered:**
- Manual character iteration (rejected: verbose, error-prone)
- `String.Replace` chains (rejected: cannot express "not digit" condition)

## 6. Invariant culture capitalisation (ToUpper without culture)

**Decision:** Stage 6 uses `match.Value.ToUpper()` without explicit culture.

**Rationale:**
- Simpler code — no `CultureInfo` parameter
- Matches typical "sentence case" expectation for English-like text
- Current thread culture is usually appropriate for UI text

**Alternatives considered:**
- `ToUpperInvariant()` (rejected: may produce unexpected results for non-Latin scripts)
- `ToUpper(CultureInfo.CurrentCulture)` explicit (rejected: verbose, same behaviour)

**Known issue:** Behaviour varies by thread culture. Not documented as locale-aware.

## 7. Version number protection in stage 5

**Decision:** Pattern `([,!?;:])([^\s\d])` excludes digits after punctuation.

**Rationale:**
- Prevents `"1.2.3"` → `"1. 2. 3"`
- Preserves common version number formatting
- Heuristic: digits after punctuation likely part of number

**Alternatives considered:**
- Always add space (rejected: breaks version numbers)
- More sophisticated number detection (rejected: complexity not justified)

## 8. No async API

**Decision:** `NormaliseSentence` is synchronous.

**Rationale:**
- CPU-bound, no I/O, no await points
- Async would add overhead (state machine, allocation) for no benefit
- Callers can `Task.Run` if needed for offloading

## 9. No logging or diagnostics

**Decision:** Zero observability built in.

**Rationale:**
- Library is a pure function — no side effects to observe
- Consumers can wrap for logging if needed
- Avoids logging dependency and configuration

## 10. GPL-3.0-or-later license

**Decision:** Copyleft license.

**Rationale:**
- Ensures derivative works remain free
- Aligns with author's values
- NuGet package metadata declares license expression

## 11. net10.0 target only

**Decision:** Single TFM `net10.0`.

**Rationale:**
- Simplifies build, test, package
- No `#if` conditionals for API differences
- .NET 10 is current at time of writing

**Alternatives considered:**
- Multi-target net8.0/net9.0/net10.0 (rejected: no demand, adds complexity)

## 12. ImplicitUsings disabled

**Decision:** `<ImplicitUsings>disable</ImplicitUsings>`.

**Rationale:**
- Explicit `using` directives improve readability
- Avoids hidden namespace imports
- Consistent with explicit coding style

## 13. NUnit for testing

**Decision:** NUnit 4.x with NUnit3TestAdapter.

**Rationale:**
- Mature, feature-rich test framework
- `TestCase` attribute supports parameterised tests cleanly
- Good VS Test Explorer integration via adapter

## 14. Test naming convention

**Decision:** `Given{Context}_When{Action}_Then{Outcome}` pattern.

**Rationale:**
- BDD-style names document behaviour
- Test explorer groups by Given/When/Then
- Clear failure messages: "Given A Valid String When Normalising Sentence Then The Result Has Been Normalised"

## 15. No Moq usage in current tests

**Decision:** Tests instantiate `NuciTextNormaliser` directly.

**Rationale:**
- Implementation is stateless, no dependencies to mock
- Direct instantiation is simpler and faster
- Moq available for future interface-mocking needs

## 16. CI on ubuntu-latest only

**Decision:** Single Linux runner in GitHub Actions.

**Rationale:**
- .NET 10 cross-platform; Linux sufficient for build/test
- Faster, cheaper than matrix
- No platform-specific code to validate

**Alternatives considered:**
- Windows + Linux + macOS matrix (rejected: no native deps, no platform behaviour differences)