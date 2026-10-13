# Ambiguities and open questions

## Unresolved items

### 1. Culture-specific capitalisation

**Question:** Should `ToUpper()` use invariant culture or current culture?

**Current behaviour:** Uses current thread culture (`ToUpper()` without culture parameter).

**Impact:** Turkish `i` → `İ` instead of `I`; other locale-specific mappings.

**Options:**
- Change to `ToUpperInvariant()` — consistent but may surprise non-English users
- Add `CultureInfo` parameter — breaking change, adds complexity
- Document current behaviour — simplest, but leaves ambiguity

**Recommendation:** Document current behaviour; consider `ToUpperInvariant()` in v2.0.

### 2. Version number heuristic scope

**Question:** What patterns should be protected from space insertion?

**Current behaviour:** Only protects digits immediately after punctuation (`([,!?;:])([^\s\d])`).

**Gaps:**
- `v1.2.3` — `v` is not a digit, space added: `v 1.2.3`
- `Version 1.2.3` — already has space, no issue
- `1.2.3-beta` — `-` not in punctuation set, no protection
- `1,000` — comma before digits protected, but `1, 000` would be modified

**Options:**
- Expand punctuation set to include `-`, `:`, etc.
- Use more sophisticated number detection (regex for version patterns)
- Document as known limitation

**Recommendation:** Document as known limitation; expand if real-world cases reported.

### 3. Multiple sentence handling

**Question:** Should the library explicitly support multi-sentence input?

**Current behaviour:** Capitalises after `.!?` + whitespace, but doesn't separate sentences.

**Ambiguity:** Input `"hello.world"` (no space) → `"Hello.world."` (no capitalisation after `.`)

**Options:**
- Add space after sentence-ending punctuation if missing
- Document as single-sentence assumption
- Add `NormaliseParagraph` method

**Recommendation:** Document single-sentence assumption; consider `NormaliseParagraph` in future.

### 4. Ellipsis handling

**Question:** How should `...` be treated?

**Current behaviour:** Ends with `.` → no period appended. But `...` is three characters.

**Edge cases:**
- `"hello..."` → `"Hello..."` (correct)
- `"hello... world"` → `"Hello... World."` (capitalises after `...` + space)
- `"hello...world"` → `"Hello...world."` (no space after `...`)

**Options:**
- Treat `...` as single punctuation unit
- Document current behaviour
- Add ellipsis-aware patterns

**Recommendation:** Document current behaviour.

### 5. Non-ASCII punctuation

**Question:** Should the library handle Unicode punctuation?

**Current behaviour:** Only ASCII punctuation `. , ! ? ; :` in patterns.

**Gaps:**
- Ideographic full stop `。` (U+3002)
- Arabic comma `،` (U+060C)
- Other Unicode punctuation categories

**Options:**
- Use Unicode categories (`\p{P}`) in regex
- Document ASCII-only scope
- Add locale-specific variants

**Recommendation:** Document ASCII-only scope; consider Unicode in v2.0.

### 6. Empty string vs null distinction

**Question:** Should empty string and null be distinguished?

**Current behaviour:** Both return `null`.

**Options:**
- Return `""` for empty string, `null` for null
- Keep current (both → `null`)
- Throw for null, return `""` for empty

**Recommendation:** Keep current; documented in invariants.

### 7. Performance for pathological input

**Question:** Is O(n²) space collapse acceptable?

**Current behaviour:** Iterative `Replace` for repeated spaces.

**Worst case:** `"a" + " " * 10000 + "b"` → ~5000 iterations, ~5000 string allocations.

**Options:**
- Replace with `Regex.Replace(input, @"\s{2,}", " ")` — single pass
- Use `StringBuilder` single pass
- Accept as is (sentence-scale input unlikely to trigger)

**Recommendation:** Accept for now; optimise if profiling shows issue.

### 8. Interface extensibility

**Question:** Should `INuciTextNormaliser` have more methods?

**Current behaviour:** Single method `NormaliseSentence`.

**Options:**
- Add `NormaliseParagraph`, `NormaliseDocument`
- Add `Normalise(string, NormalisationOptions)`
- Keep minimal

**Recommendation:** Keep minimal; new methods = new interface or major version.

### 9. Nullable reference types

**Question:** Should the project enable nullable reference types?

**Current behaviour:** Not explicitly enabled (`<Nullable>enable</Nullable>` not set).

**Impact:** Compiler doesn't warn about null handling in consumer code.

**Options:**
- Enable `<Nullable>enable</Nullable>` in csproj
- Add nullable annotations to interface
- Keep as is

**Recommendation:** Enable in next minor version with annotations.

### 10. XML documentation generation

**Question:** Should the NuGet package include XML docs?

**Current behaviour:** `<GenerateDocumentationFile>` not set.

**Impact:** No IntelliSense for consumers.

**Options:**
- Enable `<GenerateDocumentationFile>true</GenerateDocumentationFile>`
- Add `<DocumentationFile>` path
- Keep as is

**Recommendation:** Enable for better developer experience.

## Known gaps in test coverage

| Gap | Priority | Description |
|-----|----------|-------------|
| Space before punctuation removal | Medium | No test for `"hello , world"` → `"hello, world"` |
| Space after punctuation insertion | Medium | No test for `"hello,world"` → `"hello, world"` |
| Multiple consecutive punctuation | Low | `"hello!! world"` |
| Ellipsis handling | Low | `"hello..."` |
| Non-ASCII punctuation | Low | `"hello。world"` |
| Culture-specific capitalisation | Medium | Turkish `i` test |
| Idempotence | Low | `Normalise(Normalise(x)) == Normalise(x)` |
| Very long input | Low | Performance test |

## Future considerations

### Potential v2.0 breaking changes

1. **Enable nullable reference types** — requires consumer updates
2. **Change to `ToUpperInvariant()`** — changes output for some locales
3. **Add `NormaliseParagraph` method** — new interface or extension
4. **Add configuration options** — new interface or options parameter
5. **Multi-target .NET 8/9/10** — broader compatibility

### Potential v1.x enhancements

1. **XML documentation** — no breaking change
2. **Additional test cases** — no breaking change
3. **Performance optimisation** — no breaking change if output identical
4. **CI improvements** — no breaking change

## Decision tracking

| Item | Status | Decision | Date |
|------|--------|----------|------|
| Culture handling | Open | Document current | 2026-10-13 |
| Version heuristic | Open | Document limitation | 2026-10-13 |
| Multi-sentence | Open | Document assumption | 2026-10-13 |
| Ellipsis | Open | Document current | 2026-10-13 |
| Unicode punctuation | Open | Document ASCII-only | 2026-10-13 |
| Null vs empty | Resolved | Both → null | 2026-10-13 |
| Space collapse perf | Open | Accept for now | 2026-10-13 |
| Interface extensibility | Resolved | Keep minimal | 2026-10-13 |
| Nullable ref types | Open | Enable in v1.1 | 2026-10-13 |
| XML docs | Open | Enable in v1.1 | 2026-10-13 |