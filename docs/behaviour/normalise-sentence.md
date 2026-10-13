# Normalise sentence behaviour

## Overview

`NormaliseSentence` transforms a raw sentence string into a consistently formatted output by applying a deterministic pipeline of seven stages. The method is pure (no side effects), stateless, and thread-safe.

## Input contract

| Input | Behaviour |
|-------|-----------|
| `null` | Returns `null` immediately |
| `""` (empty) | Returns `null` immediately |
| Whitespace only (`" "`, `"\t\n"`, etc.) | Returns `null` immediately |
| Any other string | Processes through full pipeline |

## Output contract

| Condition | Output |
|-----------|--------|
| Input was null/whitespace | `null` |
| Input was valid | Non-null, non-empty string satisfying all invariants |

## Pipeline stages (in order)

### Stage 1: Null/whitespace guard
```
if (string.IsNullOrWhiteSpace(input)) return null;
```
- **Purpose:** Fast exit for invalid input
- **Effect:** No allocation, no further processing

### Stage 2: Trim
```
result = input.Trim();
```
- **Purpose:** Remove leading/trailing whitespace
- **Effect:** `"  hello  "` → `"hello"`

### Stage 3: Collapse repeated spaces
```
while (result.Contains("  "))
    result = result.Replace("  ", " ");
```
- **Purpose:** Normalise internal whitespace to single spaces
- **Effect:** `"a  b   c"` → `"a b c"`
- **Algorithm:** Iterative replacement (not regex)
- **Complexity:** O(n²) worst case, O(n) typical

### Stage 4: Remove spaces before punctuation
```
result = Regex.Replace(result, @"\s+([.,!?;:])", "$1");
```
- **Purpose:** Eliminate whitespace immediately before punctuation marks
- **Pattern:** `\s+([.,!?;:])` — one or more whitespace chars before `. , ! ? ; :`
- **Replacement:** `$1` (the captured punctuation)
- **Effect:** `"hello , world"` → `"hello, world"`

### Stage 5: Ensure space after punctuation
```
result = Regex.Replace(result, @"([,!?;:])([^\s\d])", "$1 $2");
```
- **Purpose:** Insert single space after punctuation when followed by non-whitespace, non-digit
- **Pattern:** `([,!?;:])([^\s\d])` — punctuation followed by non-space, non-digit
- **Replacement:** `$1 $2` — punctuation + space + following char
- **Effect:** `"hello,world"` → `"hello, world"`
- **Protection:** Does not add space after `.` before digit (preserves `1.2.3`)

### Stage 6: Capitalise sentence starts
```
result = Regex.Replace(result, @"(^\s*[a-z])|([.!?]\s+[a-z])", m => m.Value.ToUpper());
```
- **Purpose:** Uppercase first letter of string and first letter after sentence-ending punctuation
- **Pattern:** `(^\s*[a-z])|([.!?]\s+[a-z])` — start-of-string + lowercase OR `.!?` + whitespace + lowercase
- **Replacement:** `match.Value.ToUpper()` — entire match uppercased
- **Culture:** Uses current thread culture (not invariant)
- **Effect:** `"hello. world"` → `"Hello. World"`

### Stage 7: Append terminal period
```
if (!Regex.IsMatch(result, @"[.!?]$"))
    result += ".";
```
- **Purpose:** Ensure output ends with sentence-terminating punctuation
- **Check:** `Regex.IsMatch(result, @"[.!?]$")` — ends with `.`, `!`, or `?`
- **Action:** Append `.` if missing
- **Effect:** `"hello"` → `"hello."`; `"hello!"` unchanged

## Behavioural examples

| Input | Output | Stages triggered |
|-------|--------|------------------|
| `null` | `null` | 1 only |
| `""` | `null` | 1 only |
| `"   "` | `null` | 1 only |
| `"hello"` | `"Hello."` | 2,3,6,7 |
| `"  hello  "` | `"Hello."` | 2,3,6,7 |
| `"hello world"` | `"Hello world."` | 2,3,6,7 |
| `"hello   world"` | `"Hello world."` | 2,3,6,7 |
| `"hello , world"` | `"Hello, world."` | 2,3,4,6,7 |
| `"hello,world"` | `"Hello, world."` | 2,3,5,6,7 |
| `"hello. world"` | `"Hello. World."` | 2,3,6,7 |
| `"this. that. other"` | `"This. That. Other."` | 2,3,6,7 |
| `"version 1.2.3"` | `"Version 1.2.3."` | 2,3,5,6,7 (stage 5 skips digits) |
| `"hello..."` | `"Hello..."` | 2,3,6,7 (ends with `.`) |
| `"HELLO"` | `"HELLO."` | 2,3,7 (already capitalised) |

## Edge cases

| Scenario | Behaviour | Note |
|----------|-----------|------|
| Multiple spaces before punctuation | All removed | Stage 4 uses `\s+` |
| Punctuation followed by digit | No space added | Stage 5 excludes `\d` |
| Punctuation followed by space | Unchanged | Stage 5 requires non-space |
| Already capitalised | Unchanged | `ToUpper()` on uppercase is no-op |
| Already ends with `!` or `?` | Unchanged | Stage 7 checks `[.!?]` |
| Input ends with `...` | No extra period | Ends with `.` |
| Non-ASCII letters | `ToUpper()` per culture | Turkish `i` → `İ` |
| Empty after trim | Impossible | Guard catches whitespace-only |

## Idempotence

`NormaliseSentence(NormaliseSentence(x)) == NormaliseSentence(x)` for all valid inputs.

The pipeline converges to a fixed point because:
- Stage 3: Already collapsed → no change
- Stage 4: No spaces before punctuation → no match
- Stage 5: Already spaced → no match
- Stage 6: Already capitalised → `ToUpper()` no-op
- Stage 7: Already ends with punctuation → no append

## Performance characteristics

| Metric | Value |
|--------|-------|
| Time complexity | O(n) typical, O(n²) worst case (space collapse) |
| Space complexity | O(n) allocations (strings, regex matches) |
| Allocations per call | ~5-10 strings + regex match objects |
| Thread contention | None (stateless) |

## Limitations

| Limitation | Impact |
|------------|--------|
| Single sentence assumption | Multiple sentences capitalised but not separated |
| No locale parameter | Uses thread culture for `ToUpper()` |
| No configuration | Pipeline fixed at compile time |
| Version heuristic only | `1.2.3` protected but `v1.2.3` not |
| No HTML/markdown awareness | Treats markup as plain text |

## Error handling

| Error condition | Handling |
|-----------------|----------|
| `null` input | Returns `null` (not exception) |
| Regex failure | Impossible (patterns constant) |
| Out of memory | Propagates `OutOfMemoryException` (BCL) |
| Stack overflow | Impossible (no recursion) |

## Thread safety

- **Stateless:** No instance or static fields
- **Reentrant:** Safe for concurrent calls
- **No locks:** No synchronization needed
- **Immutable input:** `string` is immutable

## Composability

```csharp
// Chain with other string operations
string result = normaliser.NormaliseSentence(input)?.ToLowerInvariant();

// Use in LINQ
var normalised = inputs.Select(normaliser.NormaliseSentence).Where(s => s != null);

// Safe null handling
string display = normaliser.NormaliseSentence(userInput) ?? "[empty]";
```