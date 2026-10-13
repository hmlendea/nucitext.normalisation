# Data model

## Domain entities

This library has no persistent domain entities. It operates on a single primitive input and produces a single primitive output.

## Input

| Property | Type | Constraints | Description |
|----------|------|-------------|-------------|
| `input` | `string` | Can be `null`, empty, whitespace, or any UTF-16 string | Raw sentence text to normalise |

## Output

| Property | Type | Constraints | Description |
|----------|------|-------------|-------------|
| Return value | `string` | `null` if input is null/whitespace; otherwise non-empty, trimmed, normalised string | Normalised sentence |

## Data transformations

The `NormaliseSentence` method applies a deterministic pipeline of transformations:

### Pipeline stages

| Stage | Operation | Implementation | Input → Output example |
|-------|-----------|----------------|------------------------|
| 1 | Null/whitespace guard | `string.IsNullOrWhiteSpace(input)` | `null` → `null`; `"   "` → `null` |
| 2 | Trim | `input.Trim()` | `"  hello  "` → `"hello"` |
| 3 | Collapse repeated spaces | `while (result.Contains("  ")) result = result.Replace("  ", " ")` | `"a  b   c"` → `"a b c"` |
| 4 | Remove spaces before punctuation | `Regex.Replace(result, @"\s+([.,!?;:])", "$1")` | `"hello , world"` → `"hello, world"` |
| 5 | Ensure space after punctuation | `Regex.Replace(result, @"([,!?;:])([^\s\d])", "$1 $2")` | `"hello,world"` → `"hello, world"` |
| 6 | Capitalise sentence starts | `Regex.Replace(result, @"(^\s*[a-z])|([.!?]\s+[a-z])", m => m.Value.ToUpper())` | `"hello. world"` → `"Hello. World"` |
| 7 | Append terminal period | `if (!Regex.IsMatch(result, @"[.!?]$")) result += "."` | `"hello"` → `"hello."` |

### Transformation details

#### Stage 3: Collapse repeated spaces
- **Algorithm:** Iterative string replacement (not regex)
- **Complexity:** O(n²) worst case for many repeated spaces; O(n) typical
- **Note:** Could be optimised to single-pass or regex `\s{2,}`

#### Stage 4: Remove spaces before punctuation
- **Pattern:** `\s+([.,!?;:])`
- **Replacement:** `$1` (captured punctuation)
- **Matches:** One or more whitespace chars before `. , ! ? ; :`
- **Edge case:** Does not handle spaces before `)` or `]`

#### Stage 5: Ensure space after punctuation
- **Pattern:** `([,!?;:])([^\s\d])`
- **Replacement:** `$1 $2`
- **Matches:** Punctuation followed by non-whitespace, non-digit
- **Rationale:** Avoids adding space after version numbers like `1.2.3`
- **Edge case:** Does not add space after `.` if followed by digit

#### Stage 6: Capitalise sentence starts
- **Pattern:** `(^\s*[a-z])|([.!?]\s+[a-z])`
- **Replacement:** `match.Value.ToUpper()`
- **Matches:**
  - Start of string (optional whitespace) + lowercase letter
  - Sentence-ending punctuation (`. ! ?`) + whitespace + lowercase letter
- **Culture:** Uses `ToUpper()` without culture → current thread culture
- **Edge case:** Does not handle `...` ellipsis or `?`/`!` mid-sentence

#### Stage 7: Append terminal period
- **Check:** `Regex.IsMatch(result, @"[.!?]$")`
- **Action:** Append `.` if no match
- **Edge case:** Input ending with `...` → no period added (ends with `.`)

## Invariants

| Invariant | Verification |
|-----------|--------------|
| Null/whitespace input → null output | Stage 1 guard |
| Output never empty string | Stage 1 + Stage 7 guarantee |
| Output trimmed | Stage 2 |
| No repeated spaces | Stage 3 |
| No space before punctuation | Stage 4 |
| Space after punctuation (except before digits) | Stage 5 |
| First letter of each sentence capitalised | Stage 6 |
| Ends with `.`, `!`, or `?` | Stage 7 |

## Edge cases and known behaviours

| Input | Output | Notes |
|-------|--------|-------|
| `null` | `null` | Guard clause |
| `""` | `null` | Guard clause |
| `"   "` | `null` | Guard clause |
| `"hello"` | `"Hello."` | Capitalised + period |
| `"HELLO"` | `"HELLO."` | Already capitalised + period |
| `"hello world"` | `"Hello world."` | Single sentence |
| `"hello. world"` | `"Hello. World."` | Two sentences |
| `"hello , world"` | `"Hello, world."` | Space before comma removed |
| `"hello,world"` | `"Hello, world."` | Space after comma added |
| `"version 1.2.3"` | `"Version 1.2.3."` | No space after `.` before digit |
| `"hello..."` | `"Hello..."` | Ends with `.` → no extra period |
| `"hello!?"` | `"Hello!?"` | Ends with `?` → no extra period |

## Serialisation

Not applicable — no serialisation, no DTOs, no JSON, no persistence.

## Versioning

No data model versioning. The input/output contract is the method signature.
Breaking changes to normalisation behaviour would require major version bump.