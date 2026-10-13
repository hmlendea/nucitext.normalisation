# Invariants

## System-wide invariants

| # | Invariant | Description | Enforcement |
|---|-----------|-------------|-------------|
| 1 | **Null/whitespace in → null out** | `NormaliseSentence(null)`, `NormaliseSentence("")`, `NormaliseSentence("   ")` all return `null` | Guard clause at method entry |
| 2 | **Non-null output is non-empty** | If input passes guard, output is never `""` | Trim + append period guarantees length ≥ 1 |
| 3 | **Output is trimmed** | No leading/trailing whitespace in result | `Trim()` at stage 2 |
| 4 | **No repeated spaces** | No occurrence of `"  "` (two spaces) in result | Stage 3 collapse loop |
| 5 | **No space before punctuation** | No whitespace immediately before `. , ! ? ; :` | Stage 4 regex removal |
| 6 | **Space after punctuation (except before digits)** | Punctuation `. , ! ? ; :` followed by non-whitespace, non-digit gets a space | Stage 5 regex insertion |
| 7 | **Sentence starts capitalised** | First letter of string and first letter after `. ! ?` + whitespace is uppercase | Stage 6 regex replacement |
| 8 | **Ends with terminal punctuation** | Result ends with `.`, `!`, or `?` | Stage 7 append check |
| 9 | **Deterministic** | Same input always produces same output | Pure function, no randomness, no external state |
| 10 | **No exceptions** | Method never throws for any string input | All operations guarded; regex patterns constant |
| 11 | **Thread-safe** | Concurrent calls produce correct independent results | No shared mutable state |
| 12 | **Idempotent** | `NormaliseSentence(NormaliseSentence(x)) == NormaliseSentence(x)` | Pipeline converges to fixed point |

## Method-level contracts

### `INuciTextNormaliser.NormaliseSentence`

**Preconditions:**
- None (accepts any `string` including `null`)

**Postconditions:**
- Returns `null` iff `string.IsNullOrWhiteSpace(input)`
- Returns non-null `string` otherwise
- Return value satisfies invariants 3–8 above

**Side effects:**
- None (pure function)

**Thread safety:**
- Safe for concurrent calls

**Complexity:**
- Time: O(n) typical, O(n²) worst case (space collapse loop)
- Space: O(n) allocations (strings, regex matches)

## Interface invariants

| Invariant | Description |
|-----------|-------------|
| **Single method** | Interface has exactly one member: `NormaliseSentence` |
| **No properties/events** | Pure behavioural contract |
| **No generic parameters** | Closed over `string` |

## Implementation invariants

| Invariant | Description |
|-----------|-------------|
| **Sealed** | `NuciTextNormaliser` cannot be inherited |
| **Stateless** | No instance fields, no static fields |
| **No virtual members** | No override points |
| **Single constructor** | Implicit parameterless constructor only |

## Test invariants

| Invariant | Verification |
|-----------|--------------|
| **Null/whitespace → null** | 3 test cases: `null`, `""`, `"   "` |
| **Valid input → non-null** | 2 test cases |
| **Valid input → non-empty** | 2 test cases |
| **Normalisation correctness** | 7 parameterised test cases covering pipeline stages |

## Version invariants

| Invariant | Description |
|-----------|-------------|
| **Semantic versioning** | `<Version>` in csproj follows SemVer |
| **Breaking change = major** | Any change to normalisation output for same input requires major version bump |
| **Interface stability** | `INuciTextNormaliser` shape unchanged since v1.0.0 |

## License invariant

| Invariant | Description |
|-----------|-------------|
| **GPL-3.0-or-later** | Package license expression fixed; cannot be changed without major version and legal review |

## Build invariants

| Invariant | Description |
|-----------|-------------|
| **net10.0 only** | Single TFM; no multi-targeting |
| **ImplicitUsings disabled** | Explicit `using` required |
| **No analyzers** | Build passes with zero warnings (currently) |

## CI invariants

| Invariant | Description |
|-----------|-------------|
| **Build on master/PR** | `.github/workflows/dotnet.yml` runs on push/PR to master |
| **Test must pass** | `dotnet test` failure fails CI |
| **Single platform** | ubuntu-latest only |

## Known invariant violations (edge cases)

| Invariant | Violation scenario | Impact |
|-----------|-------------------|--------|
| Space after punctuation | `"hello,world"` → `"hello, world"` but `"1.2.3"` unchanged | By design (version protection) |
| Capitalisation culture | `ToUpper()` uses thread culture | Turkish `i` → `İ` may differ from expectation |
| Space collapse performance | `"a" + " " * 10000 + "b"` → O(n²) | Acceptable for sentence input |
| Multiple sentences | `"hello. world"` → `"Hello. World."` (not `"Hello. World."`) | Treats as single text block |

## Invariant testing strategy

| Invariant | Test coverage |
|-----------|---------------|
| 1–2 | `GivenANullOrWhitespaceString_WhenNormalisingSentence_ThenTheResultIsEmpty` |
| 3–8 | `GivenAValidString_WhenNormalisingSentence_ThenTheResultHasBeenNormaliseSentenced` (7 cases) |
| 9 | Implicit (deterministic implementation) |
| 10 | Implicit (no throw in any test) |
| 11 | Not explicitly tested (stateless) |
| 12 | Not explicitly tested (could add) |