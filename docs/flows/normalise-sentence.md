# Normalise sentence flow

## End-to-end execution flow

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                        NormaliseSentence(string input)                      │
└─────────────────────────────────────────────────────────────────────────────┘
                                      │
                                      ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│  STAGE 1: Null/whitespace guard                                             │
│  ┌─────────────────────────────────────────────────────────────────────┐   │
│  │ if (string.IsNullOrWhiteSpace(input))                               │   │
│  │     return null;  ◄─────────────────────────────────────────────────┘   │
│  └─────────────────────────────────────────────────────────────────────┘   │
│                                      │                                      │
│                                      ▼ (input valid)                        │
└─────────────────────────────────────────────────────────────────────────────┘
                                      │
                                      ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│  STAGE 2: Trim                                                              │
│  ┌─────────────────────────────────────────────────────────────────────┐   │
│  │ string result = input.Trim();                                       │   │
│  └─────────────────────────────────────────────────────────────────────┘   │
│                                      │                                      │
│                                      ▼                                      │
└─────────────────────────────────────────────────────────────────────────────┘
                                      │
                                      ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│  STAGE 3: Collapse repeated spaces (iterative)                              │
│  ┌─────────────────────────────────────────────────────────────────────┐   │
│  │ while (result.Contains("  "))                                       │   │
│  │ {                                                                   │   │
│  │     result = result.Replace("  ", " ");  // Allocate new string    │   │
│  │ }                                                                   │   │
│  └─────────────────────────────────────────────────────────────────────┘   │
│                                      │                                      │
│                                      ▼                                      │
└─────────────────────────────────────────────────────────────────────────────┘
                                      │
                                      ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│  STAGE 4: Remove spaces before punctuation (Regex)                          │
│  ┌─────────────────────────────────────────────────────────────────────┐   │
│  │ result = Regex.Replace(                                             │   │
│  │     result,                                                         │   │
│  │     @"\s+([.,!?;:])",  // Pattern: whitespace before punctuation   │   │
│  │     "$1"               // Replacement: captured punctuation only   │   │
│  │ );                                                                  │   │
│  └─────────────────────────────────────────────────────────────────────┘   │
│                                      │                                      │
│                                      ▼                                      │
└─────────────────────────────────────────────────────────────────────────────┘
                                      │
                                      ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│  STAGE 5: Ensure space after punctuation (Regex)                            │
│  ┌─────────────────────────────────────────────────────────────────────┐   │
│  │ result = Regex.Replace(                                             │   │
│  │     result,                                                         │   │
│  │     @"([,!?;:])([^\s\d])",  // Punctuation + non-space, non-digit  │   │
│  │     "$1 $2"              // Punctuation + space + following char   │   │
│  │ );                                                                  │   │
│  └─────────────────────────────────────────────────────────────────────┘   │
│                                      │                                      │
│                                      ▼                                      │
└─────────────────────────────────────────────────────────────────────────────┘
                                      │
                                      ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│  STAGE 6: Capitalise sentence starts (Regex + lambda)                       │
│  ┌─────────────────────────────────────────────────────────────────────┐   │
│  │ result = Regex.Replace(                                             │   │
│  │     result,                                                         │   │
│  │     @"(^\s*[a-z])|([.!?]\s+[a-z])",  // Start or post-punctuation  │   │
│  │     match => match.Value.ToUpper()   // Uppercase entire match     │   │
│  │ );                                                                  │   │
│  └─────────────────────────────────────────────────────────────────────┘   │
│                                      │                                      │
│                                      ▼                                      │
└─────────────────────────────────────────────────────────────────────────────┘
                                      │
                                      ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│  STAGE 7: Append terminal period if missing (Regex check)                   │
│  ┌─────────────────────────────────────────────────────────────────────┐   │
│  │ if (!Regex.IsMatch(result, @"[.!?]$"))                              │   │
│  │ {                                                                   │   │
│  │     result += ".";  // Append period                                │   │
│  │ }                                                                   │   │
│  └─────────────────────────────────────────────────────────────────────┘   │
│                                      │                                      │
│                                      ▼                                      │
└─────────────────────────────────────────────────────────────────────────────┘
                                      │
                                      ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│  RETURN result                                                              │
└─────────────────────────────────────────────────────────────────────────────┘
```

## Data flow trace

### Example: `"   hello   world!   "`

| Stage | Operation | Result |
|-------|-----------|--------|
| Input | — | `"   hello   world!   "` |
| 1 | Guard | Pass (not null/whitespace) |
| 2 | Trim | `"hello   world!"` |
| 3 | Collapse spaces | `"hello world!"` |
| 4 | Remove space before `!` | `"hello world!"` (no space before) |
| 5 | Space after punctuation | `"hello world!"` (no match) |
| 6 | Capitalise | `"Hello world!"` |
| 7 | Terminal punctuation | `"Hello world!"` (ends with `!`) |
| Return | — | `"Hello world!"` |

### Example: `"this one should be capitalised. yes it should!"`

| Stage | Operation | Result |
|-------|-----------|--------|
| Input | — | `"this one should be capitalised. yes it should!"` |
| 1 | Guard | Pass |
| 2 | Trim | `"this one should be capitalised. yes it should!"` |
| 3 | Collapse spaces | `"this one should be capitalised. yes it should!"` |
| 4 | Remove space before punctuation | `"this one should be capitalised. yes it should!"` |
| 5 | Space after punctuation | `"this one should be capitalised. yes it should!"` |
| 6 | Capitalise | `"This one should be capitalised. Yes it should!"` |
| 7 | Terminal punctuation | `"This one should be capitalised. Yes it should!"` |
| Return | — | `"This one should be capitalised. Yes it should!"` |

### Example: `"Test sentence is testy"`

| Stage | Operation | Result |
|-------|-----------|--------|
| Input | — | `"Test sentence is testy"` |
| 1 | Guard | Pass |
| 2 | Trim | `"Test sentence is testy"` |
| 3 | Collapse spaces | `"Test sentence is testy"` |
| 4 | Remove space before punctuation | `"Test sentence is testy"` |
| 5 | Space after punctuation | `"Test sentence is testy"` |
| 6 | Capitalise | `"Test sentence is testy"` (already capitalised) |
| 7 | Terminal punctuation | `"Test sentence is testy."` (appends `.`) |
| Return | — | `"Test sentence is testy."` |

## Call graph

```
NormaliseSentence(string input)
│
├── string.IsNullOrWhiteSpace(input)  → bool
│
├── input.Trim()  → string
│
├── while (result.Contains("  "))
│   └── result.Replace("  ", " ")  → string (repeated)
│
├── Regex.Replace(result, @"\s+([.,!?;:])", "$1")  → string
│   └── Regex engine: match, capture, replace
│
├── Regex.Replace(result, @"([,!?;:])([^\s\d])", "$1 $2")  → string
│   └── Regex engine: match, capture, replace
│
├── Regex.Replace(result, @"(^\s*[a-z])|([.!?]\s+[a-z])", m => m.Value.ToUpper())  → string
│   └── Regex engine: match, lambda per match
│       └── match.Value.ToUpper()  → string (per match)
│
├── Regex.IsMatch(result, @"[.!?]$")  → bool
│
└── result += "."  → string (conditional)
```

## Allocation profile

| Stage | Allocations |
|-------|-------------|
| 1 | None (guard) |
| 2 | 1 string (trimmed) |
| 3 | N strings (one per `Replace` iteration) |
| 4 | 1 string + regex match objects |
| 5 | 1 string + regex match objects |
| 6 | 1 string + regex match objects + lambda captures |
| 7 | 0 or 1 string (conditional append) |
| **Total** | ~5-15 strings + regex overhead |

## Error paths

| Stage | Possible exception | Handling |
|-------|-------------------|----------|
| 1 | None | — |
| 2 | `NullReferenceException` | Impossible (guard passed) |
| 3 | `OutOfMemoryException` | Propagates (BCL) |
| 4 | `ArgumentException` (regex) | Impossible (constant pattern) |
| 4 | `OutOfMemoryException` | Propagates |
| 5 | Same as stage 4 | Propagates |
| 6 | Same as stage 4 | Propagates |
| 6 | `OutOfMemoryException` (lambda) | Propagates |
| 7 | `OutOfMemoryException` | Propagates |

**No try/catch in method** — exceptions propagate to caller.

## Concurrency flow

```
Thread A                    Thread B                    Thread C
   │                          │                          │
   ▼                          ▼                          ▼
NormaliseSentence("a")    NormaliseSentence("b")    NormaliseSentence("c")
   │                          │                          │
   ├─ Stage 1..7 (local)     ├─ Stage 1..7 (local)     ├─ Stage 1..7 (local)
   │                          │                          │
   ▼                          ▼                          ▼
Return "A."                Return "B."                Return "C."
```

- **No shared state** — each call has own `result` variable
- **No static fields** — no cross-thread interference
- **Regex is thread-safe** — static `Regex.Replace`/`IsMatch` are safe
- **String is immutable** — no mutation visible across threads

## Integration flow (DI)

```
┌─────────────┐     ┌──────────────────────┐     ┌────────────────────┐
│  Consumer   │────▶│  INuciTextNormaliser │────▶│ NuciTextNormaliser │
│  (service)  │     │   (interface)        │     │   (implementation) │
└─────────────┘     └──────────────────────┘     └────────┬───────────┘
                                                          │
                                                          ▼
                                                ┌────────────────────┐
                                                │ System.Text.       │
                                                │ RegularExpressions │
                                                └────────────────────┘
```

1. Consumer requests `INuciTextNormaliser` via DI
2. Container provides `NuciTextNormaliser` singleton
3. Consumer calls `NormaliseSentence(input)`
4. Implementation executes pipeline
5. Result returned to consumer

## Test flow

```
Test: GivenAValidString_WhenNormalisingSentence_ThenTheResultHasBeenNormaliseSentenced
│
├── Setup: normaliser = new NuciTextNormaliser()
│
├── Act: result = normaliser.NormaliseSentence(input)
│
├── Assert: Assert.That(result, Is.EqualTo(expected))
│
└── Teardown: (none, stateless)
```

## Performance flow (hot path)

```
Input string (stack/reference)
    │
    ▼
Trim() ──────────────────▶ New string (heap)
    │
    ▼
while (Contains) ───────▶ N × Replace() ──────▶ N strings (heap)
    │                          │
    │                          ▼
    │                    (optimisation: could use StringBuilder)
    │
    ▼
Regex.Replace ──────────▶ New string + MatchCollection (heap)
    │
    ▼
Regex.Replace ──────────▶ New string + MatchCollection (heap)
    │
    ▼
Regex.Replace (lambda) ─▶ New string + MatchCollection + lambdas (heap)
    │
    ▼
Regex.IsMatch ──────────▶ bool (stack)
    │
    ▼
Conditional append ─────▶ 0 or 1 string (heap)
    │
    ▼
Return string (heap)
```

## Optimisation opportunities

| Stage | Current | Optimised |
|-------|---------|-----------|
| 3 | Iterative `Replace` | `Regex.Replace(input, @"\s{2,}", " ")` or `StringBuilder` single pass |
| 4-6 | 3 separate regex passes | Single combined regex with `MatchEvaluator` |
| 7 | Separate `IsMatch` | Check last char directly: `result[^1]` |

**Trade-off:** Current code prioritises readability over allocation reduction. Acceptable for sentence-scale input.